using Microsoft.AspNetCore.Mvc;
using ShopM4_DataMigrations.Repository.IReporitory;

namespace ShopM4.Controllers
{
    public class QueryController : Controller
    {
        private IRepositoryQueryHeader repositoryQueryHeader;
        private IRepositoryQueryDetail repositoryQueryDetail;
        public QueryController(IRepositoryQueryHeader repositoryQueryHeader,
         IRepositoryQueryDetail repositoryQueryDetail)
        {
            this.repositoryQueryDetail = repositoryQueryDetail;
            this.repositoryQueryHeader = repositoryQueryHeader;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
