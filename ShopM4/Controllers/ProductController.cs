using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopM4.Data;
using ShopM4.Models;
using ShopM4.Models.ViewModels;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

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
                MyModelList = db.MyModel.Select(x => new SelectListItem
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
            if (productViewModel.Product.Id == 0)
            {
                string upload = wwRoot + PathManager.ImageProductPath;
                string imageName = Guid.NewGuid().ToString();
                string ext = Path.GetExtension(files[0].FileName);
                string path = upload + imageName + ext;
                using (var filestream = new FileStream(path, FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                productViewModel.Product.Image = imageName + ext;
                db.Add(productViewModel.Product);
            }
            else
            {
                var product = db.Product.AsNoTracking().FirstOrDefault(
                    x => x.Id == productViewModel.Product.Id);
                if (files.Count > 0)
                {
                    string upload = wwRoot + PathManager.ImageProductPath;
                    string imageName = Guid.NewGuid().ToString();
                    string ext = Path.GetExtension(files[0].FileName);

                    var oldFile = upload + product.Image;

                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }
                    string path = upload + imageName + ext;
                    using (var filestream = new FileStream(path, FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    productViewModel.Product.Image = imageName + ext;

                }
                else
                {
                    productViewModel.Product.Image = product.Image;
                }
                db.Update(productViewModel.Product);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var product = db.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            product.Category = db.Category.Find(product.CategoryId);
            product.MyModel = db.MyModel.Find(product.MyModelId);

            return View(product);
        }
        [HttpPost]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var product = db.Product.Find(id);
            var files = HttpContext.Request.Form.Files;
            string wwRoot = webHostEnvironment.WebRootPath;
            string upload = wwRoot + PathManager.ImageProductPath;

            var oldFile = upload + product.Image;

            if (System.IO.File.Exists(oldFile))
                System.IO.File.Delete(oldFile);

            db.Product.Remove(product);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
