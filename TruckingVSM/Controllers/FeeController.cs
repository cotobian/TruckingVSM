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
    public class FeeController : Controller
    {
        TruckingVSMEntities db = new TruckingVSMEntities();
        // GET: Fee
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        //get Fee Index Data
        public async Task<JsonResult> GetFee()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Fee> conList = await db.Fees.ToListAsync<Fee>();
            return Json(new { data = conList }, JsonRequestBehavior.AllowGet);
        }
        //edit Fee
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new Fee());
            else
            {
                return View(db.Fees.Where(x => x.ID == id).FirstOrDefault<Fee>());
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(Fee con)
        {
            if (con.ID == 0)
            {
                db.Fees.Add(con);
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
            Fee con = db.Fees.Where(x => x.ID == id).FirstOrDefault<Fee>();
            db.Fees.Remove(con);
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }

        //get price by name
        [HttpPost]
        public JsonResult GetPriceByName(string term, string type)
        {
            int id = Int32.Parse(type);
            if(type.Equals("0"))
            {
                return Json(new { success = true, data = db.Fees.Where(x => x.ShortName.Contains(term)).Select(x => new { x.Price, x.Unit }).FirstOrDefault()
                }, JsonRequestBehavior.AllowGet);   
            }
            else return Json(new
            {
                success = true,
                data = db.FeeByConsignees.Where(x => x.ShortName.Contains(term) && x.ConsigneeID == id).Select(x => new { x.Price, x.Unit }).FirstOrDefault()
            }, JsonRequestBehavior.AllowGet);
        }
        
    }
}