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
        private IWebHostEnvironment webHostEnvironment;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            this.db = db;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> objList = db.Product;
            //links to categories
            foreach (var item in objList)
            {
                //item.Category = db.Category.FirstOrDefault(x => x.Id == item.CategoryId);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEdit(ProductViewModel productViewModel) 
        {
            var files = HttpContext.Request.Form.Files;
            string wwRoot = webHostEnvironment.WebRootPath;
            if(productViewModel.Product.Id == 0)
            {
                string upload = wwRoot + PathManager.ImageProductPath;
                string imageName = Guid.NewGuid().ToString();
                string ext = Path.GetExtension(files[0].FileName);
                string path = upload+imageName+ext;
                using( var filestream = new FileStream(path,FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                productViewModel.Product.Image = imageName + ext;
                db.Product.Add(productViewModel.Product);
                db.SaveChanges();
            }
            else
            {

            }
            return RedirectToAction("Index");
        }
    }
}
