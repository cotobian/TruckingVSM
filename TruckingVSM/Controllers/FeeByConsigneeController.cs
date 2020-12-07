using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TruckingVSM.Models;

namespace TruckingVSM.Controllers
{
    public class FeeByConsigneeController : Controller
    {
        TruckingVSMEntities db = new TruckingVSMEntities();
        // GET: FeeByConsignee
        public ActionResult Index()
        {
            return View();
        }

        //get Fee By Customer
        public JsonResult getFee()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<VT_GetAllFeeByConsignee_Result> conList = db.VT_GetAllFeeByConsignee().ToList();
            return Json(new { data = conList }, JsonRequestBehavior.AllowGet);
        }

        //edit Fee By Customer
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            ViewBag.Fee = db.StandardFees.ToList();
            if (id == 0)
            {
                
                return View(new VT_GetFeeByConsignee_Result());
            }
            else
            {
                return View(db.VT_GetFeeByConsignee(id).FirstOrDefault());
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(VT_GetFeeByConsignee_Result fee)
        {
            if (fee.ID == 0)
            {
                FeeByConsignee f = new FeeByConsignee();
                f.ConsigneeID = fee.cid;
                f.Name = fee.Name;
                f.Price = fee.Price;
                f.ShortName = fee.ShortName;
                f.StdFeeID = fee.fid;
                f.Unit = fee.Unit;
                db.FeeByConsignees.Add(f); 
                db.SaveChanges();
                return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                FeeByConsignee f = db.FeeByConsignees.Where(x => x.ID == fee.ID).FirstOrDefault();
                f.ConsigneeID = fee.cid;
                f.Name = fee.Name;
                f.Price = fee.Price;
                f.StdFeeID = fee.fid;
                f.ShortName = fee.ShortName;
                f.Unit = fee.Unit;
                db.Entry(f).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        //delete Fee By Customer
        [HttpPost]
        public ActionResult Delete(int id)
        {
            FeeByConsignee con = db.FeeByConsignees.Where(x => x.ID == id).FirstOrDefault<FeeByConsignee>();
            db.FeeByConsignees.Remove(con);
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }
    }
}