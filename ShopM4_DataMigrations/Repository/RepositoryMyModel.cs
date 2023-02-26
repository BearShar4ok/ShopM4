using ShopM4_DataMigrations.Data;
using ShopM4_DataMigrations.Repository.IReporitory;
using ShopM4_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopM4_DataMigrations.Repository
{
    public class RepositoryMyModel : Repository<ShopM4_Models.MyModel>, IRepositoryMyModel
    {
        public RepositoryMyModel(ApplicationDbContext db) : base(db)
        {

        }

        public void Update(ShopM4_Models.MyModel obj)
        {
            var objFromDB = db.MyModel.FirstOrDefault(x => x.Id == obj.Id);
            if (objFromDB != null)
            {
                objFromDB.Name = obj.Name;
                objFromDB.Number = obj.Number;
            }
        }
    }
}
