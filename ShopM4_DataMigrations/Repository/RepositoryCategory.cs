using ShopM4_Models;
using System;
using ShopM4_DataMigrations.Repository.IReporitory;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopM4_DataMigrations.Data;

namespace ShopM4_DataMigrations.Repository
{
    public class RepositoryCategory :Repository<Category>, IRepositoryCategory
    {
        public RepositoryCategory(ApplicationDbContext db):base(db) {}

        public void Update(Category obj)
        {
            var objFromDB=db.Category.FirstOrDefault(x => x.Id == obj.Id);
            if (objFromDB!=null)
            {
                objFromDB.Name = obj.Name;
                objFromDB.DisplayOrder = obj.DisplayOrder;

            }
        }
    }
}
