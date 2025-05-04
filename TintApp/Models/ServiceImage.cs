using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TintApp.Models
{
    public class ServiceImage
    {
        [Key]
        public int Id { get; set; }

        public int ServiceItemId { get; set; }

        [ForeignKey("ServiceItemId")]
        public ServiceItem? ServiceItem { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
    }
}
