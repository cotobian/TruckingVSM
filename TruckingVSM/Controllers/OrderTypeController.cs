using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TruckingVSM.Models;


namespace TruckingVSM.Controllers
{
    public class OrderTypeController : Controller
    {
        TruckingVSMEntities db = new TruckingVSMEntities();
        // GET: OrderType
        [Authorize(Roles ="Admin")]
        public ActionResult Index()
        {
            return View();
        }
        //get Fee Index Data
        public async Task<JsonResult> GetOrderType()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<OrderType> conList = await db.OrderTypes.ToListAsync<OrderType>();
            return Json(new { data = conList }, JsonRequestBehavior.AllowGet);
        }
        //edit Fee
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new OrderType());
            else
            {
                return View(db.OrderTypes.Where(x => x.ID == id).FirstOrDefault<OrderType>());
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(OrderType con)
        {
            if (con.ID == 0)
            {
                db.OrderTypes.Add(con);
                db.SaveChanges();
                return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                db.Entry(con).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        //delete OrderType
        [HttpPost]
        public ActionResult Delete(int id)
        {
            OrderType con = db.OrderTypes.Where(x => x.ID == id).FirstOrDefault<OrderType>();
            db.OrderTypes.Remove(con);
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }

    }
}