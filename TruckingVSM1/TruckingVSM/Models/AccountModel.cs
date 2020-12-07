using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TruckingVSM.Models
{
    public class AccountModel
    {
        private TruckingVSMEntities db = null;
        public AccountModel()
        {
            db = new TruckingVSMEntities();
        }
        public bool Login(string username,string password)
        {
            var result = db.Users.Where(u => u.Username == username && u.Password == password).FirstOrDefault();
            if (result != null) return true;
            else return false;
        }
        public bool Delete(string username)
        {
            User user = db.Users.Where(u => u.Username == username).FirstOrDefault();
            db.Users.Remove(user);
            db.SaveChanges();
            return true;
        }
    }
}