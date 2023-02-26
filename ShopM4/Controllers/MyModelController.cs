using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopM4_DataMigrations.Data;
using ShopM4_DataMigrations.Repository.IReporitory;
using ShopM4_Models;
using ShopM4_Utility;
using System.Data;
using System.Diagnostics;

namespace ShopM4.Controllers
{
    [Authorize(Roles = PathManager.AdminRole)]
    public class MyModelController : Controller
    {
        //private ApplicationDbContext db;
        private IRepositoryMyModel repositoryMyModel;
        public MyModelController(IRepositoryMyModel repositoryMyModel)
        {
            this.repositoryMyModel = repositoryMyModel;
        }
        public IActionResult Index()
        {
            IEnumerable<MyModel> categories = repositoryMyModel.GetAll();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(MyModel category)
        {
            if (category == null || category.Name == null)
                return RedirectToAction("Index");
            repositoryMyModel.Add(category);
            repositoryMyModel.Save();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var myModel = repositoryMyModel.Find(id.Value);
           

            if (myModel == null)
            {
                return NotFound();
            }
            return View(myModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MyModel myModel)
        {
            if (ModelState.IsValid)
            {
                repositoryMyModel.Update(myModel);
                repositoryMyModel.Save();

                return RedirectToAction("Index");
            }

            return View(myModel);
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var myModel = repositoryMyModel.Find(id.Value);

            if (myModel == null)
            {
                return NotFound();
            }
            return View(myModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var myModel = repositoryMyModel.Find(id.Value);

            if (myModel == null)
            {
                return NotFound();
            }
            repositoryMyModel.Remove(myModel);
            repositoryMyModel.Save();

            return RedirectToAction("Index");
        }
    }
}
