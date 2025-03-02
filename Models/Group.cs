namespace diplom.Models;

public partial class Group
{
    public int Idgroups { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
