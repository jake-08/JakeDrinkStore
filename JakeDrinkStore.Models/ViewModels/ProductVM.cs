using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace JakeDrinkStore.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }

        [Display(Name = "Tags")]
        public List<int> TagIds { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> TagList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> DrinkTypeList { get; set; }
    }
}
