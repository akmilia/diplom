namespace diplom.Models;

public partial class Schedule
{
    public int Idschedule { get; set; }

    public DateTimeOffset Time { get; set; }

    public int SubjectsIdsubjects { get; set; }

    public int UsersIdusers { get; set; }

    public int CabinetsIdcabinet { get; set; }

    public int? GroupsIdgroup { get; set; }

    public int DayOfWeek { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Cabinet CabinetsIdcabinetNavigation { get; set; } = null!;

    public virtual Group? GroupsIdgroupNavigation { get; set; }

    public virtual Subject SubjectsIdsubjectsNavigation { get; set; } = null!;

    public virtual User UsersIdusersNavigation { get; set; } = null!;
}
