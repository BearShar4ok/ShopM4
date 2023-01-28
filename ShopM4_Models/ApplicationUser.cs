using Microsoft.AspNetCore.Identity;
namespace ShopM4_Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullNama { get; set; }
    }
}
