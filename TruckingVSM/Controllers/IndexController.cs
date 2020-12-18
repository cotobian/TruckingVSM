using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using TruckingVSM.Models;
using System.Web.Mvc;
using System.Threading.Tasks;
using System;
using System.Globalization;

namespace TruckingVSM.Controllers
{
    public class IndexController : Controller
    {
        TruckingVSMEntities db = new TruckingVSMEntities();
        // GET: Index
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        //get consignee Index View
        public ActionResult Consignee()
        {
            return View();
        }

        //get Consignee Index Data
        public JsonResult GetConsignee()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<VT_GetAllConsigneeWithArea_Result> conList = db.VT_GetAllConsigneeWithArea().ToList();
            return Json(new { data = conList }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //ham get consignee voi ten
        public async Task<JsonResult> GetCusByName(string term)
        {
            var conList = await db.Consignees
                .Where(x => x.Name.ToUpper().Contains(term.ToUpper()))
                .Select(x => new { x.Name, x.ID })
                .ToListAsync();
            return Json(conList, JsonRequestBehavior.AllowGet);
        }

        //edit Consignee
        [HttpGet]
        public ActionResult AddOrEditConsignee(int id = 0)
        {
            if (id == 0)
            {
                List<Area> alist = db.Areas.ToList();
                ViewBag.data = alist;
                return View(new VT_GetConsigneeWithArea_Result());
            }
            else
            {
                List<Area> alist = db.Areas.ToList();
                ViewBag.data = alist;
                return View(db.VT_GetConsigneeWithArea(id).FirstOrDefault());
            }
        }

        [HttpPost]
        public ActionResult AddOrEditConsignee(VT_GetConsigneeWithArea_Result con)
        {
            if (con.ID == 0)
            {
                Consignee c = new Consignee();
                c.Address = con.Address;
                c.Name = con.Name;
                c.AreaID = con.aid;
                c.ShortName = con.ShortName;
                db.Consignees.Add(c);
                db.SaveChanges();
                return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Consignee c = db.Consignees.Where(x => x.ID == con.ID).FirstOrDefault();
                c.Address = con.Address;
                c.Name = con.Name;
                c.TaxCode = con.TaxCode;
                c.AreaID = con.aid;
                c.ShortName = con.ShortName;
                c.UpdateEmailTime = TimeSpan.Parse(con.UpdateEmailTime);
                db.Entry(c).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        //delete Consignee
        [HttpPost]
        public ActionResult DeleteConsignee(int id)
        {
            Consignee con = db.Consignees.Where(x => x.ID == id).FirstOrDefault<Consignee>();
            db.Consignees.Remove(con);
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }
    }
}