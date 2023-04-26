using FinanceSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceSystem.Models
{
    public class Wallet 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int Id { get; set; }
        [StringLength(450)]
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Transaction> Transactions  { get; set; }
        public virtual FinanceSystemUser User { get; set; }
    }
}
