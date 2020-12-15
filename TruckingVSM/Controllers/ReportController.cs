using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.Mvc;
using OfficeOpenXml;
using TruckingVSM.Models;
using System.Globalization;
using System.Web.Hosting;
using System.Threading.Tasks;

namespace TruckingVSM.Controllers
{
    public class ReportController : Controller
    {
        TruckingVSMEntities db = new TruckingVSMEntities();
        // GET: Report
        public ActionResult Index()
        {
            List<OrderType> olist = db.OrderTypes.ToList();
            ViewBag.odata = olist;
            return View();
        }

        public ActionResult GetRevenue()
        {
            List<Area> alist = db.Areas.ToList();
            ViewBag.data = alist;
            return View();
        }

        public ActionResult Daily()
        {
            return View();
        }

        //ham hien bao cao hang ngay ra luoi
        [HttpPost]
        public JsonResult DailyReportGrid(string startd, string endd)
        {
            DateTime start = DateTime.ParseExact(startd + " 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact(endd + " 23:59", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            List<VT_DailyPlan_Result> data = db.VT_DailyPlan(start, end).ToList();
            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        //ham ke hoach hang ngay
        public void DailyReport(string startd, string endd)
        {
            DateTime start = DateTime.ParseExact(startd + " 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact(endd + " 23:59", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            FileInfo template = new FileInfo(HostingEnvironment.MapPath("~/Content/template3.xlsx"));
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage EP = new ExcelPackage(template);
            ExcelWorksheet Sheet = EP.Workbook.Worksheets[0];
            List<VT_DailyPlan_Result> list = db.VT_DailyPlan(start, end).ToList();
            int row = 4;
            Sheet.Cells[string.Format("A1")].Value = "KẾ HOẠCH VẬN CHUYỂN NGÀY " + startd;
            foreach(var item in list)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = row - 3;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.TransportDate;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Name;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.laddress;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Caller;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Inquiry;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.cntr20;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.cntr40dc;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.cntr40hc;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.cntr45;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.doc;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.Shipping;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.Commodity;
                Sheet.Cells[string.Format("N{0}", row)].Value = item.Type;
                Sheet.Cells[string.Format("O{0}", row)].Value = item.ExpireDate.ToString().Substring(0,10);
                Sheet.Cells[string.Format("P{0}", row)].Value = item.bainang;
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.baiha;
                Sheet.Cells[string.Format("R{0}", row)].Value = item.Payer;
                Sheet.Cells[string.Format("S{0}", row)].Value = item.TruckNo;
                Sheet.Cells[string.Format("T{0}", row)].Value = item.CntrNo;
                Sheet.Cells[string.Format("U{0}", row)].Value = item.Weight;
                Sheet.Cells[string.Format("V{0}", row)].Value = item.driver;
                Sheet.Cells[string.Format("W{0}", row)].Value = item.nightstore.ToString();
                row++;
            }
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(EP.GetAsByteArray());
            Response.End();
        }

        //ham bao cao doanh thu van chuyen theo khach hang
        public void RevenueByCustomer(string CustomerName,string CustomerId,string Start,string End)
        {
            DateTime start = DateTime.ParseExact(Start + " 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact(End + " 23:59", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            FileInfo template = new FileInfo(HostingEnvironment.MapPath("~/Content/template.xlsx"));
            ExcelPackage EP = new ExcelPackage(template);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelWorksheet Sheet = EP.Workbook.Worksheets[0];
            int id = Int32.Parse(CustomerId);
            List<VT_RevenueByCustomer_Result> list = db.VT_RevenueByCustomer(id, start, end).ToList();
            int row = 16;
            Sheet.Cells[string.Format("D6")].Value = CustomerName;
            foreach (var item in list)
            {
                int sum = 0;
                Sheet.Cells[string.Format("A{0}", row)].Value = item.Type;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.doc;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.TruckNo;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.CntrNo;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Name;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.CreateDate;
                if (item.CntrSize.Contains("20"))
                {
                    Sheet.Cells[string.Format("H{0}", row)].Value = "1";
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.FeeAmount;
                    sum += Int32.Parse(item.FeeAmount.ToString()); 
                }
                else if (item.CntrSize.Contains("40"))
                {
                    Sheet.Cells[string.Format("I{0}", row)].Value = "1";
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.FeeAmount;
                    sum += Int32.Parse(item.FeeAmount.ToString());
                }
                else if (item.CntrSize.Contains("45"))
                {
                    Sheet.Cells[string.Format("J{0}", row)].Value = "1";
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.FeeAmount;
                    sum += Int32.Parse(item.FeeAmount.ToString());
                }
                if (db.VT_GetLiftingFee(item.TransID, item.CntrSize).FirstOrDefault() != null)
                {
                    if(item.CntrSize.Contains("20"))
                    {
                        Sheet.Cells[string.Format("N{0}", row)].Value = db.VT_GetLiftingFee(item.TransID, item.CntrSize);
                    }
                    else if(item.CntrSize.Contains("40"))
                    {
                        Sheet.Cells[string.Format("O{0}", row)].Value = db.VT_GetLiftingFee(item.TransID, item.CntrSize);
                    }
                    else if (item.CntrSize.Contains("45"))
                    {
                        Sheet.Cells[string.Format("P{0}", row)].Value = db.VT_GetLiftingFee(item.TransID, item.CntrSize);
                    }
                    sum += Int32.Parse(db.VT_GetLiftingFee(item.TransID, item.CntrSize).FirstOrDefault().ToString());
                }
                
                Sheet.Cells[string.Format("Q{0}", row)].Value = sum;
                Sheet.Cells[string.Format("R{0}", row)].Value = item.Note;
                row++;
            }
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(EP.GetAsByteArray());
            Response.End();
        }

        //ham bao cao doanh thu theo khach hang
        public void AllTariffByMonth(string Start, string End, string Area)
        {
            DateTime start = DateTime.ParseExact(Start + " 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact(End + " 23:59", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            int area = 0;
            if(!string.IsNullOrEmpty(Area))
            {
                area = Int32.Parse(Area);
            }
            FileInfo template = new FileInfo(HostingEnvironment.MapPath("~/Content/template1.xlsx"));
            ExcelPackage EP = new ExcelPackage(template);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelWorksheet Sheet = EP.Workbook.Worksheets[0];
            List<VT_GetCustomerWithTransaction_Result> list = db.VT_GetCustomerWithTransaction(start, end, area).ToList();
            int row = 5;
            int sum = 0;
            Sheet.Cells[string.Format("B2")].Value = "SẢN LƯỢNG HÀNG VẬN CHUYỂN TỪ " + Start + "-" + End;
            foreach (var item in list)
            {
                Sheet.Cells[string.Format("B{0}", row)].Value = row - 4;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Name;
                Sheet.Cells[string.Format("D{0}", row)].Value = db.VT_GetCntr20(start,end, item.ID);
                Sheet.Cells[string.Format("E{0}", row)].Value = db.VT_GetCntr40(start, end, item.ID);
                Sheet.Cells[string.Format("F{0}", row)].Value = db.VT_GetCntr45(start, end, item.ID);
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Total;
                sum += (int)item.Total;
                row++;
            }
            Sheet.Cells[string.Format("H{0}", row)].Value = sum;
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(EP.GetAsByteArray());
            Response.End();
        }
        public string getFullName(string shortname)
        {
            return db.Fees.Where(x => x.ShortName.Contains(shortname)).Select(x => x.Name).FirstOrDefault();
        }
        //ham bao cao tong doanh thu 
        public void RevenueByMonth(string Start, string End, string CustomerId, string CustomerName)
        {
            DateTime start = DateTime.ParseExact(Start + " 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact(End + " 23:59", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            FileInfo template = new FileInfo(HostingEnvironment.MapPath("~/Content/template2.xlsx"));
            ExcelPackage EP = new ExcelPackage(template);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelWorksheet Sheet = EP.Workbook.Worksheets[0];
            Sheet.Cells[string.Format("B3")].Value = "Từ " + Start + " - " + End + " - " + CustomerName;
            int id = Int32.Parse(CustomerId);
            List<VT_RevenueByMonth_Result> list = db.VT_RevenueByMonth(start, end, id).ToList();
            int row = 6;
            foreach (var item in list)
            {
                Sheet.Cells[string.Format("B{0}", row)].Value = row - 5;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.CreateDate;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Type;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Booking;
                Sheet.Cells[string.Format("F{0}", row)].Value = getFullName(item.FeeName);
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Unit;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Quantity;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.FeeAmount;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.Total;
                row++;
            }
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(EP.GetAsByteArray());
            Response.End();
        }

        //bao cao chuyen xe theo tai xe
        public void TripByDriver(string Start,string End)
        {
            DateTime start = DateTime.ParseExact(Start + " 00:00", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact(End + " 23:59", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            List<VT_TripByDriver_Result> driverList = db.VT_TripByDriver(start, end).ToList();
            ExcelPackage EP = new ExcelPackage();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelWorksheet Sheet = EP.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A2"].Value = Start + "-" + End;
            Sheet.Cells["A3"].Value = "Số cont";
            Sheet.Cells["B3"].Value = "Địa chỉ";
            Sheet.Cells["C3"].Value = "Xe vận tải";
            Sheet.Cells["D3"].Value = "Tài xế";
            Sheet.Cells["E3"].Value = "Ngày vận chuyển";
            Sheet.Cells["F3"].Value = "Ngày hoàn tất";
            Sheet.Cells["G3"].Value = "Trạng thái";
            Sheet.Cells["H3"].Value = "Loại hàng";
            Sheet.Cells["I3"].Value = "Loại xe";
            int row = 4;
            foreach (var item in driverList)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.CntrNo;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.TruckNo;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Driver;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.CreateDate;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.CompleteDate;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Status;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Type;
                if(item.OwnRent == false)
                {
                    Sheet.Cells[string.Format("I{0}", row)].Value = "Xe nhà";
                }
                else
                {
                    Sheet.Cells[string.Format("I{0}", row)].Value = "Xe thuê";
                }
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(EP.GetAsByteArray());
            Response.End();
        }

        //bao cao dieu van tong hop

    }
}