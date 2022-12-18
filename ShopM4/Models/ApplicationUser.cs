using Microsoft.AspNetCore.Identity;
namespace ShopM4.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullNama { get; set; }
    }
}
