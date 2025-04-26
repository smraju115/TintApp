using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TintApp.Models
{
    public class Booking
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Customer name is required")]
        [Display(Name = "Name")]


        public string CustomerName { get; set; }
        [Required]
        public string BookingNumber { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(); // Auto generate

        [Required(ErrorMessage = "Date Time is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [Display(Name = "Category")]
        public int ServiceCategoryId { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        [Display(Name = "Item")]
        public int ServiceItemId { get; set; }


        [Required, Column(TypeName ="Money")]
        public decimal? Price { get; set; }

        [ForeignKey("ServiceCategoryId")]
        public ServiceCategory? ServiceCategory { get; set; }

        [ForeignKey("ServiceItemId")]
        public ServiceItem? ServiceItem { get; set; }

        [Required(ErrorMessage = "Mobile is required")]
        [Phone]
        [Display(Name = "Mobile")]

        public string CustomerMobile { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]

        public string CustomerEmail { get; set; }

        [Required(ErrorMessage = "Card Model name is required")]
        [Display(Name = "Car Model")]

        public string CarModel { get; set; }

        [Required(ErrorMessage = "Car Number Plate is required")]
        [Display(Name = "Car Number Plaete No")]

        public string CarNumberPlate { get; set; }

        [Display(Name = "Message")]
        public string? CustomerMessage { get; set; }

        public string? PdfUrl { get; set; } // Path to generated PDF
        public string Status { get; set; } = string.Empty; // Pending / Completed


    }
}
