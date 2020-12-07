using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TruckingVSM.Models;

namespace TruckingVSM.Controllers
{
    public class StandardFeeController : Controller
    {
        TruckingVSMEntities db = new TruckingVSMEntities();
        // GET: StandardFee
        public ActionResult Index()
        {
            return View();
        }

        //Get Code and Unit of Std Fee
        [HttpPost]
        public ActionResult GetStdFeeInfo(int id)
        {
            StandardFee StdFee =  db.StandardFees.Where(f => f.ID == id).FirstOrDefault();
            return Json(StdFee, JsonRequestBehavior.AllowGet);
        }
    }
}