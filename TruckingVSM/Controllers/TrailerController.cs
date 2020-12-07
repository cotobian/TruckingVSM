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
    public class TrailerController : Controller
    {
        private static TruckingVSMEntities db = new TruckingVSMEntities();

        // GET: Trailer
        public ActionResult Index()
        {
            return View();
        }

        //get Trailer Index Data
        public async Task<JsonResult> GetTrailer()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Trailer> TrailerList = await db.Trailers.ToListAsync<Trailer>();
            return Json(new { data = TrailerList }, JsonRequestBehavior.AllowGet);
        }

        //ham goi y so ro mooc
        [HttpPost]
        public async Task<JsonResult> GetTrailerByName(string term)
        {
            var trailerlist = db.Trucks.Where(x => x.TrailerID != null).Select(x => x.TrailerID).ToList();
            var LocationList = await db.Trailers.Where(c => c.TrailerNo.ToUpper()
                                            .Contains(term.ToUpper()) && !trailerlist.Contains(c.ID))
                                            .Select(c => new { Name = c.TrailerNo, ID = c.ID })
                                            .Distinct().ToListAsync();
            return Json(LocationList, JsonRequestBehavior.AllowGet);
        }

        //get Trailer contains string
        public JsonResult GetTrailerContainString(string term)
        {
            var TrailerList = db.Trailers.Where(c => c.TrailerNo.Contains(term)).Select(c => new { c.ID, c.TrailerNo }).ToList();
            return Json(new { data = TrailerList }, JsonRequestBehavior.AllowGet);
        }

        //edit Trailer
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new Trailer());
            else
            {
                return View(db.Trailers.Where(x => x.ID == id).FirstOrDefault<Trailer>());
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(Trailer con)
        {
            if (con.ID == 0)
            {
                db.Trailers.Add(con);
                db.SaveChanges();
                return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var local = db.Set<Trailer>()
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

        //delete Trailer
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Trailer con = db.Trailers.Where(x => x.ID == id).FirstOrDefault<Trailer>();
            db.Trailers.Remove(con);
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }
    }
}