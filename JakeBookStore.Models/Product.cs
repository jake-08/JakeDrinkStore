using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JakeDrinkStore.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "List Price")]
        public double ListPrice { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Case Price")]
        public double CasePrize { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Quantity Per Case")]
        public int QuantityPerCase { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Bulk Case Price")]
        public double BulkCasePrice { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Mininum Case for Bulk Price")]
        public int MinBulkCase { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; }

        [ValidateNever]
        public List<ProductTag> ProductTags { get; set; } = new List<ProductTag>();

        [Required]
        [Display(Name = "Drink Type")]
        public Guid? DrinkTypeId { get; set; }

        [ValidateNever]
        [ForeignKey("DrinkTypeId")]
        public DrinkType? DrinkType { get; set; }
    }
}
