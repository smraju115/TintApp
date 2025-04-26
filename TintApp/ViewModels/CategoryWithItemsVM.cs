using TintApp.Models;

namespace TintApp.ViewModels
{
    public class CategoryWithItemsVM
    {
        public ServiceCategory Category { get; set; }
        public List<ServiceItem> Items { get; set; }
    }
}
