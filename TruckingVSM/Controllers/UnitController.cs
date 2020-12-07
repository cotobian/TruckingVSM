using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TruckingVSM.Models;

namespace TruckingVSM.Controllers
{
    public class UnitController : Controller
    {
        private TruckingVSMEntities db = new TruckingVSMEntities();

        // GET: Unit
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        //get Unit Index Data
        public async Task<JsonResult> GetUnit()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Unit> UnitList = await db.Units.ToListAsync<Unit>();
            return Json(new { data = UnitList }, JsonRequestBehavior.AllowGet);
        }

        //edit Unit
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new Unit());
            else
            {
                return View(db.Units.Where(x => x.ID == id).FirstOrDefault<Unit>());
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(Unit con)
        {
            if (con.ID == 0)
            {
                db.Units.Add(con);
                db.SaveChanges();
                return Json(new { success = true, message = "Lưu đơn vị tính thành công" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var local = db.Set<Unit>()
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
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Unit con = db.Units.Where(x => x.ID == id).FirstOrDefault<Unit>();
            db.Units.Remove(con);
            db.SaveChanges();
            return Json(new { success = true, message = "Xóa đơn vị tính thành công" }, JsonRequestBehavior.AllowGet);
        }
    }
}