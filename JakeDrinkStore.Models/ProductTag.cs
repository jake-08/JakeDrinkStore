using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JakeDrinkStore.Models
{
    public class ProductTag
    {
        [Key]
        public int Id { get; set; }
        public int TagId { get; set; }
        public Tag? Tag { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
