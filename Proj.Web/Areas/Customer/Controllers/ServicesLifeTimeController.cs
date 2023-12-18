using Microsoft.AspNetCore.Mvc;
using Proj.Web.Services;
using System.Text;

namespace Proj.Web.Areas.Customer.Controllers
{
    public class ServicesLifeTimeController : Controller
    {
        private readonly ISingleTonGuidService _SingleTone1;
        private readonly ISingleTonGuidService _SingleTone2;

        private readonly IScopedGuidService _Scope1;
        private readonly IScopedGuidService _Scope2;

        private readonly ITransientGuidService _Trans1;
        private readonly ITransientGuidService _Trans2;

        public ServicesLifeTimeController(
                ISingleTonGuidService singleTone1,
                ISingleTonGuidService singleTone2,
                IScopedGuidService Scope1,
                IScopedGuidService Scope2,
                ITransientGuidService Trans1,
                ITransientGuidService Trans2
            )
        {
            _SingleTone1 = singleTone1;
            _SingleTone2 = singleTone2;

            _Scope1 = Scope1;
            _Scope2 = Scope2;

            _Trans1 = Trans1;
            _Trans2 = Trans2;
        }

        public IActionResult Index()
        {
            StringBuilder str = new StringBuilder();
            str.Append("__________ SingleTon LifeCycle (same for End Of Project) _________________\n\n");
            str.Append($"SingleTon 1 : {_SingleTone1.GetGuild()} \n");
            str.Append($"SingleTon 2 : {_SingleTone2.GetGuild()} \n\n");

            str.Append("__________ Scope LifeCycle (same for Per Request) _________________\n\n");
            str.Append($"Scope 1 : {_Scope1.GetGuild()} \n");
            str.Append($"Scope 2 : {_Scope2.GetGuild()} \n\n");

            str.Append("__________ Transient L.C (Change with Every Request) _________________\n\n");
            str.Append($"Transient 1 : {_Trans1.GetGuild()} \n");
            str.Append($"Transient 2 : {_Trans2.GetGuild()} \n");



            return Ok(str.ToString());
        }
    }
}
