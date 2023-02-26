using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopM4_DataMigrations.Data;
using ShopM4_DataMigrations.Repository.IReporitory;
using ShopM4_Models;
using ShopM4_Models.ViewModels;
using ShopM4_Utility;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace ShopM4.Controllers
{
    [Authorize(Roles = PathManager.AdminRole)]
    public class ProductController : Controller
    {
        //private ApplicationDbContext db;
        private IRepositoryProduct repositoryProduct;
        private IWebHostEnvironment webHostEnvironment;
        public ProductController(IRepositoryProduct repositoryProduct, IWebHostEnvironment webHostEnvironment)
        {
            this.repositoryProduct = repositoryProduct;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> objList = repositoryProduct.GetAll();
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
                CategoriesList = repositoryProduct.GetListItems(PathManager.NameCategory),
                MyModelList = repositoryProduct.GetListItems(PathManager.NameMyModel),
            };
            //Product p = new Product();
            if (id == null)
            {
                return View(productViewModel);
            }
            else
            {
                productViewModel.Product = repositoryProduct.Find(id.Value);
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
                repositoryProduct.Add(productViewModel.Product);
            }
            else
            {
                var product = repositoryProduct.FirstOrDefault(
                    x => x.Id == productViewModel.Product.Id, isTracking: false);
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
                repositoryProduct.Update(productViewModel.Product);
            }
            repositoryProduct.Save();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var product = repositoryProduct.FirstOrDefault(x => x.Id == id,
                includeProperties: PathManager.NameCategory + "," + PathManager.NameMyModel);
            if (product == null)
            {
                return NotFound();
            }
            //product.Category = db.Category.Find(product.CategoryId);
            //
            //product.MyModel = db.MyModel.Find(product.MyModelId);

            return View(product);
        }
        [HttpPost]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product product = repositoryProduct.Find(id.Value);

            string upload = webHostEnvironment.WebRootPath + PathManager.ImageProductPath;

            var oldFile = upload + product.Image;

            if (System.IO.File.Exists(oldFile))
                System.IO.File.Delete(oldFile);

            repositoryProduct.Remove(product);
            repositoryProduct.Save();

            return RedirectToAction("Index");
        }
    }
}
