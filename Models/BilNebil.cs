namespace diplom.Models;

public partial class BilNebil
{
    public int Idattendance { get; set; }

    public int Iduser { get; set; }

    public bool? Status { get; set; }

    public virtual Attendance IdattendanceNavigation { get; set; } = null!;
}
