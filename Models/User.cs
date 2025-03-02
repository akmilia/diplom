using System.ComponentModel.DataAnnotations;
namespace diplom.Models;

public partial class User
{
    [Key]
    public int Idusers { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Paternity { get; set; }

    public DateOnly? Birthdate { get; set; }

    public string Login { get; set; } = null!;

    public string? Password { get; set; }

    public int RolesIdroles { get; set; }

    public virtual Role RolesIdrolesNavigation { get; set; } = null!;

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
