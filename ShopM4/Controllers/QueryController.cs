using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopM4_DataMigrations.Repository.IReporitory;
using ShopM4_Models;
using ShopM4_Models.ViewModels;
using ShopM4_Utility;

namespace ShopM4.Controllers
{
    [Authorize(Roles =PathManager.AdminRole)]
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
        [HttpPost]
        public IActionResult Details()
        {
            List<Cart> carts = new List<Cart>();

            QueryViewModel.QueryDetail = repositoryQueryDetail.GetAll(
                x => x.QueryHeaderId == QueryViewModel.QueryHeader.Id);

            foreach (var item in QueryViewModel.QueryDetail)
            {
                carts.Add(new Cart()
                {
                    ProductId=item.ProductId
                });
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(PathManager.SessionCart,carts);
            HttpContext.Session.Set(PathManager.SessionQuery,QueryViewModel.QueryHeader.Id);
            
            
            return RedirectToAction("Index","Cart");
        }
        public IActionResult GetQueryList()
        {
            var result = Json(new { data = repositoryQueryHeader.GetAll() });

            return result;
        }
        [HttpPost]
        public IActionResult Delete()
        {
            QueryHeader queryHeader = repositoryQueryHeader.FirstOrDefault(
                x => x.Id == QueryViewModel.QueryHeader.Id);

            IEnumerable<QueryDetail> queryDetails = repositoryQueryDetail.GetAll(
                x=>x.QueryHeaderId==QueryViewModel.QueryHeader.Id);

            repositoryQueryDetail.Remove(queryDetails);
            repositoryQueryHeader.Remove(queryHeader);

            repositoryQueryHeader.Save();

            return RedirectToAction("Index");
        }
    }
}
