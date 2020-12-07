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
    public class TruckController : Controller
    {
        TruckingVSMEntities db = new TruckingVSMEntities();
        // GET: Truck
        public ActionResult Index()
        {
            return View();
        }

        //get Truck Index Data
        public JsonResult GetTruck()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<VT_GetTruck_Result> conList = db.VT_GetTruck().ToList();
            return Json(new { data = conList }, JsonRequestBehavior.AllowGet);
        }

        //ham tra ve danh sach xe van tai goi y
        [HttpPost]
        public async Task<JsonResult> GetTruckByName(string term,string kind)
        {
            bool or=false;
            if (kind.Equals("true")) or = true;
            var LocationList = await db.Trucks.Where(c => c.TruckNo.ToUpper()
                                            .Contains(term.ToUpper()) && c.Status.Equals("Available") && c.OwnRent == or)
                                            .Select(c => new { Name = c.TruckNo, ID = c.ID })
                                            .Distinct().ToListAsync();
            return Json(LocationList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetAllTruckByName(string term)
        {
            var LocationList = await db.Trucks.Where(c => c.TruckNo.ToUpper()
                                            .Contains(term.ToUpper()) && c.Status.Equals("Available"))
                                            .Select(c => new { Name = c.TruckNo, ID = c.ID })
                                            .Distinct().ToListAsync();
            return Json(LocationList, JsonRequestBehavior.AllowGet);
        }

        //ham tra ve ten tai xe theo so xe
        [HttpPost]
        public async Task<JsonResult> GetDriverByTruck(string term)
        {
            int? id = await db.Trucks.Where(c => c.TruckNo.Equals(term)).Select(c => c.DriverID).FirstOrDefaultAsync();
            var driver = await db.Drivers.Where(x => x.ID == id).Select(x => new { Name = x.Name, ID = x.ID }).FirstOrDefaultAsync();
            return Json(driver, JsonRequestBehavior.AllowGet);
        }

        //edit Truck
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new VT_GetTruckInFo_Result());
            else
            {
                VT_GetTruckInFo_Result result = db.VT_GetTruckInFo(id).FirstOrDefault();
                return View(result);
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(VT_GetTruckInFo_Result con)
        {
            if (con.ID == 0)
            {
                Truck truck = new Truck();
                truck.DriverID = con.DriverID;
                truck.TruckNo = con.TruckNo;
                truck.Status = con.Status;
                truck.TrailerID = con.TrailerID;
                truck.Expire = con.Expire;
                truck.Transit = con.Transit;
                truck.OwnRent = con.OwnRent;
                db.Trucks.Add(truck);
                db.SaveChanges();
                return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Truck truck = db.Trucks.Where(c => c.ID == con.ID).FirstOrDefault();
                truck.DriverID = con.DriverID;
                truck.TruckNo = con.TruckNo;
                truck.Status = con.Status;
                truck.TrailerID = con.TrailerID;
                truck.Expire = con.Expire;
                truck.Transit = con.Transit;
                truck.OwnRent = con.OwnRent;
                db.Entry(truck).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        //delete Truck
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Truck con = db.Trucks.Where(x => x.ID == id).FirstOrDefault<Truck>();
            db.Trucks.Remove(con);
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }

    }
}