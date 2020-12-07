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
    public class AreaController : Controller
    {
        TruckingVSMEntities db = new TruckingVSMEntities();
        // GET: Area
        public ActionResult Index()
        {
            return View();
        }

        //get Area Index Data
        public async Task<JsonResult> GetArea()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Area> conList = await db.Areas.ToListAsync<Area>();
            return Json(new { data = conList }, JsonRequestBehavior.AllowGet);
        }
        //edit Area
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new Area());
            else
            {
                return View(db.Areas.Where(x => x.ID == id).FirstOrDefault<Area>());
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(Area con)
        {
            if (con.ID == 0)
            {
                db.Areas.Add(con);
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

        //delete Area
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Area con = db.Areas.Where(x => x.ID == id).FirstOrDefault<Area>();
            db.Areas.Remove(con);
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }
    }
}