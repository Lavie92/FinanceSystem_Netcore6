using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace FinanceSystem.Areas.Identity.Data;

// Add profile data for application users by adding properties to the FinanceSystemUser class
public class FinanceSystemUser : IdentityUser
{    public UserInfor UserInfor { get; set; }
        public ICollection<Wallet> Wallets { get; set; }
}

