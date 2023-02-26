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
    public class RepositoryQueryDeatail : Repository<QueryDetail>, IRepositoryQueryDetail
    {
        public RepositoryQueryDeatail(ApplicationDbContext db) : base(db)
        {
        }

        void IRepositoryQueryDetail.Update(QueryDetail obj)
        {
            var objFromDB = db.QueryDetail.FirstOrDefault(x => x.Id == obj.Id);
            if (objFromDB != null)
            {
                objFromDB.Product = obj.Product;
                objFromDB.ProductId = obj.ProductId;
                objFromDB.QueryHeader = obj.QueryHeader;
                objFromDB.QueryHeaderId = obj.QueryHeaderId;
            }
        }
    }
}
