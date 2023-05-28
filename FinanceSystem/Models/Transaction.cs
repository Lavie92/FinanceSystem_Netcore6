using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceSystem.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }
        public int? WalletId { get; set; }
        public int? CategoryId { get; set; }
        public int? PlanId { get; set; }
        [Required]
        [Range(1000, int.MaxValue)]
        public decimal Amount { get; set; }
		[Required]
		public DateTime CreateDate { get; set; } 
        public string? Image { get; set; }
        public bool Income { get; set; }
        [StringLength(100, MinimumLength = 3)]
        public string? Note { get; set; }
        public virtual Category? Category { get; set; }
        public virtual Wallet? Wallet { get; set; }
        public virtual Plan? Plan { get; set; }

		public IFormFile? ImageFile;

	}
}
