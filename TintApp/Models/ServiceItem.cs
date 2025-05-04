using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TintApp.Models
{
    public class ServiceItem
    {
        [Key]
        public int Id { get; set; }

        public int ServiceCategoryId { get; set; }

        [ForeignKey("ServiceCategoryId")]
        public ServiceCategory? Category { get; set; }

        [Required]
        [Display(Name = "Service Name")]
        public string ItemName { get; set; }

        public string PictureUrl { get; set; }=string.Empty;

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        public bool IsDeleted { get; set; } = false;
        public ICollection<ServiceImage>? Images { get; set; }

    }
}
