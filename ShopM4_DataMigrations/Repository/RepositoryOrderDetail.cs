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
    public class RepositoryOrderDetail : Repository<OrderDetails>, IRepositoryOrderDetail
    {
        public RepositoryOrderDetail(ApplicationDbContext db) : base(db)
        {
        }
        public void Update(OrderDetails obj)
        {
            db.OrderDetails.Update(obj);
        }


    }
}
