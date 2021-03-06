﻿using System;
using System.Linq;
using TruckingVSM.Models;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;

namespace TruckingVSM.Controllers
{
    public class TransactionController : Controller
    {
        TruckingVSMEntities db = new TruckingVSMEntities();
        // GET: Transaction
        [Authorize]
        public ActionResult Index()
        {
            List<Fee> flist = db.Fees.ToList();
            List<OrderType> olist = db.OrderTypes.ToList();
            List<ShippingLine> slist = db.ShippingLines.ToList();
            ViewBag.odata = olist;
            ViewBag.sdata = slist;
            return View();
        }

        //trang quan li giao dich
        [Authorize]
        public ActionResult Manage()
        {
            ViewBag.perm = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name.Split('|')[2]);
            return View();
        }

        //ham tao giao dịch mới
        [HttpPost]
        public ActionResult AddNew(string Type,string Bill, string Note,string PickupYard,string Commodity,string Inquiry,string Payer,string Weight,string Caller,
            string ConsigneeID,string ExpireDate,string Shipping,string ChangePlan, string TransportDate,string StuffDate,
            string StuffWH)
        {
            Transaction t = new Transaction();
            DateTime date = DateTime.ParseExact(ExpireDate + " 23:59", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            t.CreateDate = DateTime.Now;
            t.ConsigneeID = Int32.Parse(ConsigneeID);
            t.Note = Note;
            t.Type = Uri.UnescapeDataString(Type);
            t.Total = 0;
            t.TransportDate = DateTime.ParseExact(TransportDate, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            t.ExpireDate = date;
            t.StuffDate = StuffDate;
            t.StuffingWarehouse = StuffWH;
            t.ChangePlan = ChangePlan;
            if (Type.Contains("Xuat"))
            {
                t.Booking = Bill;
            }
            else
            {
                t.Bill = Bill;
            }
            t.PickupYard = PickupYard;
            t.staff_cd = User.Identity.Name.Split('|')[0];
            t.Shipping = Shipping;
            t.update_time = DateTime.Now;
            t.Commodity = Commodity;
            t.Inquiry = Inquiry;
            t.Payer = Payer;
            t.Weight = Weight;
            t.Caller = Caller;
            db.Transactions.Add(t);
            db.SaveChanges();
            int id = t.ID;
            int cid = Int32.Parse(ConsigneeID);
            var price = db.FeeByConsignees.Where(x => x.ConsigneeID == cid).Select(x => new { x.Name, x.ShortName }).ToList(); 
            return Json(new { success = true, message = "Saved Successfully", tranid = id, price}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult checkBookExist(string bkno, string blno)
        {
            string result = "0";
            if (db.VT_CheckBookExist(bkno, blno).FirstOrDefault() != null) result = "1";
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        //ham tra ve danh sach khach hang goi y
        [HttpPost]
        public async Task<JsonResult> GetCustomerList(string term)
        {
            var Customerlist = await db.Consignees.Where(c => c.Name.ToUpper()
                                            .Contains(term.ToUpper()))
                                            .Select(c => new { Name = c.Name, ID = c.ID })
                                            .Distinct().ToListAsync();
            return Json(Customerlist, JsonRequestBehavior.AllowGet);
        }

        //ham tra ve danh sach dia chi goi y
        [HttpPost]
        public async Task<JsonResult> GetLocationList(string term)
        {
            var LocationList = await db.Locations.Where(c => c.Name.ToUpper()
                                            .Contains(term.ToUpper()))
                                            .Select(c => new { Name = c.Name, ID = c.ID })
                                            .Distinct().ToListAsync();
            return Json(LocationList, JsonRequestBehavior.AllowGet);
        }
        
        //delete Consignee
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Transaction con = db.Transactions.Where(x => x.ID == id).FirstOrDefault<Transaction>();
            db.Transactions.Remove(con);
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }

        //get view edit transaction
        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.ID = id;
            VT_GetTransacstionById_Result model = db.VT_GetTransacstionById(id).FirstOrDefault();
            List<FeeByConsignee> clist = db.FeeByConsignees.Where(x => x.ConsigneeID == model.ConsigneeID).ToList();
            ViewBag.odata = clist;
            return View(model);
        }

        //ham lay danh sach transaction
        public async Task<JsonResult> GetTransaction()
        {
            return await Task.Run<JsonResult>(() =>
            {
                db.Configuration.ProxyCreationEnabled = false;
                var data = db.Database
                            .SqlQuery<VT_GetTransaction_Result>("VT_GetTransaction")
                            .ToList();
                return Json(new { data }, JsonRequestBehavior.AllowGet);
            });   
        }

        //ham lay danh sach transaction theo ngay 
        public async Task<JsonResult> GetTransactionByDate(string Start, string End)
        {
            return await Task.Run<JsonResult>(() =>
            {
                DateTime start = DateTime.ParseExact(Start + " 00:00", "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(End + " 23:59", "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
                List<VT_GetTransactionByDate_Result> data = db.VT_GetTransactionByDate(start, end).ToList();
                return Json(new { data }, JsonRequestBehavior.AllowGet);
            });
        }
        //ham lay ghi chu
        [HttpPost]
        public async Task<JsonResult> GetNoteByName(string term)
        {
            return await Task.Run<JsonResult>(() =>
            {
                int id = Int32.Parse(term.Substring(term.LastIndexOf("-") + 1));
                string data = db.VT_GetNoteByName(id).FirstOrDefault();
                return Json(new { data }, JsonRequestBehavior.AllowGet);
            });
        }

        //ham lay danh sach transaction detail
        [HttpPost]
        public async Task<JsonResult> GetDetail(int id)
        {
            return await Task.Run<JsonResult>(() =>
            {
                db.Configuration.ProxyCreationEnabled = false;
                List<VT_GetTransactionDetailById_Result> data = db.VT_GetTransactionDetailById(id).ToList();
                return Json(new { data }, JsonRequestBehavior.AllowGet);
            });
        }

        //ham view detail transaction
        public ActionResult Detail(int TransID)
        {
            ViewBag.TransID = TransID;
            List<Fee> flist = db.Fees.ToList();
            ViewBag.data = flist;
            return View();
        }

        //ham xoa transaction detail
        [HttpPost]
        public JsonResult DeleteDetail(string id)
        {
            int did = Int32.Parse(id);
            db.VT_DeleteDetailById(did);
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }

        //ham xoa transaction
        [HttpPost]
        public JsonResult DeleteTransaction(string id)
        {
            db.VT_DeleteTransactionById(Int32.Parse(id));
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }

        //ham tao detail transaction
        [HttpPost]
        public ActionResult AddDetail(string TransactionID, string Quantity, string FeeName,
            string FeeAmount, string CntrSize,string LocationID)
        {
            int tid = Int32.Parse(TransactionID);
            string unit = Uri.UnescapeDataString(CntrSize);
            TransactionDetail td = new TransactionDetail();
            Transaction t = db.Transactions.Where(x => x.ID == tid).FirstOrDefault();
            td.TransactionID = Int32.Parse(TransactionID);
            td.FeeName = FeeName;
            td.FeeAmount = Int32.Parse(FeeAmount);
            td.Quantity = Int32.Parse(Quantity);
            td.Total = td.FeeAmount * td.Quantity;
            t.Total += td.Total; 
            td.CntrSize = unit;
            db.TransactionDetails.Add(td);
            db.SaveChanges();
            if(FeeName.Contains("VanChuyen"))
            {
                for(int i=0;i<Int32.Parse(Quantity);i++)
                {
                    TransportOrder to = new TransportOrder();
                    to.TransactionID = td.ID;
                    to.LocationID = Int32.Parse(LocationID);
                    to.CntrSize = unit;
                    db.TransportOrders.Add(to);
                    db.SaveChanges();
                }
            }
            return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
        }

        //edit Trailer
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0) return View(new Trailer());
            else
            {
                List<OrderType> flist = db.OrderTypes.ToList();
                ViewBag.data = flist;
                List<ShippingLine> lines = db.ShippingLines.ToList();
                ViewBag.sdata = lines;
                return View(db.VT_GetTransacstionById(id).FirstOrDefault<VT_GetTransacstionById_Result>());
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(VT_GetTransacstionById_Result con)
        {
            DateTime end = new DateTime();
            if (con.ExpireDate.Length < 12)
            {
                end = DateTime.ParseExact(con.ExpireDate + " 23:59", "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            Transaction tran = db.Transactions.Where(x => x.ID == con.ID).FirstOrDefault();
            tran.TransportDate = con.TransportDate;
            tran.Note = con.Note;
            if(con.Type.Contains("Xuat")||con.Type.Contains("Xuất"))
            {
                tran.Booking = con.Bill;
            }
            else tran.Bill = con.Bill;
            tran.Commodity = con.Commodity;
            tran.Caller = con.Caller;
            tran.Inquiry = con.Inquiry;
            tran.Payer = con.Payer;
            tran.Weight = con.Weight;
            tran.Type = con.Type;
            tran.Shipping = con.Shipping;
            tran.PickupYard = con.PickupYard;
            tran.ConsigneeID = con.ConsigneeID;
            tran.ChangePlan = con.ChangePlan;
            if (con.ExpireDate.Length < 12)
            {
                tran.ExpireDate = end;
            }
            db.SaveChanges();
            return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
        }

        //ham tao chi tiet nhanh
        [HttpPost]
        public JsonResult AddDetailFast(string ConsigneeID, string Cntr20, string Cntr40DC,string Cntr40HC, string Cntr45, string HangLe, string TransactionID, string LocationID)
        {
            DateTime now = DateTime.Now;
            if(!string.IsNullOrEmpty(Cntr20))
            {
                db.VT_AddDetailFast(Int32.Parse(TransactionID), Int32.Parse(ConsigneeID), Int32.Parse(LocationID), Int32.Parse(Cntr20), "Cont 20", "", now);
            }
            if (!string.IsNullOrEmpty(Cntr40DC))
            {
                db.VT_AddDetailFast(Int32.Parse(TransactionID), Int32.Parse(ConsigneeID), Int32.Parse(LocationID), Int32.Parse(Cntr40DC), "Cont 40DC", "", now);
            }
            if (!string.IsNullOrEmpty(Cntr40HC))
            {
                db.VT_AddDetailFast(Int32.Parse(TransactionID), Int32.Parse(ConsigneeID), Int32.Parse(LocationID), Int32.Parse(Cntr40HC), "Cont 40HC", "", now);
            }
            if (!string.IsNullOrEmpty(Cntr45))
            {
                db.VT_AddDetailFast(Int32.Parse(TransactionID), Int32.Parse(ConsigneeID), Int32.Parse(LocationID), Int32.Parse(Cntr45), "Cont 45", "", now);
            }
            if (!string.IsNullOrEmpty(HangLe))
            {

            }
            //db.VT_AddDetailFast(TransactionID, ConsigneeID, LocationID, Quantity, CntrSize, "", now);
            db.VT_ResetTotalPrice(Int32.Parse(TransactionID));
            return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
        }

        //ham edit detail
        [HttpGet]
        public ActionResult AddOrEditDetail(int id)
        {
            return View(db.VT_GetDetailById(id).FirstOrDefault());
        }
        
        //ham chinh sua transaction detail
        [HttpPost]
        public ActionResult AddOrEditDetail(VT_GetDetailById_Result con)
        {
            TransactionDetail td = db.TransactionDetails.Where(x => x.ID == con.ID).FirstOrDefault();
            if(td.Quantity != con.Quantity)
            { 
                if(td.Quantity < con.Quantity)
                {
                    db.VT_ExtraDetail(td.ID, td.FeeName, con.Quantity, td.FeeAmount, DateTime.Now);
                    return Json(new { success = true, message = "Added Successfully" }, JsonRequestBehavior.AllowGet);
                }
                if(td.Quantity > con.Quantity)
                {
                    if(con.Quantity == 0)
                    {
                        db.TransactionDetails.Remove(td);
                        db.SaveChanges();
                        return Json(new { success = true, message = "Removed Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if(!con.FeeName.Contains("VanChuyen"))
                        {
                            db.VT_RemoveDetail(td.ID, td.FeeName, con.Quantity, td.FeeAmount, DateTime.Now);
                            return Json(new { success = true, message = "Removed Successfully" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var count = db.TransportOrders
                                    .Where(o => o.TransactionID == con.ID && o.TripID != null)
                                    .Count();
                            if (count < (td.Quantity - con.Quantity))
                            {
                                return Json(new { success = true, message = "Lỗi: số lệnh cần xóa lớn hơn số lệnh đã tạo chuyến" }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                db.VT_RemoveDetail(td.ID, td.FeeName, con.Quantity, td.FeeAmount, DateTime.Now);
                                return Json(new { success = true, message = "Removed Successfully" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            TransactionDetail tran = db.TransactionDetails.Where(x => x.ID == con.ID).FirstOrDefault();
            tran.FeeAmount = con.FeeAmount;
            tran.Total = tran.FeeAmount * tran.Quantity;
            db.Entry(tran).State = EntityState.Modified;
            db.SaveChanges();
            int? total = db.TransactionDetails.Where(x => x.TransactionID == tran.TransactionID).Sum(x => x.Total);
            Transaction tr = db.Transactions.Where(x => x.ID == tran.TransactionID).FirstOrDefault();
            tr.Total = total;
            db.Entry(tr).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
            
        }
    }
}