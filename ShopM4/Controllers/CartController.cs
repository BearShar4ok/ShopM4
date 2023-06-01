using ShopM4_Models;
using ShopM4_Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShopM4_Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;
using ShopM4_DataMigrations.Repository.IReporitory;
using ShopM4_Utility.BrainTree;
using Braintree;

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
        IRepositoryOrderHeader repositoryOrderHeader;
        IRepositoryOrderDetail repositoryOrderDetail;
        IBrainTreeBridge brainTreeBridge;

        public CartController(IRepositoryProduct repositoryProduct,
            IRepositoryApplicationUser repositoryApplicationUser,
            IRepositoryQueryDetail repositoryQueryDetail,
            IRepositoryQueryHeader repositoryQueryHeader,
            IWebHostEnvironment webHostEnvironment, IEmailSender emailSender,
            IRepositoryOrderHeader repositoryOrderHeader,
            IRepositoryOrderDetail repositoryOrderDetail,
            IBrainTreeBridge brainTreeBridge)
        {
            this.repositoryQueryDetail = repositoryQueryDetail;
            this.repositoryQueryHeader = repositoryQueryHeader;
            this.repositoryApplicationUser = repositoryApplicationUser;
            this.repositoryProduct = repositoryProduct;
            this.webHostEnvironment = webHostEnvironment;
            this.emailSender = emailSender;
            this.repositoryOrderDetail = repositoryOrderDetail;
            this.repositoryOrderHeader = repositoryOrderHeader;
            this.brainTreeBridge = brainTreeBridge;
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
                includeProperties: PathManager.NameCategory);

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
        public IActionResult InquiryConformation(OrderHeader oh)
        {
            HttpContext.Session.Clear();
            return View(oh);
        }
        [HttpPost]
        public async Task<IActionResult> SummaryPost(IFormCollection collection,
            ProductUserViewModel productUserViewModel)
        {
            var identityClaim = (ClaimsIdentity)User.Identity;

            var claim = identityClaim.FindFirst(ClaimTypes.NameIdentifier);
            var ss = repositoryApplicationUser.FirstOrDefault(x => x.Id == claim.Value);


            if (User.IsInRole(PathManager.AdminRole))
            {
                OrderHeader orderHeader = new OrderHeader()
                {
                    AdminId = claim.Value,
                    DateOrder = DateTime.Now,
                    TotalPrice = productUserViewModel.ProductList.Sum(x => x.Price * x.TempCount),
                    Status = PathManager.StatusPending,
                    FullName = productUserViewModel.ApplicationUser.FullName,
                    Email = productUserViewModel.ApplicationUser.Email,
                    Phone = productUserViewModel.ApplicationUser.PhoneNumber,
                    City = productUserViewModel.ApplicationUser.City,
                    Street = productUserViewModel.ApplicationUser.Street,
                    House = productUserViewModel.ApplicationUser.House,
                    Apartament = productUserViewModel.ApplicationUser.Apartament,
                    PostalCode = productUserViewModel.ApplicationUser.PostalCode,
                };
                string nonce = collection["payment_method_nonce"];
                var request = new TransactionRequest()
                {
                    Amount = 1,
                    PaymentMethodNonce = nonce,
                    OrderId = "1",
                    Options = new TransactionOptionsRequest() { SubmitForSettlement = true },

                };
                var gateWay = brainTreeBridge.GetGateWay();
                var resultTrunsaction = gateWay.Transaction.Sale(request);
                var id = resultTrunsaction.Target.Id;
                var status = resultTrunsaction.Target.ProcessorResponseText;

                orderHeader.TransactionId = id;
                repositoryOrderHeader.Add(orderHeader);
                repositoryOrderHeader.Save();


                foreach (var product in productUserViewModel.ProductList)
                {
                    OrderDetails orderdetail = new OrderDetails()
                    {
                        OrderHeaderId = orderHeader.Id,
                        ProductId = product.Id,
                        Count = product.TempCount,
                        PricePurUnit = (int)product.Price
                    };
                    repositoryOrderDetail.Add(orderdetail);
                }
                repositoryOrderDetail.Save();

                return RedirectToAction("InquiryConformation", orderHeader);
            }
            else
            {
                var path = webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString() +
                "templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";
                var subject = "New query";
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
                        ProductId = product.Id,
                        QueryHeader = queryHeader,
                        QueryHeaderId = queryHeader.Id,
                        Product = repositoryProduct.Find(product.Id),
                    };
                    repositoryQueryDetail.Add(queryDetail);
                }


                repositoryQueryHeader.Save();

            }



            return RedirectToAction("InquiryConformation");
        }

        [HttpPost]
        public IActionResult Summary()
        {
            ApplicationUser applicationUser;
            if (User.IsInRole(PathManager.AdminRole))
            {
                if (HttpContext.Session.Get<int>(PathManager.SessionQuery) != 0)
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
                var gateWay = brainTreeBridge.GetGateWay();
                var tokenClient = gateWay.ClientToken.Generate();
                ViewBag.TokenClient = tokenClient;
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;

                // если пользователь вошел в систему, то объект будет определен
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                applicationUser = repositoryApplicationUser.FirstOrDefault(
                    x => x.Id == claim.Value);
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
        public IActionResult Update(Product product)
        {
            if (product != null && (product.TempCount < 1 || product.TempCount > 100))
            {
                product.TempCount = 1;
            }
            List<Cart> cartList = new List<Cart>();
            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);
            }
            for (int i = 0; i < cartList.Count(); i++)
            {
                if (cartList[i].ProductId == product.Id)
                {
                    cartList[i].Count = product.TempCount;
                }
            }
            HttpContext.Session.Set(PathManager.SessionCart, cartList);
            return RedirectToAction("Index");
        }
        public IActionResult Clear()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
