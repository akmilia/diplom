using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace diplom.Models;

[Table("types_subjects")]
public partial class types_subjects
{  

    [Key]
    [Column(Order = 0)]
    public int types_id { get; set; }

    [Key]
    [Column(Order = 1)]
    public int subjects_idsubjects { get; set; }

}
