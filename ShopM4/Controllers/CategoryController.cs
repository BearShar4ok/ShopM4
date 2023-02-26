using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopM4_DataMigrations.Data;
using ShopM4_DataMigrations.Repository;
using ShopM4_DataMigrations.Repository.IReporitory;
using ShopM4_Models;
using ShopM4_Utility;
using System.Data;
using System.Data.SqlTypes;

namespace ShopM4.Controllers
{
    [Authorize(Roles = PathManager.AdminRole)]
    public class CategoryController : Controller
    {
        private IRepositoryCategory repositoryCategory;
        //private ApplicationDbContext db;
        public CategoryController(IRepositoryCategory repositoryCategory)
        {
            this.repositoryCategory = repositoryCategory;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = repositoryCategory.GetAll();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                repositoryCategory.Add(category);
                repositoryCategory.Save();

                return RedirectToAction("Index");
            }

            return View(category);
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = repositoryCategory.Find(id.GetValueOrDefault());

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                repositoryCategory.Update(category);
                repositoryCategory.Save();

                return RedirectToAction("Index");
            }

            return View(category);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = repositoryCategory.Find(id.GetValueOrDefault());

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = repositoryCategory.Find(id.GetValueOrDefault());

            if (category == null)
            {
                return NotFound();
            }
            repositoryCategory.Remove(category);
            repositoryCategory.Save();

            return RedirectToAction("Index");
        }
    }
}
