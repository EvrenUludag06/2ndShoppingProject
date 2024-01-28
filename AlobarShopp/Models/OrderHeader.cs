using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace AlobarShopp.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime OrderDate { get; set; }
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Adress { get; set; }
        [Required]
        public string Semt { get; set; }
        [Required]
        public string Sehir { get; set; }
        [Required]
        public string Postakodu { get; set; }
        [Required]
        public string CartName { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string ExpiriationMonth { get; set; }
        [Required]
        public string ExpiriationYear { get; set; }
        [Required]
        public string Cvc { get; set; }
    }
}
