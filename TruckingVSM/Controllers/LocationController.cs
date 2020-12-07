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
    public class LocationController : Controller
    {
        TruckingVSMEntities db = new TruckingVSMEntities();
        // GET: Location
        public ActionResult Index()
        {
            return View();
        }

        //get Location Index Data
        public async Task<JsonResult> GetLocation()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Location> conList = await db.Locations.ToListAsync<Location>();
            return Json(new { data = conList }, JsonRequestBehavior.AllowGet);
        }

        //edit Location
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new Location());
            else
            {
                return View(db.Locations.Where(x => x.ID == id).FirstOrDefault<Location>());
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(Location con)
        {
            if (con.ID == 0)
            {
                db.Locations.Add(con);
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

        //delete Location
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Location con = db.Locations.Where(x => x.ID == id).FirstOrDefault<Location>();
            db.Locations.Remove(con);
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }
    }
}