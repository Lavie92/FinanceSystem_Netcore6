using FinanceSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FinanceSystem.Models
{
    public class UserInfor
    {
        [StringLength(450)]
        [Key] 
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Gender { get; set; }
        public DateTime Birthdate { get; set; }
        public string? Image { get; set; }
        public bool Vozer { get; set; }
        public virtual FinanceSystemUser? User { get; set; }

    }
}
