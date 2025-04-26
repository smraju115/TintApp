using System.ComponentModel.DataAnnotations;

namespace TintApp.Models
{
    public class ServiceCategory
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }

        [Required]
        public string Title { get; set; }

        public string? ImageUrl { get; set; } = string.Empty;

        public ICollection<ServiceItem>? Items { get; set; }
    }
}
