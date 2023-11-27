﻿using Microsoft.AspNetCore.Mvc;
using Proj.DataAccess.Repository.IRepository;

namespace Proj.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _iUnit;
        public ProductController(IUnitOfWork iUnit)
        {
            _iUnit = iUnit;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            var product = _iUnit.Product.Get(x=>x.Id == id,includeProperties : "Category");
            return View(product);
        }
    }
}
