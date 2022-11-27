using Microsoft.AspNetCore.Mvc;
using ShopM4.Data;
using ShopM4.Models;
using System.Diagnostics;

namespace ShopM4.Controllers
{
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
    }
}
