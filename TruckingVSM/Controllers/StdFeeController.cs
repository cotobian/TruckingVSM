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
    public class StdFeeController : Controller
    {
        TruckingVSMEntities db = new TruckingVSMEntities();
        // GET: StdFee
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        //get StdFee Index Data
        public async Task<JsonResult> GetFee()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<StandardFee> feeList = await db.StandardFees.ToListAsync<StandardFee>();
            return Json(new { data = feeList }, JsonRequestBehavior.AllowGet);
        }

        //edit StdFee
        [Authorize(Roles = "Admin")]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new StandardFee());
            else
            {
                return View(db.StandardFees.Where(x => x.ID == id).FirstOrDefault<StandardFee>());
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AddOrEdit(StandardFee con)
        {
            if (con.ID == 0)
            {
                db.StandardFees.Add(con);
                db.SaveChanges();
                return Json(new { success = true, message = "Lưu biểu cước thành công" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var local = db.Set<StandardFee>()
                         .Local
                         .FirstOrDefault(f => f.ID == con.ID);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }
                db.Entry(con).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
        }

        //delete StandardFee
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            StandardFee con = db.StandardFees.Where(x => x.ID == id).FirstOrDefault<StandardFee>();
            db.StandardFees.Remove(con);
            db.SaveChanges();
            return Json(new { success = true, message = "Xóa biểu cước thành công" }, JsonRequestBehavior.AllowGet);
        }

    }
}