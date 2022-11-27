using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopM4.Data;
using ShopM4.Models;
using ShopM4.Models.ViewModels;

namespace ShopM4.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db;
        public ProductController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> objList = db.Product;
            //links to categories
            foreach (var item in objList)
            {
                item.Category = db.Category.FirstOrDefault(x => x.Id == item.CategoryId);
            }
            return View(objList);
        }

        public IActionResult CreateEdit(int? id)
        {
            //IEnumerable<SelectListItem> CategoriesList = db.Category.Select(x =>
            //new SelectListItem
            //{
            //    Text = x.Name,
            //    Value = x.Id.ToString(),
            //});
            //
            ////ViewBag.CategoryList = CategoriesList;
            //ViewData["CategoryList"] = CategoriesList;
            ProductViewModel productViewModel = new ProductViewModel()
            {
                Product = new Product(),
                CategoriesList = db.Category.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                }),
            };
            //Product p = new Product();
            if (id == null)
            {
                return View(productViewModel);
            }
            else
            {
                productViewModel.Product = db.Product.Find(id);
                if (productViewModel.Product == null)
                {
                    return NotFound();
                }
                return View(productViewModel);
            }

        }
    }
}
