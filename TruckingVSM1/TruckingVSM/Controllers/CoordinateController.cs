using System;
using System.Collections.Generic;
using System.Linq;
using TruckingVSM.Models;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Globalization;
using System.Threading;

namespace TruckingVSM.Controllers
{
    public class CoordinateController : Controller
    {
        //Initialize class
        TruckingVSMEntities db = new TruckingVSMEntities();
        SMS message = new SMS();
        Zalo zalo = new Zalo();

        // GET: Coordinate
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.perm = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name.Split('|')[2]);
            return View();
        }

        //trang thong tin xe
        public ActionResult TruckInfo(string day)
        {
            if(string.IsNullOrEmpty(day))
            {
                var query = from ti in db.TruckInfoes
                            where ti.Ngay.Day == DateTime.Now.Day && ti.Ngay.Year == DateTime.Now.Year && ti.Ngay.Month == DateTime.Now.Month
                            select ti;
                return View(query.FirstOrDefault<TruckInfo>());
            }
            else
            {
                DateTime date = DateTime.ParseExact(day,"MM/dd/yyyy", CultureInfo.InvariantCulture);
                var query = from ti in db.TruckInfoes
                            where ti.Ngay.Day == date.Day && ti.Ngay.Year == date.Year && ti.Ngay.Month == date.Month
                            select ti;
                return View(query.FirstOrDefault<TruckInfo>());
            }
        }

        //thong tin 
        //tao dieu van moi
        [Authorize]
        public ActionResult CreateNew()
        {
            List<string> orderx = db.VT_TransportOrderX().ToList();
            ViewBag.orderx = orderx;
            List<string> ordern = db.VT_TransportOrderN().ToList();
            ViewBag.ordern = ordern;
            List<string> ordernn = db.VT_TransportOrderNN().ToList();
            ViewBag.ordernn = ordernn;
            List<string> ordernx = db.VT_TransportOrderNX().ToList();
            ViewBag.ordernx = ordernx;
            ViewBag.cntr20 = db.VT_RemainOrder("20").FirstOrDefault();
            ViewBag.cntr40 = db.VT_RemainOrder("40").FirstOrDefault();
            ViewBag.cntr45 = db.VT_RemainOrder("45").FirstOrDefault();
            return View();
        }

        //edit Trip
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            VT_GetTripOrder_Result odr = db.VT_GetTripOrder(id).FirstOrDefault();
            return View(odr);
        }

        [HttpPost]
        public ActionResult AddOrEdit(VT_GetTripOrder_Result result)
        {
            if(result.CompleteDate == null)
            {
                db.VT_CompleteTrip(result.CntrNo, result.ID, result.TruckNo, DateTime.Now, result.Status,result.Note);
            }
            else
            {
                DateTime complete = DateTime.ParseExact(result.CompleteDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                db.VT_CompleteTrip(result.CntrNo, result.ID, result.TruckNo, complete, result.Status,result.Note);
            }
            db.SaveChanges();
            return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
        }

        //ham lay danh sach trip
        public async Task<JsonResult> GetTrip()
        {
            return await Task.Run<JsonResult>(() =>
            {
                db.Configuration.ProxyCreationEnabled = false;
                var data = db.Database
                            .SqlQuery<VT_GetCompleteTrip_Result>("VT_GetCompleteTrip")
                            .ToList();
                return Json(new { data }, JsonRequestBehavior.AllowGet);
            });
        }

        //ham lay danh sach trip theo ngay
        public async Task<JsonResult> GetTripByDate(string Start, string End)
        {
            return await Task.Run<JsonResult>(() =>
            {
                db.Configuration.ProxyCreationEnabled = false;
                DateTime start = DateTime.ParseExact(Start + " 00:00", "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(End + " 23:59", "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
                List<VT_GetTripByDate_Result> data = db.VT_GetTripByDate(start, end).ToList();
                return Json(new { data }, JsonRequestBehavior.AllowGet);
            });
        }

        //ham tao trip moi
        public JsonResult AddTrip(string TruckId, string DriverId,  string Order1,string Order2,string Status,string Note)
        {
            int id1 = 0;
            id1 = Int32.Parse(Order1.Substring(Order1.LastIndexOf("-") + 1));
            string staff_cd = User.Identity.Name.Split('|')[0];
            if(!string.IsNullOrEmpty(Order2))
            {
                int id2 = Int32.Parse(Order2.Substring(Order2.LastIndexOf("-") + 1));
                db.VT_AddNewTrip(Int32.Parse(TruckId), Int32.Parse(DriverId), "", "", id1, id2, DateTime.Now, staff_cd,"OnGoing",Note);
            }
            else db.VT_AddNewTrip(Int32.Parse(TruckId), Int32.Parse(DriverId), "", "NULL" , id1, 0, DateTime.Now, staff_cd,"OnGoing",Note);
            return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
        }

        //edit Info
        [HttpGet]
        public ActionResult AddOrEditInfo(int id = 0)
        {
            if (id == 0) return View(new TruckInfo());
            else
            {
                return View(db.TruckInfoes.Where(x => x.ID == id).FirstOrDefault<TruckInfo>());
            }
        }

        [HttpPost]
        public ActionResult AddOrEditInfo(TruckInfo ti)
        {
            if (ti.ID == 0)
            {
                ti.Ngay = DateTime.Now.Date;
                db.TruckInfoes.Add(ti);
                db.SaveChanges();
                return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ti.Ngay = DateTime.Now.Date;
                db.Entry(ti).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        //delete Trip
        [HttpPost]
        public JsonResult Delete(int id)
        {
            if(db.TransportOrders.Where(c => c.ID == id).Select(c => c.SendSMS).FirstOrDefault().Equals("1"))
            {
                int result = 0;
                VT_GetSMSInfo_Result param = db.VT_GetSMSInfo(id).FirstOrDefault();
                if (param.Type.Contains("Xuất") || param.Type.Contains("Xuat"))
                {
                    result = zalo.sendMessage(param.ZaloId, "LỆNH ĐIỀU VẬN: HÀNG XUẤT - HỦY KẾ HOẠCH \nNgày:" + param.transdate + "\nKH:" + param.ShortName +
                        "\nADD:" + param.Location + "\nLOẠI: " + param.CntrSize + "-" + param.Shipping + "\nLH:" +
                        param.Caller + "\nYC: " + param.Inquiry + "\nTàu chạy/Hết hạn: " + param.ExpireDate + "\nBốc cont/Hạ rỗng:" + param.PickupYard
                        + "\nGhi chú: " + param.Note);
                }
                else
                {
                    result = zalo.sendMessage(param.ZaloId, "LỆNH ĐIỀU VẬN: HÀNG NHẬP - HỦY KẾ HOẠCH \nNgày:" + param.transdate + "\nKH:" + param.ShortName +
                        "\nADD:" + param.Location + "LOẠI: " + param.CntrSize + "-" + param.Shipping + "\n LH:" +
                        param.Caller + "\nYC: " + param.Inquiry + "\nTàu chạy/Hết hạn: " + param.ExpireDate + "\nBốc cont/Hạ rỗng:" + param.PickupYard
                        + "\nGhi chú: " + param.Note);
                }
                db.VT_DeleteTripByOrder(id);
                db.SaveChanges();
                //if (sms.Contains("SUCCESS"))
                //{
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
                /*}
                else
                {
                    return Json(new { success = false, message = "Deleted Failed" }, JsonRequestBehavior.AllowGet);
                }*/
            }
            else
            {
                db.VT_DeleteTripByOrder(id);
                db.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}