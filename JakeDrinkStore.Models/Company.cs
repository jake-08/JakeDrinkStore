using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JakeDrinkStore.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Company Name")]

        public string? Name { get; set; }

        [DisplayName("Street Address")]
        public string? StreetAddress { get; set; }

        public string? Suburb { get; set; }

        public string? State { get; set; }

        public string? Postcode { get; set; }

        [DisplayName("Phone Number")]
        public string? PhoneNumber { get; set; }
    }
}
