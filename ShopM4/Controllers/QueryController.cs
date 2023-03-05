using Microsoft.AspNetCore.Mvc;
using ShopM4_DataMigrations.Repository.IReporitory;
using ShopM4_Models;
using ShopM4_Models.ViewModels;

namespace ShopM4.Controllers
{
    public class QueryController : Controller
    {
        private IRepositoryQueryHeader repositoryQueryHeader;
        private IRepositoryQueryDetail repositoryQueryDetail;
        [BindProperty]
        public QueryViewModel QueryViewModel { get; set; }
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
        public IActionResult Details(int id)
        {
            QueryViewModel = new QueryViewModel()
            {
                QueryHeader = repositoryQueryHeader.FirstOrDefault(x => x.Id == id),
                QueryDetail = repositoryQueryDetail.GetAll(x => x.QueryHeaderId == id,
                includeProperties: "Product")
            };
            return View(QueryViewModel);
        }
        public IActionResult GetQueryList()
        {
            var result = Json(new { data = repositoryQueryHeader.GetAll() });

            return result;
        }
    }
}
