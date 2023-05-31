using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopM4_Models
{
    public class ProductCard
    {
        public Product Product { get; set; }
        public bool IsItImportantForAdmin { get; set; }
        public bool ShowInCart { get; set; }
    }
}
