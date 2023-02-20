using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JakeDrinkStore.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Tag Name")]
        public string Name { get; set; }

        [DisplayName("Created Date")]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        public List<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
    }
}
