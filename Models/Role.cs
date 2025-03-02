namespace diplom.Models;

public partial class Role
{
    public int Idroles { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
