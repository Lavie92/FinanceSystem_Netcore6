using FinanceSystem.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinanceSystem.Models
{
	public class Plan
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[StringLength(450)]
		public string? Name { get; set; }
		public string? UserId { get; set; }
		public DateTime PlanDateStart  { get; set; }
        public DateTime PlanDateEnd { get; set; }
		public decimal Amount { get; set; }
		public virtual ICollection<Transaction>? Transactions { get; set; }
		public virtual FinanceSystemUser? User { get; set; }
	}
}
