using System.ComponentModel.DataAnnotations;

namespace diplom.Models
{
    public partial class usersshow
    {
        [Key]
        public int idusers { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string full_name { get; set; }
        public int idroles { get; set; }
        public string user_role { get; set; }
    }
}
