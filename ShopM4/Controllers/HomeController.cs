using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopM4_DataMigrations.Data;
using ShopM4_DataMigrations.Repository.IReporitory;
using ShopM4_Models;
using ShopM4_Models.ViewModels;
using ShopM4_Utility;
using System.Diagnostics;

namespace ShopM4.Controllers
{
    public class HomeController : Controller

    {
        private readonly ILogger<HomeController> _logger;
        private IRepositoryProduct repositoryProduct;
        private IRepositoryCategory repositoryCategory;
        public HomeController(ILogger<HomeController> logger,
            IRepositoryProduct repositoryProduct, IRepositoryCategory repositoryCategory)
        {
            this.repositoryCategory = repositoryCategory;
            this.repositoryProduct = repositoryProduct;
            _logger = logger;
        }

        public IActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel()
            {

                Products = repositoryProduct.GetAll(
                    includeProperties: PathManager.NameCategory + "," + PathManager.NameMyModel),
                Categories = repositoryCategory.GetAll(),

            };
            return View(homeViewModel);
        }

        public IActionResult Details(int id)
        {
            List<Cart> cartList = new List<Cart>();
            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);
            }


            DetailsViewModel detailsViewModel = new DetailsViewModel()
            {
                Product = repositoryProduct.FirstOrDefault(x => x.Id == id,
                includeProperties: PathManager.NameCategory + "," + PathManager.NameMyModel),
                IsInCart = false,
            };

            foreach (var item in cartList)
            {
                if (item.ProductId == id)
                {
                    detailsViewModel.IsInCart = true;
                }
            }

            return View(detailsViewModel);
        }

        [HttpPost]
        public IActionResult DetailsPost(int id)
        {
            List<Cart> cartList = new List<Cart>();
            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);
            }
            cartList.Add(new Cart { ProductId = id });

            HttpContext.Session.Set(PathManager.SessionCart, cartList);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            List<Cart> cartList = new List<Cart>();
            if (HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<Cart>>(PathManager.SessionCart).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<Cart>>(PathManager.SessionCart);
            }
            cartList.Remove(cartList.Find(x => x.ProductId == id));

            HttpContext.Session.Set(PathManager.SessionCart, cartList);
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}