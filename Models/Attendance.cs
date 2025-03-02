namespace diplom.Models;

public partial class Attendance
{
    public int Idattendance { get; set; }

    public int Idschedule { get; set; }

    public DateOnly Date { get; set; }

    public virtual ICollection<BilNebil> BilNebils { get; set; } = new List<BilNebil>();

    public virtual Schedule IdscheduleNavigation { get; set; } = null!;
}
