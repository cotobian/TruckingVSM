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
    public class DriverController : Controller
    {
        private static TruckingVSMEntities db = new TruckingVSMEntities();
        // GET: Driver
        public ActionResult Index()
        {
            return View();
        }

        //get Driver Index Data
        public async Task<JsonResult> GetDriver()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Driver> driverList = await db.Drivers.ToListAsync<Driver>();
            return Json(new { data = driverList }, JsonRequestBehavior.AllowGet);
        }

        //ham goi y ten tai xe
        [HttpPost]
        public async Task<JsonResult> GetDriverByName(string term)
        {
            var LocationList = await db.Drivers.Where(c => c.Name.ToUpper()
                                            .Contains(term.ToUpper()))
                                            .Select(c => new { Name = c.Name, ID = c.ID })
                                            .Distinct().ToListAsync();
            return Json(LocationList, JsonRequestBehavior.AllowGet);
        }

        //edit Driver
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new Driver());
            else
            {
                return View(db.Drivers.Where(x => x.ID == id).FirstOrDefault<Driver>());
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(Driver con)
        {
            if (con.ID == 0)
            {
                db.Drivers.Add(con);
                db.SaveChanges();
                return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var local = db.Set<Driver>()
                         .Local
                         .FirstOrDefault(f => f.ID == con.ID);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                db.Entry(con).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        //delete Driver
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Driver con = db.Drivers.Where(x => x.ID == id).FirstOrDefault<Driver>();
            db.Drivers.Remove(con);
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }
    }
}