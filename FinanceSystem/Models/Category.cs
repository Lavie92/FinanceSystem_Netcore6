using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceSystem.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength =3)]
        public string Name { get; set; }
        [MaxLength(100)]
        [Required]
        public string? Image { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }

    }
}
