﻿using ShopM4_DataMigrations.Data;
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
            IEnumerable<Product> productList = repositoryProduct.GetAll(
                x => productsIdInCart.Contains(x.Id),
                includeProperties: PathManager.NameCategory);
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
        public async Task<IActionResult> SummaryPost(ProductUserViewModel productUserViewModel)
        {
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
                ApplicationUserId = productUserViewModel.ApplicationUser.Id,
                QueryDate = DateTime.Now,
                PhoneNumber = productUserViewModel.ApplicationUser.PhoneNumber,
                FullName = productUserViewModel.ApplicationUser.FullName,
                Email = productUserViewModel.ApplicationUser.Email,
                ApplicationUser = productUserViewModel.ApplicationUser
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
            IEnumerable<Product> productList = repositoryProduct.GetAll(
                x => productsIdInCart.Contains(x.Id));


            productUserViewModel = new ProductUserViewModel()
            {
                ApplicationUser = repositoryApplicationUser.FirstOrDefault(x => x.Id == claim.Value),
                ProductList = productList.ToList()
            };

            return View(productUserViewModel);
        }
    }
}
