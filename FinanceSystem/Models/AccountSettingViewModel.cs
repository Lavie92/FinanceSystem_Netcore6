using FinanceSystem.Areas.Identity.Pages.Account;
using FinanceSystem.Areas.Identity.Pages.Account.Manage;

namespace FinanceSystem.Models
{
    public class AccountSettingViewModel
    {
        public UserInfor UserInfor { get; set; }
        public ChangePasswordModel ChangePasswordModel { get; set; }
    }
}
