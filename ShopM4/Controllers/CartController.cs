using ShopM4_DataMigrations.Data;
using ShopM4_Models;
using ShopM4_Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShopM4_Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ShopM4.Controllers
{

    [Authorize]
    public class CartController : Controller
    {
        ApplicationDbContext db;

        ProductUserViewModel productUserViewModel;
        IWebHostEnvironment webHostEnvironment;
        IEmailSender emailSender;

        public CartController(ApplicationDbContext db, 
            IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            this.db = db;
            this.webHostEnvironment = webHostEnvironment;
            this.emailSender = emailSender;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            List<Cart> cartList = new List<Cart>();
            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);
                // хотим получить каждый товар из корзины
            }
            // получаем лист id товаров
            List<int> productsIdInCart = cartList.Select(x => x.ProductId).ToList();
            // извлекаем сами продукты по списку id
            IEnumerable<Product> productList = db.Product.Where(x => productsIdInCart.Contains(x.Id));
            return View(productList);
        }
        public IActionResult Remove(int id)
        {
            // удаление из корзины
            List<Cart> cartList = new List<Cart>();

            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);
            }

            cartList.Remove(cartList.FirstOrDefault(x => x.ProductId == id));

            // переназначение сессии
            HttpContext.Session.Set(PathManager.SessionCart, cartList);

            return RedirectToAction("Index");
        }
        public IActionResult InquiryConformation()
        {
            HttpContext.Session.Clear();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult >SummaryPost(ProductUserViewModel productUserViewModel)
        {
            var path = webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString() +
                 "templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";
            var subject = "New order";
            string bodyHtml = "";
            using(StreamReader r = new StreamReader(path))
            {
                bodyHtml = r.ReadToEnd();
            }

            string textProducts = "";
            foreach (var item in productUserViewModel.ProductList)
            {
                textProducts += $"-Name: {item.Name}, ID: {item.Id}\n";
            }

           string body= string.Format(bodyHtml, productUserViewModel.ApplicationUser.FullNama,
                productUserViewModel.ApplicationUser.Email,
                productUserViewModel.ApplicationUser.PhoneNumber, textProducts);

            await emailSender.SendEmailAsync(productUserViewModel.ApplicationUser.Email, subject, body);
            //await emailSender.SendEmailAsync(productUserViewModel.ApplicationUser.Email, subject, body);

            return RedirectToAction("InquiryConformation");
        }
        [HttpPost]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            // если пользователь вошел в систему, то объект будет определен
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<Cart> cartList = new List<Cart>();

            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);
            }

            // получаем лист id товаров
            List<int> productsIdInCart = cartList.Select(x => x.ProductId).ToList();

            // извлекаем сами продукты по списку id
            IEnumerable<Product> productList = db.Product.Where(x => productsIdInCart.Contains(x.Id));


            productUserViewModel = new ProductUserViewModel()
            {
                ApplicationUser = db.ApplicationUser.FirstOrDefault(x => x.Id == claim.Value),
                ProductList = productList.ToList()
            };

            return View(productUserViewModel);
        }
    }
}
