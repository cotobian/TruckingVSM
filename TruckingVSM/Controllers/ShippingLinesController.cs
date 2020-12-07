using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TruckingVSM.Models;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;

namespace TruckingVSM.Controllers
{
    public class ShippingLinesController : Controller
    {
        TruckingVSMEntities db = new TruckingVSMEntities();
        // GET: ShippingLines
        public ActionResult Index()
        {
            return View();
        }

        //get Trailer Index Data
        public async Task<JsonResult> GetLines()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<ShippingLine> List = await db.ShippingLines.ToListAsync<ShippingLine>();
            return Json(new { data = List }, JsonRequestBehavior.AllowGet);
        }


        //edit Lines
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new ShippingLine());
            else
            {
                return View(db.ShippingLines.Where(x => x.ID == id).FirstOrDefault<ShippingLine>());
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(ShippingLine con)
        {
            if (con.ID == 0)
            {
                db.ShippingLines.Add(con);
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

        //delete Fee
        [HttpPost]
        public ActionResult Delete(int id)
        {
            ShippingLine con = db.ShippingLines.Where(x => x.ID == id).FirstOrDefault<ShippingLine>();
            db.ShippingLines.Remove(con);
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }
    }
}