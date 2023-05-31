using Braintree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopM4_DataMigrations.Repository.IReporitory;
using ShopM4_Models;
using ShopM4_Models.ViewModels;
using ShopM4_Utility;
using ShopM4_Utility.BrainTree;

namespace ShopM4.Controllers
{
    public class OrderController : Controller
    {
        IRepositoryOrderHeader repositoryOrderHeader;
        IRepositoryOrderDetail repositoryOrderDetail;
        IBrainTreeBridge brainTreeBridge;

        [BindProperty]
        public OrderHeaderDetailViewModel OrderViewModel { get; set; }
        public OrderController(IRepositoryOrderHeader repositoryOrderHeader,
        IRepositoryOrderDetail repositoryOrderDetail,
        IBrainTreeBridge brainTreeBridge)
        {
            this.repositoryOrderDetail = repositoryOrderDetail;
            this.repositoryOrderHeader = repositoryOrderHeader;
            this.brainTreeBridge = brainTreeBridge;
        }
        public IActionResult Index(string searchName = null,
            string searchEmail = null, string searchPhone = null, string status = null)
        {
            OrderViewModel viewModel = new OrderViewModel()
            {
                OrderHeaderList = repositoryOrderHeader.GetAll(),
                StatusList = PathManager.StatusList.ToList().Select(
                    x => new SelectListItem { Text = x, Value = x }),
            };
            if (searchName != null)
            {
                viewModel.OrderHeaderList = viewModel.OrderHeaderList.Where(
                    x => x.FullName.ToLower().Contains(searchName.ToLower()));
            }
            if (searchEmail != null)
            {
                viewModel.OrderHeaderList = viewModel.OrderHeaderList.Where(
                    x => x.Email.ToLower().Contains(searchEmail.ToLower()));
            }
            if (searchPhone != null)
            {
                viewModel.OrderHeaderList = viewModel.OrderHeaderList.Where(
                    x => x.Phone.Contains(searchPhone));
            }
            if (status != null && status != "Choose Status")
            {
                viewModel.OrderHeaderList = viewModel.OrderHeaderList.Where(
                     x => x.Status == status);
            }
            return View(viewModel);
        }
        public IActionResult Details(int id)
        {
            OrderViewModel = new OrderHeaderDetailViewModel()
            {
                OrderDetail = repositoryOrderDetail.GetAll(x=>x.OrderHeaderId==id,
                includeProperties: "Product"),
                OrderHeader = repositoryOrderHeader.FirstOrDefault(x => x.Id == id)
            };
            return View(OrderViewModel);
        }
        [HttpPost]
        public IActionResult StartInProcessing()
        {
            OrderHeader oh = repositoryOrderHeader.
                FirstOrDefault(x => x.Id == OrderViewModel.OrderHeader.Id);

            oh.Status = PathManager.StatusInProcess;

            repositoryOrderHeader.Save();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult StartOrderDone()
        {
            OrderHeader oh = repositoryOrderHeader.
               FirstOrDefault(x => x.Id == OrderViewModel.OrderHeader.Id);

            oh.Status = PathManager.StatusOrderDone;

            repositoryOrderHeader.Save();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult StartOrderCancel()
        {
            OrderHeader oh = repositoryOrderHeader.
               FirstOrDefault(x => x.Id == OrderViewModel.OrderHeader.Id);

            var gateWay = brainTreeBridge.GetGateWay();

            Transaction tr = gateWay.Transaction.Find(oh.TransactionId);

            if (tr.Status==TransactionStatus.AUTHORIZED ||
                tr.Status==TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                gateWay.Transaction.Void(oh.TransactionId);
            }
            else
            {
                gateWay.Transaction.Refund(oh.TransactionId);
            }

            oh.Status = PathManager.StatusDenied;

            repositoryOrderHeader.Save();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult StartBack()
        {
            OrderHeader oh = repositoryOrderHeader.
               FirstOrDefault(x => x.Id == OrderViewModel.OrderHeader.Id);

            oh.Status = PathManager.StatusPending;

            repositoryOrderHeader.Save();

            return RedirectToAction("Index");
        }
    }
}
