using FinanceSystem.Areas.Identity.Data;
using FinanceSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace FinanceSystem.Areas.Admin.Models
{
	public class UserViewModel
	{
		public FinanceSystemUser User { get; set; }
		public UserInfor UserInfor { get; set; }
		public List<Wallet> Wallets { get; set; }
		public FinanceSystemUser FirstUser { get; set; }
	}
}
