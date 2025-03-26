using System.ComponentModel.DataAnnotations;

namespace diplom.Models
{
    public partial class scheduleshow
    {
        [Key]
        public int idschedule { get; set; }
        public TimeSpan time { get; set; }
        public string subject_name { get; set; }
        public string teacher { get; set; }
        public int cabinet { get; set; }
        public string group_nam { get; set; }
        public string day_of_week { get; set; } 

    }
}
