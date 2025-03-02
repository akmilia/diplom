namespace diplom.Models;

public partial class GroupsUser
{
    public int GroupsIdgroups { get; set; }

    public int UsersIdusers { get; set; }

    public virtual Group GroupsIdgroupsNavigation { get; set; } = null!;

    public virtual User UsersIdusersNavigation { get; set; } = null!;
}
