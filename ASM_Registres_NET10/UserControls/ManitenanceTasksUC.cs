using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;

namespace ASM_Registres.UserControls
{
    public partial class MaintenanceTasksUC : UserControl
    {
        private const int MaintenanceGroupId = 16;
        private static readonly Random RandomGenerator = new Random();

        private readonly NPGSQLService _dbService;
        private readonly TareasMantenimientoRepository _taskRepo;
        private readonly RegistroMantenimientoRepository _recordRepo;
        private readonly User _currentUser;
        private List<RegistreManteniment> _records;

        public MaintenanceTasksUC(User user)
        {
            InitializeComponent();
            _currentUser = user;
            _dbService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);
            _taskRepo = new TareasMantenimientoRepository(_dbService);
            _recordRepo = new RegistroMantenimientoRepository(_dbService);
            _records = new List<RegistreManteniment>();

            ConfigureLayout();
            AttachSchedulerEvents();
            LoadAndBindRecords();
        }

        private void ConfigureLayout()
        {
            panel1.Dock = DockStyle.Top;
            panel2.Dock = DockStyle.Fill;

            panel1.Controls.Add(radSchedulerNavigator1);
            panel2.Controls.Add(radCalendar1);

            radSchedulerNavigator1.Dock = DockStyle.Top;
            radCalendar1.Dock = DockStyle.Fill;

            radSchedulerNavigator1.AssociatedScheduler = radCalendar1;
            radCalendar1.ActiveViewType = SchedulerViewType.Month;
        }

        private void AttachSchedulerEvents()
        {
            radCalendar1.AppointmentAdded += OnAppointmentAdded;
            radCalendar1.AppointmentDeleted += OnAppointmentDeleted;
            radCalendar1.AppointmentChanged += OnAppointmentChanged;
            radCalendar1.AppointmentMoved += OnAppointmentMoved;
        }

        private void LoadAndBindRecords()
        {
            radCalendar1.Appointments.Clear();
            _records = _recordRepo.GetTodosRegistrosManteniment();
            foreach (var record in _records)
            {
                radCalendar1.Appointments.Add(CreateAppointment(record));
            }
        }

        private Appointment CreateAppointment(RegistreManteniment record)
        {
            return new Appointment
            {
                UniqueId = new EventId(record.Id),
                Summary = record.NomTasca,
                Description = record.Observacions,
                Start = record.ProperaDataProgramada,
                End = record.ProperaDataProgramada.AddHours(1),
                AllDay = true
            };
        }

        private void OnAppointmentAdded(object sender, AppointmentAddedEventArgs e)
        {
            if (e.Appointment == null) return;

            var templateId = _taskRepo.GetIdInternByNombreTasca(e.Appointment.Summary);
            if (e.Appointment.RecurrenceRule != null)
            {
                AddRecurringRecords(e.Appointment, templateId);
            }
            else
            {
                AddSingleRecord(e.Appointment, templateId);
            }
            LoadAndBindRecords();
        }

        private void AddSingleRecord(IEvent appointment, int templateId)
        {
            var record = MapAppointmentToRecord(appointment, templateId);
            _recordRepo.AddRegistreManteniment(record);
        }

        private void AddRecurringRecords(IEvent appointment, int templateId)
        {
            string lastId = null;
            foreach (var occurrence in appointment.Occurrences.OfType<IEvent>())
            {
                var record = MapAppointmentToRecord(occurrence, templateId);
                do
                {
                    record.Id = GenerateRandomId(15);
                } while (record.Id == lastId);
                lastId = record.Id;
                _recordRepo.AddRegistreManteniment(record);
            }
        }

        private void OnAppointmentDeleted(object sender, SchedulerAppointmentEventArgs e)
        {
            if (e.Appointment == null) return;
            _recordRepo.DeleteRegistreManteniment(
                e.Appointment.Summary,
                e.Appointment.Description,
                e.Appointment.Start);
        }

        private void OnAppointmentChanged(object sender, AppointmentChangedEventArgs e)
        {
            if (e.Appointment == null) return;
            UpdateRecord(e.Appointment);
            LoadAndBindRecords();
        }

        private void OnAppointmentMoved(object sender, AppointmentMovedEventArgs e)
        {
            if (e.Appointment == null) return;
            UpdateRecord(e.Appointment);
            LoadAndBindRecords();
        }

        private void UpdateRecord(IEvent appointment)
        {
            var templateId = _taskRepo.GetIdInternByNombreTasca(appointment.Summary);
            var record = MapAppointmentToRecord(appointment, templateId);
            _recordRepo.UpdateRegistreManteniment(record);
        }

        private RegistreManteniment MapAppointmentToRecord(IEvent appointment, int templateId)
        {
            return new RegistreManteniment
            {
                Id = appointment.UniqueId.ToString(),
                FetaPer = _currentUser.Name,
                NomTasca = appointment.Summary,
                Observacions = string.IsNullOrEmpty(appointment.Description) ? "Cap" : appointment.Description,
                IdGrup = MaintenanceGroupId,
                IdTascaInterna = templateId,
                ProperaDataProgramada = appointment.Start
            };
        }

        private static string GenerateRandomId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(0, length)
                .Select(_ => chars[RandomGenerator.Next(chars.Length)])
                .ToArray());
        }
    }
}