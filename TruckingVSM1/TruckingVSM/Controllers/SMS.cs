using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace TruckingVSM.Controllers
{
    public class SMS
    {
        //login vao he thong SMS Mobifone
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
            string param5, string param6)
        {
            try
            {
                string result = "";
                string request = "http://onesms.mobifone.vn" + sessionid +
                    "&sms_id=1&msisdn=" + receiver + "&sms_template_code=3K0002&param1=" + param1 + "&param2=" + param2 + "&param3=" +
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

        //ham gui SMS huy chuyen
        public async Task<string> sendDeleteSMS(string sessionid, string receiver, string param1, string param2, string param3, string param4,
            string param5, string param6, string param7)
        {
            try
            {
                string result = "";
                string request = "http://onesms.mobifone.vn" + sessionid +
                    "&sms_id=1&msisdn=" + receiver + "&sms_template_code=3K0001&param1=" + param1 + "&param2=" + param2 + "&param3=" +
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

        //create new SMS for new trip
        public async Task<string> sendSMSAddTrip(string receiver, string param1, string param2, string param3, string param4,
            string param5, string param6)
        {
            string sessionid = await loginSMS();
            string keepSesion = await keepSession(sessionid);
            string result = await sendCreateSMS(sessionid, receiver, param1, param2, param3, param4,
             param5, param6);
            string logout = await logoutSMS(sessionid);
            return result;
        }

        //create new SMS for delete trip
        public async Task<string> sendSMSDeleteTrip(string receiver, string param1, string param2, string param3, string param4,
            string param5, string param6, string param7)
        {
            string sessionid = await loginSMS();
            string keepSesion = await keepSession(sessionid);
            string result = await sendDeleteSMS(sessionid, receiver, param1, param2, param3, param4,
             param5, param6, param7);
            string logout = await logoutSMS(sessionid);
            return result;
        }
    }
}