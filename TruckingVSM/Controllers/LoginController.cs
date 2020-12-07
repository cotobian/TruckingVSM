using System.Linq;
using System.Web.Mvc;
using TruckingVSM.Models;
using System.Web.Security;

namespace TruckingVSM.Controllers
{
    public class LoginController : Controller
    {
        TruckingVSMEntities db = new TruckingVSMEntities();
        CustomMembershipProvider provider = new CustomMembershipProvider();
        // GET: Login
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        //xac thuc tai khoan
        [HttpPost]
        public ActionResult Authenticate(string username,string password, string ReturnUrl)
        {
            if(provider.ValidateUser(username,password) == true)
            {
                User user = db.Users.Where(u => u.Username == username).FirstOrDefault();
                FormsAuthentication.SetAuthCookie(username + "|" + user.FullName + "|" + user.RolesID, true);
                return Redirect("/Index/Index");
            }
            else
            {
                TempData["Tag"] = "Wrong username or password!";
                return RedirectToAction("Index", "Login", TempData);
            }
        }
        [AllowAnonymous]
        //LogOut Action
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/Login/Index");
        }

        //doi password
        public ActionResult Edit()
        {
            return View();
        }

        //ham post doi mat khau
        public ActionResult ChangePass(string username, string password, string newpassword)
        {
            User u = db.Users.Where(x => x.Username == username && x.Password == password).FirstOrDefault();
            if (u != null)
            {
                u.Password = newpassword;
                db.SaveChanges();
                FormsAuthentication.SetAuthCookie(username + "|" + u.FullName + "|" + u.RolesID, true);
                return Redirect("/Index/Index");
            }
            else
            {
                TempData["Tag"] = "Wrong username or password!";
                return RedirectToAction("Edit", "Login", TempData);
            }
        }
    }
}