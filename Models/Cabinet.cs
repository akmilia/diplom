namespace diplom.Models;

public partial class Cabinet
{
    public int Idcabinet { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
