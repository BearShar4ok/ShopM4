using Microsoft.AspNetCore.Mvc.Rendering;
using ShopM4_DataMigrations.Data;
using ShopM4_DataMigrations.Repository.IReporitory;
using ShopM4_Models;
using ShopM4_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopM4_DataMigrations.Repository
{
    public class RepositoryProduct : Repository<Product>, IRepositoryProduct
    {
        public RepositoryProduct(ApplicationDbContext db) : base(db) { }

        public void Update(Product obj)
        {
            //db.Product.Update(obj);
            var objFromDB = db.Product.FirstOrDefault(x => x.Id == obj.Id);
            if (objFromDB != null)
            {
                objFromDB.Name = obj.Name;
                objFromDB.Price = obj.Price;
                objFromDB.ShortDescription = obj.ShortDescription;
                objFromDB.Description = obj.Description;
                objFromDB.Category = obj.Category;
                objFromDB.CategoryId = obj.CategoryId;
                objFromDB.MyModel = obj.MyModel;
                objFromDB.MyModelId = obj.MyModelId;
                objFromDB.Image = obj.Image;
            }
        }

        IEnumerable<SelectListItem> IRepositoryProduct.GetListItems(string odj)
        {
            if (odj == PathManager.NameCategory)
            {
                return db.Category.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                });
            }
            if (odj == PathManager.NameMyModel)
            {
                return db.MyModel.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                });
            }
            return null;
        }
    }
}
