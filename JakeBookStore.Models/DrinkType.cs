using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JakeDrinkStore.Models
{
    public class DrinkType
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [DisplayName("Drink Type")]
        public string Name { get; set; }

        [DisplayName("Created Date")]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
