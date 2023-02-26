using ShopM4_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopM4_DataMigrations.Repository.IReporitory
{
    public interface IRepositoryCategory : IRepository<Category> 
    {
        void Update(Category obj);
    }
}
