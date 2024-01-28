using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlobarShopp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public string Adress { get; set; }
        public string Sehir { get; set; }
        public string Semt { get; set; }
        public string PosaKodu { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}