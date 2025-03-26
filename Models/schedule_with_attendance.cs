using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace diplom.Models
{
    public partial class schedule_with_attendance
    {
        [Key]
        public int idattendance { get; set; }
        public int idschedule { get; set; }
        public TimeSpan time { get; set; }
        public string subject_name { get; set; }
        public string teacher { get; set; }
        public string cabinet { get; set; }
        public string group_name { get; set; }
        public string day_of_week { get; set; }
        public DateTime event_date { get; set; }
        public string formatted_date { get; set; }
        public int day_number { get; set; }
        public string month_name { get; set; }
        public int month_number { get; set; }
        public int year { get; set; }

        public string TimeFormatted => time.ToString(@"hh\:mm");
    }
}
