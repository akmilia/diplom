using System.ComponentModel.DataAnnotations;

namespace diplom.Models
{
    public partial class subjectsshow
    {
        [Key]
        public int subject_id { get; set; }
        public string subject_name { get; set; }
        public string description { get; set; }
        public int type_id { get; set; }
        public string type_name { get; set; }
    }
}
