using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace diplom.Models;


public partial class TypesSubject
{
    [Key]
    [Column(Order = 0)]
    public int TypesId { get; set; }

    [Key]
    [Column(Order = 1)]
    public int SubjectsIdsubjects { get; set; }

    public virtual Subject SubjectsIdsubjectsNavigation { get; set; } = null!;

    public virtual Type Types { get; set; } = null!;
}
