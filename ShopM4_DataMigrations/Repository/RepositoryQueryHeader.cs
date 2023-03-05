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
    public class RepositoryQueryHeader : Repository<QueryHeader>, IRepositoryQueryHeader
    {
        public RepositoryQueryHeader(ApplicationDbContext db) : base(db)
        {
        }

        public void Update(QueryHeader obj)
        {

            db.QueryHeader.Update(obj);
            //var objFromDB = db.QueryHeader.FirstOrDefault(x => x.Id == obj.Id);
            //if (objFromDB != null)
            //{
            //    objFromDB.Email = obj.Email;
            //    objFromDB.FullName = obj.FullName;
            //    objFromDB.QueryDate = obj.QueryDate;
            //    objFromDB.PhoneNumber = obj.PhoneNumber;
            //    objFromDB.ApplicationUser = obj.ApplicationUser;
            //    objFromDB.ApplicationUserId = obj.ApplicationUserId;
            //}
        }
    }
}
