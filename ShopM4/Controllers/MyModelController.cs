using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopM4.Data;
using ShopM4.Models;
using System.Data;
using System.Diagnostics;

namespace ShopM4.Controllers
{
    [Authorize(Roles = PathManager.AdminRole)]
    public class MyModelController : Controller
    {
        private ApplicationDbContext db;
        public MyModelController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<MyModel> categories = db.MyModel;
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
            db.MyModel.Add(category);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var myModel = db.MyModel.Find(id);

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
                db.MyModel.Update(myModel);
                db.SaveChanges();

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

            var myModel = db.MyModel.Find(id);

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

            var myModel = db.MyModel.Find(id);

            if (myModel == null)
            {
                return NotFound();
            }
            db.MyModel.Remove(myModel);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
