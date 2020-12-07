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
    public class UserController : Controller
    {
        TruckingVSMEntities db = new TruckingVSMEntities();
        [Authorize(Roles ="Admin")]
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        
        //get User Index Data
        public async Task<JsonResult> GetUser()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<User> conList = await db.Users.ToListAsync<User>();
            return Json(new { data = conList }, JsonRequestBehavior.AllowGet);
        }

        //edit User
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new User());
            else
            {
                return View(db.Users.Where(x => x.ID == id).FirstOrDefault<User>());
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(User con)
        {
            if (con.ID == 0)
            {
                db.Users.Add(con);
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

        //delete User
        [HttpPost]
        public ActionResult Delete(int id)
        {
            User con = db.Users.Where(x => x.ID == id).FirstOrDefault<User>();
            db.Users.Remove(con);
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }
    }
}
