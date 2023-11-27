using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models;
using Proj.Utility;

namespace Proj.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _iUnit;
        public CompanyController(IUnitOfWork iUnit)
        {
            _iUnit = iUnit;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var list = _iUnit.Company.GetAll();
            return View(list);
        }

        //_______________________ Upsert _______________________
        #region Together : Insert,Update
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            Company obj = new Company();
            if (id> 0)
            {
                obj = _iUnit.Company.Get(x => x.Id == id);
                if (obj == null)
                {
                    return NotFound();
                }
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Upsert(Company vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Id > 0 )
                {
                    _iUnit.Company.Update(vm);
                    TempData["Success"] = "Updated Successfuly";
                }
                else
                {
                    _iUnit.Company.Add(vm);
                    TempData["Success"] = "Inserted Successfuly";
                }
                _iUnit.SaveChange();
                return RedirectToAction("Index");
            }
            else
            {
                return View(vm);
            }
            
        }

        #endregion

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Company? obj = _iUnit.Company.Get(x => x.Id == id);

            
            _iUnit.Company.Remove(obj);
            _iUnit.SaveChange();
            TempData["Success"] = "Deleted Successfuly";
            return RedirectToAction("Index");
        }


        //_______________________ APis _______________________
        #region Apis work
        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _iUnit.Company.GetAll().ToList();
            return Json(new {data= list });
        }


        [HttpDelete]
        public IActionResult DeleteRecord(int? id)
        {
            if (id == null || id == 0)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            Company? obj = _iUnit.Company.Get(x => x.Id == id);
            if (obj == null)
            {
                return Json(new {success=false,message="Error while deleting"});
            }

            _iUnit.Company.Remove(obj);
            _iUnit.SaveChange();

            return Json(new { success = true, message = "Deleted Successfully" });
        }

        #endregion
    }
}
