using ShopM4_DataMigrations.Data;
using ShopM4_Models;
using ShopM4_Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShopM4_Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ShopM4_DataMigrations.Repository.IReporitory;

namespace ShopM4.Controllers
{

    [Authorize]
    public class CartController : Controller
    {

        ProductUserViewModel productUserViewModel;
        IWebHostEnvironment webHostEnvironment;
        IEmailSender emailSender;
        IRepositoryProduct repositoryProduct;
        IRepositoryApplicationUser repositoryApplicationUser;
        IRepositoryQueryDetail repositoryQueryDetail;
        IRepositoryQueryHeader repositoryQueryHeader;

        public CartController(IRepositoryProduct repositoryProduct,
            IRepositoryApplicationUser repositoryApplicationUser,
            IRepositoryQueryDetail repositoryQueryDetail,
            IRepositoryQueryHeader repositoryQueryHeader,
            IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            this.repositoryQueryDetail = repositoryQueryDetail;
            this.repositoryQueryHeader = repositoryQueryHeader;
            this.repositoryApplicationUser = repositoryApplicationUser;
            this.repositoryProduct = repositoryProduct;
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
            IEnumerable<Product> productListTemp = repositoryProduct.GetAll(
                x => productsIdInCart.Contains(x.Id),
                includeProperties:PathManager.NameCategory);

            List<Product> productList = new List<Product>();
            foreach (var item in cartList)
            {
                Product p = productListTemp.FirstOrDefault(x => x.Id == item.ProductId);
                p.TempCount = item.Count;
                productList.Add(p);
            }

            return View(productList);
        }
        [HttpPost]
        [ActionName("Index")]
        public IActionResult IndexPost(List<Product> products)
        {
            List<Cart> carts = new List<Cart>();
            foreach (var item in products)
            {
                carts.Add(new Cart() { ProductId = item.Id, Count = item.TempCount });
            }
            HttpContext.Session.Set(PathManager.SessionCart, carts);
            return RedirectToAction("Summary");
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
        public async Task<IActionResult> SummaryPost(ProductUserViewModel productUserViewModel)
        {
            var identityClaim = (ClaimsIdentity)User.Identity;

            var claim = identityClaim.FindFirst(ClaimTypes.NameIdentifier);
            var ss = repositoryApplicationUser.FirstOrDefault(x => x.Id == claim.Value);

            var path = webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString() +
                 "templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";
            var subject = "New order";
            string bodyHtml = "";
            using (StreamReader r = new StreamReader(path))
            {
                bodyHtml = r.ReadToEnd();
            }

            string textProducts = "";
            foreach (var item in productUserViewModel.ProductList)
            {
                textProducts += $"-Name: {item.Name}, ID: {item.Id}\n";
            }

            string body = string.Format(bodyHtml, productUserViewModel.ApplicationUser.FullName,
                 productUserViewModel.ApplicationUser.Email,
                 productUserViewModel.ApplicationUser.PhoneNumber, textProducts);

            await emailSender.SendEmailAsync(productUserViewModel.ApplicationUser.Email, subject, body);



            QueryHeader queryHeader = new QueryHeader()
            {
                //ApplicationUserId = productUserViewModel.ApplicationUser.Id,
                ApplicationUserId = claim.Value,
                QueryDate = DateTime.Now,
                PhoneNumber = productUserViewModel.ApplicationUser.PhoneNumber,
                FullName = productUserViewModel.ApplicationUser.FullName,
                Email = productUserViewModel.ApplicationUser.Email,
                //ApplicationUser = productUserViewModel.ApplicationUser//repositoryApplicationUser.FirstOrDefault(x => x.Id == claim.Value)
                ApplicationUser = repositoryApplicationUser.FirstOrDefault(x => x.Id == claim.Value)
            };
            repositoryQueryHeader.Add(queryHeader);
            repositoryQueryHeader.Save();


            foreach (Product product in productUserViewModel.ProductList)
            {
                QueryDetail queryDetail = new QueryDetail()
                {
                    ProductId= product.Id,
                    QueryHeader= queryHeader,
                    QueryHeaderId= queryHeader.Id,
                    Product = repositoryProduct.Find(product.Id),
                };
                repositoryQueryDetail.Add(queryDetail);
            }
            
            
            repositoryQueryHeader.Save();


            return RedirectToAction("InquiryConformation");
        }
        [HttpPost]
        public IActionResult Summary()
        {
            ApplicationUser applicationUser;
            if (User.IsInRole(PathManager.AdminRole))
            {
                if (HttpContext.Session.Get<int>(PathManager.SessionQuery)!=0)
                {
                    QueryHeader queryheader = repositoryQueryHeader.FirstOrDefault(
                      x => x.Id == HttpContext.Session.Get<int>(PathManager.SessionQuery));
                    applicationUser = new ApplicationUser()
                    {
                        Email = queryheader.Email,
                        PhoneNumber = queryheader.PhoneNumber,
                        FullName = queryheader.FullName,
                    };
                }
                else
                {
                    applicationUser = new ApplicationUser();
                    
                }
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;

                // если пользователь вошел в систему, то объект будет определен
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                applicationUser = repositoryApplicationUser.FirstOrDefault(
                    x=> x.Id==claim.Value);
            }

            

            List<Cart> cartList = new List<Cart>();

            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);
            }

            // получаем лист id товаров
            List<int> productsIdInCart = cartList.Select(x => x.ProductId).ToList();

            // извлекаем сами продукты по списку id
            IEnumerable<Product> productList = repositoryProduct.GetAll(
                x => productsIdInCart.Contains(x.Id));


            productUserViewModel = new ProductUserViewModel()
            {
                ApplicationUser = applicationUser
            };
            foreach (var item in cartList)
            {
                Product product = repositoryProduct.FirstOrDefault(x => x.Id == item.ProductId);
                product.TempCount = item.Count;
                productUserViewModel.ProductList.Add(product);
            }

            return View(productUserViewModel);
        }
        [HttpPost]
        public IActionResult Update(IEnumerable<Product> products)
        {
            List<Cart> cartList = new List<Cart>();
            foreach (var item in products)
            {
                cartList.Add(new Cart()
                {
                    ProductId = item.Id,
                    Count = item.TempCount
                });
            }
            HttpContext.Session.Set(PathManager.SessionCart, cartList);
            return RedirectToAction("Index");
        }
    }
}
