namespace ShopM4.Models.ViewModels
{
    public class DetailsViewModel
    {
        public Product Product { get; set; }
        public bool IsInCart { get; set; }

        public DetailsViewModel()
        {
            Product = new Product();
        }
    }
}
