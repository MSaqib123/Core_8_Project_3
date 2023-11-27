using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Proj.DataAccess.Data;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models;
using Proj.Utility;

namespace Proj.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _db;
        //private readonly ICategoryRepository _iRepo;
        private readonly IUnitOfWork _iUnit;


        //public CategoryController(ApplicationDbContext db)
        //public CategoryController(ICategoryRepository iRepo)
        public CategoryController(IUnitOfWork iUnit)
        {
            _iUnit = iUnit;
        }
        public IActionResult Index()
        {
            //var catList = _db.Categories.ToList();
            //var catList = _iRepo.GetAll();
            var catList = _iUnit.Category.GetAll();
            return View(catList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            Category cat = new Category();
            return View(cat);
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            //_______________ 2. Customer Validation ________________
            //display order
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Name and DisplayOrder can not be same");
            }
            //unique name
            //if (_iRepo.GetAll().Any(e => e.Name == obj.Name))
            if (_iUnit.Category.GetAll().Any(e => e.Name == obj.Name))
            {
                ModelState.AddModelError("name", "Name can not be same");
            }
            //Validation_Summery_works
            if (obj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "Name and DisplayOrder can not be same");
            }

            //________ 1. Server Side Validation ____________
            if (ModelState.IsValid)
            {
                //_iRepo.Add(obj);
                _iUnit.Category.Add(obj);
                //_iRepo.SaveChange();
                _iUnit.SaveChange();
                TempData["Success"] = "Inserted Successfuly";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //___ for only F.K Find ____
            //Category? category1 = _db.Categories.Find(id);
            ////___ withOUt F.k but  get Single Record ____
            //Category? category2 = _db.Categories.FirstOrDefault(u=>u.CategoryId == id);
            //Category? category3 = _db.Categories.Where(x=>x.CategoryId == id).FirstOrDefault();
            //Category? category3 = _iRepo.Get(x=>x.CategoryId == id);
            Category? category3 = _iUnit.Category.Get(x => x.CategoryId == id);


            if (category3 == null)
            {
                return NotFound();
            }
            return View(category3);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            //_______________ 2. Customer Validation ________________
            //display order
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Name and DisplayOrder can not be same");
            }
            //unique name
            //if (_iRepo.GetAll().Any(e => e.Name == obj.Name))
            if (_iUnit.Category.GetAll().Any(e => e.Name == obj.Name))
            {
                ModelState.AddModelError("name", "Name can not be same");
            }
            //Validation_Summery_works
            if (obj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "Name and DisplayOrder can not be same");
            }

            //_______________ 1. Server Side Validation _________________
            if (ModelState.IsValid)
            {
                //___ for only F.K Find ____
                if (obj.CategoryId > 0)
                {
                    //_iRepo.Update(obj);
                    _iUnit.Category.Update(obj);
                    //_iRepo.SaveChange();
                    _iUnit.SaveChange();
                    TempData["Success"] = "Updated Successfuly";
                    return RedirectToAction("Index");
                }
            }
            return View(obj);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //Category? category1 = _iRepo.Get(x=>x.CategoryId == id);
            Category? category1 = _iUnit.Category.Get(x => x.CategoryId == id);
            //_iRepo.Remove(category1);
            _iUnit.Category.Remove(category1);
            //_iRepo.SaveChange();
            _iUnit.SaveChange();
            TempData["Success"] = "Deleted Successfuly";
            return RedirectToAction("Index");
        }

    }
}
