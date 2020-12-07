using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using TruckingVSM.Models;
using System.Web.Mvc;

namespace TruckingVSM.Controllers
{
    public class SMSController : Controller
    {
        // GET: SMS
        public ActionResult Index()
        {
            return View();
        }
        //init db
        Zalo zalo = new Zalo();
        TruckingVSMEntities db = new TruckingVSMEntities();
        //ham tim kiem va send SMS bang id
        [HttpPost]
        public JsonResult SendById(string id)
        {
            int result = 0;
            VT_GetSMSInfo_Result param = db.VT_GetSMSInfo(int.Parse(id)).FirstOrDefault();
            if (param.Type.Contains("Xuất") || param.Type.Contains("Xuat"))
            {
                result = zalo.sendMessage(param.ZaloId, "LỆNH ĐIỀU VẬN: HÀNG XUẤT \nNgày vận chuyển:" + param.transdate + "\nKH:" + param.ShortName +
                    "\nADD:" + param.Location + "\nLOẠI: " + param.CntrSize + "-" + param.Shipping + "\nLH:" + 
                    param.Caller + "\nYC: " + param.Inquiry + "\nTàu chạy/Hết hạn: " + param.ExpireDate + "\nBốc cont/Hạ rỗng:" + param.PickupYard
                    + "\nGhi chú: " + param.Note);
            }
            else
            {
                result = zalo.sendMessage(param.ZaloId, "LỆNH ĐIỀU VẬN: HÀNG NHẬP \nNgày vận chuyển:" + param.transdate + "\nKH:" + param.ShortName +
                    "\nADD:" + param.Location + "\nLOẠI: " + param.CntrSize + "-" + param.Shipping + "\n LH:" +
                    param.Caller + "\nYC: " + param.Inquiry + "\nTàu chạy/Hết hạn: " + param.ExpireDate + "\nBốc cont/Hạ rỗng:" + param.PickupYard
                    + "\nGhi chú: " + param.Note);
            }
            return Json(new { success = true, message = "Gửi tin zalo thành công" }, JsonRequestBehavior.AllowGet);
        }
        //ham tim kiem va gui SMS huy bang id
        [HttpPost]
        public JsonResult DeleteById(string id)
        {
            int result = 0;
            VT_GetSMSInfo_Result param = db.VT_GetSMSInfo(int.Parse(id)).FirstOrDefault();
            if (param.Type.Contains("Xuất") || param.Type.Contains("Xuat"))
            {
                result = zalo.sendMessage(param.ZaloId, "LỆNH ĐIỀU VẬN: HÀNG XUẤT - HỦY KẾ HOẠCH \nNgày vận chuyển:" + param.transdate + "\nKH:" + param.ShortName +
                    "\nADD:" + param.Location + "\nLOẠI: " + param.CntrSize + "-" + param.Shipping + "\nLH:" +
                    param.Caller + "\nYC: " + param.Inquiry + "\nTàu chạy/Hết hạn: " + param.ExpireDate + "\nBốc cont/Hạ rỗng:" + param.PickupYard
                    + "\nGhi chú: " + param.Note);
            }
            else
            {
                result = zalo.sendMessage(param.ZaloId, "LỆNH ĐIỀU VẬN: HÀNG NHẬP - HỦY KẾ HOẠCH \nNgày vận chuyển:" + param.transdate + "\nKH:" + param.ShortName +
                    "\nADD:" + param.Location + "\nLOẠI: " + param.CntrSize + "-" + param.Shipping + "\nLH:" +
                    param.Caller + "\nYC: " + param.Inquiry + "\nTàu chạy/Hết hạn: " + param.ExpireDate + "\nBốc cont/Hạ rỗng:" + param.PickupYard
                    + "\nGhi chú: " + param.Note);
            }

            return Json(new { success = true, message = "Gửi tin zalo thành công" }, JsonRequestBehavior.AllowGet);
        }

        //ham gui SMS huy bang id tra ve string
        public async Task<string> DeleteByIdString(string id)
        {
            string result = "";
            string sessionid = await loginSMS();
            string keepsessionresult = await keepSession(sessionid);
            VT_GetSMSInfo_Result param = db.VT_GetSMSInfo(int.Parse(id)).FirstOrDefault();
            if (param.Type.Contains("Xuất") || param.Type.Contains("Xuat"))
            {
                result = await sendDeleteSMSExport(sessionid, param.Phone, param.ShortName, param.transdate,
                param.Location, param.CntrSize, param.PickupYard, param.bookbill);
                string logout = await logoutSMS(sessionid);
            }
            else
            {
                result = await sendDeleteSMSImport(sessionid, param.Phone, param.ShortName, param.transdate,
                param.Location, param.CntrSize, param.PickupYard, param.bookbill);
                string logout = await logoutSMS(sessionid);
            }
            return result;
        }

        //ham tim kiem va send SMS import bang id
        [HttpPost]
        public async Task<JsonResult> SendImportById(string id)
        {
            string result = "";
            string sessionid = await loginSMS();
            string keepsessionresult = await keepSession(sessionid);
            VT_GetSMSInfo_Result param = db.VT_GetSMSInfo(int.Parse(id)).FirstOrDefault();
            result = await sendCreateSMSImport(sessionid, param.Phone, param.ShortName, param.transdate,
                param.Location, param.CntrSize, param.PickupYard, param.bookbill);

            return Json(new { success = true, message = result }, JsonRequestBehavior.AllowGet);
        }

        //ham tim kiem va send SMS export bang id
        [HttpPost]
        public async Task<JsonResult> SendExportById(string id)
        {
            string result = "";
            string sessionid = await loginSMS();
            string keepsessionresult = await keepSession(sessionid);
            VT_GetSMSInfo_Result param = db.VT_GetSMSInfo(int.Parse(id)).FirstOrDefault();
            result = await sendCreateSMSExport(sessionid, param.Phone,  param.ShortName, param.transdate,
                param.Location, param.CntrSize, param.PickupYard, param.bookbill);

            return Json(new { success = true, message = result }, JsonRequestBehavior.AllowGet);
        }

        ////ham login + keepsession + sendcreate
        //public async Task<JsonResult> sendSMSThrough()
        //{
        //    string sessionid = await loginSMS();
        //    string keepsessionresult = await keepSession(sessionid);
        //    string result = await sendCreateSMS(sessionid, "0775317840", "123", "123", "123", "123", "123"
        //        , "123","123");
        //    string logout = await logoutSMS(sessionid); 
        //    return Json(new { success = true, message = result }, JsonRequestBehavior.AllowGet);
        //}

        //public async Task<JsonResult> deleteSMSThrough()
        //{
        //    string sessionid = await loginSMS();
        //    string keepsessionresult = await keepSession(sessionid);
        //    string result = await sendDeleteSMS(sessionid, "0775317840", "123", "123", "123", "123", "123"
        //        , "123");
        //    string logout = await logoutSMS(sessionid);
        //    return Json(new { success = true, message = result }, JsonRequestBehavior.AllowGet);
        //}

        public async Task<string> loginSMS()
        {
            try
            {
                string result = "";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    HttpResponseMessage response = await client.GetAsync("http://onesms.mobifone.vn");
                    if (response.IsSuccessStatusCode)
                    {
                        string responsetext = await response.Content.ReadAsStringAsync();
                        result = responsetext.Split(new char[] { '=', '&' })[3];
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //ham keep session
        public async Task<string> keepSession(string sessionid)
        {
            try
            {
                string result = "";
                string request = "http://onesms.mobifone.vn" + sessionid;
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    HttpResponseMessage response = await client.GetAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string responsetext = await response.Content.ReadAsStringAsync();
                        result = responsetext.Split('|')[1];
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //ham logout khoi he thong SMS
        public async Task<string> logoutSMS(string sessionid)
        {
            try
            {
                string result = "";
                string request = "http://onesms.mobifone.vn" + sessionid;
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    HttpResponseMessage response = await client.GetAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string responsetext = await response.Content.ReadAsStringAsync();
                        result = responsetext.Split('|')[1];
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //ham gui SMS tao chuyen
        public async Task<string> sendCreateSMS(string sessionid, string receiver, string param1, string param2, string param3, string param4,
            string param5, string param6, string param7)
        {
            try
            {
                string result = "";
                string request = "http://onesms.mobifone.vn" + sessionid +
                    "&sms_id=1&msisdn=" + receiver + "&sms_template_code=3K0002&param1=" + param1 + "&param2=" + param2 + "&param3=" +
                    param3 + "&param4=" + param4 + "&param5=" + param5 + "&param6=" + param6 + "&param7=" + param7;
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    HttpResponseMessage response = await client.GetAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string responsetext = await response.Content.ReadAsStringAsync();
                        result = responsetext.Split('|')[1];
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //ham gui SMS huy chuyen
        public async Task<string> sendDeleteSMS(string sessionid, string receiver, string param1, string param2, string param3, string param4,
            string param5, string param6)
        {
            try
            {
                string result = "";
                string request = "http://onesms.mobifone.vn" + sessionid +
                    "&sms_id=1&msisdn=" + receiver + "&sms_template_code=3K0001&param1=" + param1 + "&param2=" + param2 + "&param3=" +
                    param3 + "&param4=" + param4 + "&param5=" + param5 + "&param6=" + param6;
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    HttpResponseMessage response = await client.GetAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string responsetext = await response.Content.ReadAsStringAsync();
                        result = responsetext.Split('|')[1];
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //ham gui SMS tao chuyen nhap
        public async Task<string> sendCreateSMSImport(string sessionid, string receiver, string param1, string param2, string param3, string param4,
            string param5, string param6)
        {
            try
            {
                string result = "";
                string request = "http://onesms.mobifone.vn" + sessionid +
                    "&sms_id=1&msisdn=" + receiver + "&sms_template_code=3DA005&param1=" + param1 + "&param2=" + param2 + "&param3=" +
                    param3 + "&param4=" + param4 + "&param5=" + param5 + "&param6=" + param6;
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    HttpResponseMessage response = await client.GetAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string responsetext = await response.Content.ReadAsStringAsync();
                        result = responsetext.Split('|')[1];
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //ham gui SMS tao chuyen xuat
        public async Task<string> sendCreateSMSExport(string sessionid, string receiver, string param1, string param2, string param3, string param4,
            string param5, string param6)
        {
            try
            {
                string result = "";
                string request = "http://onesms.mobifone.vn" + sessionid +
                    "&sms_id=1&msisdn=" + receiver + "&sms_template_code=3DA003&param1=" + param1 + "&param2=" + param2 + "&param3=" +
                    param3 + "&param4=" + param4 + "&param5=" + param5 + "&param6=" + param6 ;
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    HttpResponseMessage response = await client.GetAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string responsetext = await response.Content.ReadAsStringAsync();
                        result = responsetext.Split('|')[1];
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //ham gui SMS huy chuyen xuat
        public async Task<string> sendDeleteSMSExport(string sessionid, string receiver, string param1, string param2, string param3, string param4,
            string param5, string param6)
        {
            try
            {
                string result = "";
                string request = "http://onesms.mobifone.vn" + sessionid +
                    "&sms_id=1&msisdn=" + receiver + "&sms_template_code=3DA004&param1=" + param1 + "&param2=" + param2 + "&param3=" +
                    param3 + "&param4=" + param4 + "&param5=" + param5 + "&param6=" + param6;
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    HttpResponseMessage response = await client.GetAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string responsetext = await response.Content.ReadAsStringAsync();
                        result = responsetext.Split('|')[1];
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //ham gui SMS huy chuyen nhap
        public async Task<string> sendDeleteSMSImport(string sessionid, string receiver, string param1, string param2, string param3, string param4,
            string param5, string param6)
        {
            try
            {
                string result = "";
                string request = "http://onesms.mobifone.vn" + sessionid +
                    "&sms_id=1&msisdn=" + receiver + "&sms_template_code=3DA006&param1=" + param1 + "&param2=" + param2 + "&param3=" +
                    param3 + "&param4=" + param4 + "&param5=" + param5 + "&param6=" + param6;
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    HttpResponseMessage response = await client.GetAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string responsetext = await response.Content.ReadAsStringAsync();
                        result = responsetext.Split('|')[1];
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}