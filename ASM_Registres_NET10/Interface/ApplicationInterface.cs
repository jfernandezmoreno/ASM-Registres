using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Modules.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Registres_NET10
{
    public interface ApplicationInterface
    {
    }

    public interface IControlAguas
    {
        void AddControlAguas(ControlAguas controlAguas);
        List<ControlAguas> GetControlAguasPorFecha(DateTime fechaInicio, DateTime fechaFin);
    }
    public interface IRegistroComun
    {
        void InsertRegistroComun(RegistroComun registro);
        List<RegistroComun> GetRegistrosIncomplet();
        List<RegistroComun> GetRegistrosPendent();
        void UpdateRegistroComun(int id, string estat);
        void UpdateRegistroComun(int id, string estat, string obv);
        bool ExisteRegistroComun(int id);
        RegistroComun GetRegistroComun(int id);
        List<RegistroComun> GetRegistroComunesPorIdTasca(DateTime fechaInicio, DateTime fechaFin, int idTasca);
    }
    public interface IRegistroMantenimiento
    {
        List<RegistreManteniment> GetAllRegistresManteniment();
        void AddRegistreManteniment(RegistreManteniment registre);
        void UpdateRegistreManteniment(RegistreManteniment registre);
        List<RegistreManteniment> GetTodosRegistrosManteniment();
        void DeleteRegistreManteniment(string nomTasca, string observacions, DateTime properaDataProgramada);
        int GetMaxIdRegistreManteniment();
    }
    public interface IRegistroLimpieza
    {
        void InsertRegistroLimpieza(RegistroLimpieza registro);
    }
    public interface IRegistroResiduo
    {
        Task AddRegistreAsync(RegistreResidusLaboratori registre);
    }
    public interface IRegistroBascula
    {
        void AddRegistreBascula(RegistroBascula registre);
        List<RegistroBascula> GetRegistrosBasculaPendent();
        List<RegistroBascula> GetRegistrosBasculaIncomplet();
        void UpdateRegistroBascula(int id, string estat);
        bool ExisteRegistroBascula(int id);
        void UpdateRegistroBascula(int id, string estat, string obv);
        RegistroBascula GetRegistro(int id);
        List<RegistroBascula> GetRegistrosBasculaPorFechaYIdTasca(DateTime fechaInicio, DateTime fechaFin, int idTasca);

    }
    public interface IRegistroGeneral
    {
        void AddRegistroGeneral(RegistroGeneral registro);
        List<RegistroGeneral> GetAllRegistrosGenerales();
    }
    public interface IRegistroSilos
    {
        void InsertRegistroSilos(RegistroSilos registro);
    }
    public interface IRegistroTemperatura
    {
        void AddRegistroTermico(RegistroTemperaturaCamara registro);
    }
    public interface IBasculaGrande
    {
        List<CBascGranKoh> SelectCBascGranKoh();
    }
    public interface IBasculaPequena
    {
        List<CBascPetites> SelectCBascPetites();
    }
    public interface ICarretillas
    {
        List<RsCarretilles> SelectRsCarretilles();
    }
    public interface ICompresor
    {
        List<RsCompresor> SelectRsCompresor();
    }
    public interface IController
    {
        List<int> GetNumerosEnContenido(int id);
    }
    public interface IEstadoInstalacion
    {
        List<REstatInstalacio> SelectREstatInstalacio();
    }
    public interface IFestivo
    {
        List<DateTime> GetFestivos();
    }
    public interface IGrupoTareas
    {
        List<GrupTasques> GetAllGrups();
        string GetGroupById(int id);
        List<string> GetGroupsByPlaId(int plaId);
        List<GrupTasques> GetSelectedGrupsHoy();
        List<GrupTasques> GetSelectedGrupsEsporadicas();
        string ObtenerNombreGrupoPorId(int id);
        List<GrupTasques> GetLabGroups();
    }
    public interface IIncidencias
    {
        List<Incidencia> GetIncidenciasByIdGrupTasques(int idGrupTasques);
        bool HasUnresolvedIncidencias(int idGrupTasques);
        Task AddIncidencia(Incidencia incidencia);
        void UpdateIncidencia(Incidencia incidencia);
        Incidencia GetIncidenciaById(int idIncidencia);
        int GetNumeroIncidenciasNoResueltas();
        List<Incidencia> GetIncidenciasByIdTascaAndDateRange(int idTasca, DateTime startDate, DateTime endDate);
    }
    public interface IPND
    {
        List<PndTasques> SelectPndTasques(int idGrup);
    }
    public interface IPlan
    {
        List<Pla> GetAllPlans();
        List<string> GetAllPlanNames();
    }
    public interface IRegistroGeneralMantenimiento
    {
        List<RGeneralManteniment> GetRGeneralMantenimentByGrup();
    }
    public interface ITareas
    {
        List<Tasques> GetTasks();
        void UpdateTasquesDarrera(int tascaId, DateTime nuevaFechaDarrera);
        Tasques GetTaskById(int taskId);
        List<Tasques> GetTasksByGroupId(int idGroup);
        List<Tasques> GetTasquesLab();

    }
    public interface ITareasMantenimiento
    {
        List<TManteniment> GetAllTManteniment();
        int GetIdInternByNombreTasca(string nomTasca);

    }
    public interface IUsuario
    {
        string GetPasswordHashByEmail(string email);
        User GetUserByEmail(string email);
        bool UpdateUserPassword(int userId, string newPassword);
        int getActualLevel(int usuario_id);
    }
    public interface IVerificacionRegistros
    {
        List<VRegistres> GetVRegistresByGrup();
    }
    public interface IReprocessatMostresLaboratori
    {
        void AddReprocesatMostraLaboratori(RegistreReprocesatMostresLaboratori registre);
        List<RegistreReprocesatMostresLaboratori> getRegistresReprocessatMostraLaboratoris();
        List<RegistreReprocesatMostresLaboratori> getRegistresReprocessatMostraLaboratorisByDates(DateTime d1, DateTime d2);
    }
    public interface IRegistroLimpiezaProduccionRepository
    {
        void InsertRegistro(RegistroLimpiezaProduccion registro);
        void UpdateRegistro(RegistroLimpiezaProduccion registro);
        void DeleteRegistro(int id);
        RegistroLimpiezaProduccion GetRegistroById(int id);
        List<RegistroLimpiezaProduccion> GetAllRegistros();
    }
    public interface IControlRegistroLimpiezaProduccionRepository
    {
        void InsertControl(ControlRegistroLimpiezaProduccion control);
        void UpdateControl(ControlRegistroLimpiezaProduccion control);
        void DeleteControl(int id);
        ControlRegistroLimpiezaProduccion GetControlById(int id);
        List<ControlRegistroLimpiezaProduccion> GetAllControles();
        List<ControlRegistroLimpiezaProduccion> GetControlesByRegistroLimpiezaId(int idRegistroLimpiezaProduccion);
        void UpdateFinalizadaEstado(int idControl, bool finalizada);
    }
    public interface IRegistreHAET
    {
        bool Insert(RegistreHAET haet);
        bool Update(RegistreHAET haet);
        bool Delete(int id);
        List<RegistreHAET> GetAll();
        RegistreHAET GetById(int id);
    }
    public interface ITipoLimpiezaProduccion
    {
        List<string> GetTipos();
    }
    public interface IRegistreFaeMobil
    {
        bool Insert(RegistreFaseMovil haet);
        bool Update(RegistreFaseMovil haet);
        bool Delete(int id);
        List<RegistreFaseMovil> GetAll();
        RegistreFaseMovil GetById(int id);
    }
    public interface ITasksRecurrent 
    {
        List<Tasks> GetAllTareas();
        Tasks GetTareaById(int id);
        void AddTask(Tasks newTask);
        void DeleteTask(int id);
        void UpdateTask(Tasks updatedTask);
        List<Tasks> GetTareasByFecha(DateTime fecha);
        void UpdateCompletadaEstado(int taskId, bool completada);
        List<string> GetActiveEmployees();
        void EmpezarTarea(Tasks task, string emp);
        void FinalizarTarea(Tasks task);
        void AbordarTarea(Tasks task);
        void EstablecerTareaComoNoRealizada(Tasks task);
        bool CheckEmpleadoExisteEnGesAnextia(string emp);
        bool CheckEmpleadoExisteEnBBDD(string emp);
        void AsignarRegistroATarea(int idRegistro, int idTarea);
    }

    public interface ITasksRecurrentLab
    {
        List<Tasks> GetAllTareas();
        Tasks GetTareaById(int id);
        void AddTask(Tasks newTask);
        void DeleteTask(int id);
        void UpdateTask(Tasks updatedTask);
        List<Tasks> GetTareasByFecha(DateTime fecha);
        void UpdateCompletadaEstado(int taskId, bool completada);
        List<string> GetActiveEmployees();
        DateTime GetTareaMasReciente();
        List<Tasks> GetAllTasksByDateTime();
    }

    public interface ITaskParticipationRepository
    {
        List<TaskParticipationView> GetByTaskId(int taskId);
        bool HasOpenParticipation(int taskId, int employeeId);
        TaskParticipation Start(int taskId, int employeeId, string observaciones, DateTime? inicio = null);
        void EndOpenByTaskAndEmployee(int taskId, int employeeId, DateTime? fin = null, string observacionesAppend = null);
        void EndAllOpenByTask(int taskId, DateTime? fin = null);
    }
    public interface IPdfRepository
    {
        int InsertPdf(byte[] fileData);
        byte[] GetPdfById(int id);
        void UpdatePdf(int id, byte[] fileData);
    }

}
