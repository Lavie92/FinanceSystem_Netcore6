using FinanceSystem.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinanceSystem.Models
{
    public class TargetSaving
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(450)]
        public string? Name { get; set; }
        public DateTime SaveDateStart { get; set; }
        public DateTime SaveDateEnd { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public virtual ICollection<Transaction>? Transactions { get; set; }
        public virtual FinanceSystemUser? User { get; set; }

    }
}
