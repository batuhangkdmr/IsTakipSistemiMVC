using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace İsTakipSistemiMVC.Controllers
{
    public class LogoutController : Controller
    {
        // GET: Logout
        // kullanıcı giriş yapma sayfası olan "Login" controller'ının "Index" action'ına yönlendirilir.
        public ActionResult Index()
        {
            // çıkış yapabilmesi için sesionların boşaltılması gerekiyor. bu yüzden session.Abandon yazıyoru.
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }
}