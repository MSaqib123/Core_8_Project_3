using Microsoft.AspNetCore.Mvc;
using Proj.DataAccess.Repository.IRepository;
using Proj.Utility;
using System.Security.Claims;

namespace Proj.Web.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent 
    {
        private readonly IUnitOfWork iUnit;
        public ShoppingCartViewComponent(IUnitOfWork _iUnit)
        {
            iUnit = _iUnit;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //_______ Setting Cart SEssion on login __________
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                //if session is null then  check to db
                if(HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(
                    SD.SessionCart,
                    iUnit.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).Count()
                  );
                }

                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }

        }
    }
}
