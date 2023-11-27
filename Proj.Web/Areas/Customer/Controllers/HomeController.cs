using Microsoft.AspNetCore.Mvc;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models.ViewModel;
using Proj.Web.Models;
using System.Diagnostics;

namespace Proj.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _iUnit;

        public HomeController(IUnitOfWork iUnit, ILogger<HomeController> logger)
        {
            _iUnit = iUnit;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var list = _iUnit.Product.GetAll(includeProperties: "Category").ToList();
            IndexVM vm = new IndexVM();
            vm.ProductList = list;
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}