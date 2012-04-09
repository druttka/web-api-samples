using System.Web.Mvc;
using System.Web.Security;

namespace WebApi.MvcHosted.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (!Membership.ValidateUser(username, password)) return View();

            FormsAuthentication.SetAuthCookie("druttka", false);
            return RedirectToAction("Index", "Home");
        }
    }
}