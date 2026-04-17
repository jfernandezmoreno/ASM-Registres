using ASM_Registres_NET10.Constants;

namespace ASM_Registres_NET10.Modules
{
    
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int nivel { get; set; }
        public string Departament {  get; set; }
        public User() { }

        public bool IsAdmin()
        {
            return nivel == DatabaseCredentials.ADMIN_LVL;
        }

    }
}
