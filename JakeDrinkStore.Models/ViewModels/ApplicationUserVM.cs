using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JakeDrinkStore.Models.ViewModels
{
    public class ApplicationUserVM
    {
        public string Id { get; set; }

        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        public string? StreetAddress { get; set; }

        public string? Suburb { get; set; }

        public string? State { get; set; }

        public string? Postcode { get; set; }

        public string? PhoneNumber { get; set;}
    }
}
