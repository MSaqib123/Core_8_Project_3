using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proj.DataAccess.Data;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models;
using Proj.Models.ViewModel;
using Proj.Utility;
using System.Data;

namespace Proj.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _iUnit;
        private readonly IWebHostEnvironment _iWeb;
        public ProductController(IUnitOfWork iUnit, IWebHostEnvironment iWeb)
        {
            _iUnit = iUnit;
            _iWeb = iWeb;
        }
        public IActionResult Index()
        {
            var list = _iUnit.Product.GetAll(includeProperties:"Category").ToList();
            return View(list);
        }

        //_______________________ Insert, Update _______________________
        #region Seprate : Insert,Update
        [HttpGet]
        public IActionResult Create()
        {
            Product obj = new Product();
            IEnumerable<SelectListItem> categoryList = _iUnit.Category.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CategoryId.ToString()
            });

            //________ ViewBag ___________
            //ViewBag.CategoryList = categoryList;

            //________ ViewData ___________
            //ViewData["CategoryList"] = categoryList;

            //________ ViewModel ___________
            ProductVM vm = new ProductVM();
            vm.categoryList_obj = categoryList;
            vm.Product_obj = obj;

            //return View(obj);
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(ProductVM vm)
        {
            if (ModelState.IsValid)
            {
                //if (vm.Product_obj.ImageUrl == null)
                //{
                //    vm.Product_obj.ImageUrl = "";
                //}
                _iUnit.Product.Add(vm.Product_obj);
                _iUnit.SaveChange();
                TempData["Success"] = "Inserted Successfuly";
                return RedirectToAction("Index");
            }
            return View(vm);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? obj = _iUnit.Product.Get(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            IEnumerable<SelectListItem> categoryList = _iUnit.Category.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CategoryId.ToString()
            });

            ProductVM vm = new ProductVM();
            vm.categoryList_obj = categoryList;
            vm.Product_obj = obj;

            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(ProductVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Product_obj.Id > 0)
                {
                    //if (vm.Product_obj.ImageUrl == null)
                    //{
                    //    vm.Product_obj.ImageUrl = "";
                    //}
                    _iUnit.Product.Update(vm.Product_obj);
                    _iUnit.SaveChange();
                    TempData["Success"] = "Updated Successfuly";
                    return RedirectToAction("Index");
                }
            }
            return View(vm);
        }

        #endregion

        //_______________________ Upsert _______________________
        #region Together : Insert,Update
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            ProductVM vm = new ProductVM();
            Product obj = new Product();

            IEnumerable<SelectListItem> categoryList = _iUnit.Category.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.CategoryId.ToString()
            });

            if (id> 0)
            {
                obj = _iUnit.Product.Get(x => x.Id == id);
                if (obj == null)
                {
                    return NotFound();
                }
                vm.categoryList_obj = categoryList;
                vm.Product_obj = obj;
            }
            else
            {
                vm.categoryList_obj = categoryList;
                vm.Product_obj = obj;
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM vm, IFormFile? files)
        {
            if (ModelState.IsValid)
            {
                //string wwwRootPath = _iWeb.WebRootPath;
                //if (file != null)
                //{
                //    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                //    string productPath = Path.Combine(wwwRootPath,@"Images\Products");

                //    #region badWay
                //    //if (vm.Product_obj.ImageUrl != null)
                //    //if (!string.IsNullOrEmpty(vm.Product_obj.ImageUrl))
                //    //{
                //    //    var oldImagePath = Path.Combine(wwwRootPath,vm.Product_obj.ImageUrl.TrimStart('\\'));
                //    //    if (System.IO.File.Exists(oldImagePath) && oldImagePath != "Images\\NoImage.jpg")
                //    //    {
                //    //        System.IO.File.Delete(oldImagePath);
                //    //    }
                //    //}
                //    #endregion  

                //    //__________ Delete Old Image _________
                //    DeleteOldImage(vm.Product_obj.ImageUrl,wwwRootPath);

                //    //__________ Saveing New Image _________
                //    using (var fileStream = new FileStream(Path.Combine(productPath,fileName) , FileMode.Create))
                //    {
                //        file.CopyTo(fileStream);
                //    }
                //    vm.Product_obj.ImageUrl = @"\Images\Products\" + fileName;
                //}

                if (vm.Product_obj.Id > 0)
                {
                    //if (vm.Product_obj.ImageUrl == null && file == null)
                    //{
                    //    vm.Product_obj.ImageUrl = "\\Images\\NoImage.jpg";
                    //}
                    _iUnit.Product.Update(vm.Product_obj);
                    TempData["Success"] = "Updated Successfuly";
                }
                else
                {
                    //if (vm.Product_obj.ImageUrl == null)
                    //{
                    //    vm.Product_obj.ImageUrl = "\\Images\\NoImage.jpg";
                    //}
                    _iUnit.Product.Add(vm.Product_obj);
                    TempData["Success"] = "Inserted Successfuly";
                }

                _iUnit.SaveChange();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Success"] = "";
                IEnumerable<SelectListItem> categoryList = _iUnit.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.CategoryId.ToString()
                });
                vm.categoryList_obj = categoryList;
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
            Product? obj = _iUnit.Product.Get(x => x.Id == id);

            //__________ Delete Old Image _________
            //string wwwRootPath = _iWeb.WebRootPath;
            //DeleteOldImage(obj.ImageUrl, wwwRootPath);

            _iUnit.Product.Remove(obj);
            _iUnit.SaveChange();
            TempData["Success"] = "Deleted Successfuly";
            return RedirectToAction("Index");
        }



        private void DeleteOldImage(string Image, string wwwRootPath)
        {
            if (!string.IsNullOrEmpty(Image))
            {
                var oldImagePath = Path.Combine(wwwRootPath, Image.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath) && oldImagePath != "Images\\NoImage.jpg")
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
        }


        //_______________________ APis _______________________
        #region Apis work
        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _iUnit.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data= list });
        }


        [HttpDelete]
        public IActionResult DeleteRecord(int? id)
        {
            if (id == null || id == 0)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            Product? obj = _iUnit.Product.Get(x => x.Id == id);
            if (obj == null)
            {
                return Json(new {success=false,message="Error while deleting"});
            }
            //__________ Delete Old Image _________
            string wwwRootPath = _iWeb.WebRootPath;
            //DeleteOldImage(obj.ImageUrl, wwwRootPath);

            _iUnit.Product.Remove(obj);
            _iUnit.SaveChange();

            return Json(new { success = true, message = "Deleted Successfully" });
        }

        #endregion
    }
}
