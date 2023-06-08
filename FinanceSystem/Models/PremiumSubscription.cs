using FinanceSystem.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace FinanceSystem.Models
{
	public class PremiumSubscription
	{
		[Key]
		public string? Id { get; set; }
		public string? Name { get; set; }
		public decimal? Price { get; set; }
		public virtual FinanceSystemUser? User { get; set; }
	}
}
