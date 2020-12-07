using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TruckingVSM.Models
{
    public class AccountModel
    {
        private TruckingVSMEntities db;
        public AccountModel()
        {
            db = new TruckingVSMEntities();
        }
        public bool Login(string username, string password)
        {
            var result = db.Users.Where(u => u.Username == username && u.Password == password).FirstOrDefault();
            if (result != null) return true;
            return false;
        }
        public bool Delete(string username)
        {
            User user = db.Users.Where(u => u.Username == username).FirstOrDefault();
            db.Users.Remove(user);
            db.SaveChanges();
            return true;
        }
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}