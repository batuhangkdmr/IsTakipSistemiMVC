using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using İsTakipSistemiMVC.Filters;
using İsTakipSistemiMVC.Models;

namespace İsTakipSistemiMVC.Controllers
{
    public class LoginController : Controller
    {
        // entity nesnesi üretiyoruz. bu entity sayesinde verittabanına bağlantı kurabiliyoruz.
        İsTakipDBEntities entity = new İsTakipDBEntities();
        // GET: Login
        public ActionResult Index()
        {
            ViewBag.mesaj = null;
            return View();
        }
        [HttpPost, ExcFilter]
        // Index adında ActionResult dönen bir method oluşturuyor.
        public ActionResult Index(string kullaniciAd, string parola)
        {
            var personel = (from p in entity.Personeller where p.personelKullanıcıAd == kullaniciAd && p.PersonelParola == parola && p.aktiflik == true select p).FirstOrDefault();
            // kullanıcı var mı yok mu kontrolü ettiğimiz bölge
            if (personel != null)
            {  // bu kodda veritabanında kullanıcı adı ve parolası bu değerlerle eşleşen bir kullanıcıyı sorgular ve personel değişkenine atar.

                // kullanıcının bağlı olduğu birimi kontrol eder. Eğer kullanıcı birime bağlı değilse yönetici sayfasına yönlendirilir.
                var birim = (from b in entity.Birimler where b.birimId == personel.personelBirimId select b).FirstOrDefault();
                // giriş yapıp yapmadığımızı bu session bilgisine göre kontrol ediyoruz.
                Session["PersonelAdSoyad"] = personel.PersonelAdSoyad;
                Session["PersonelId"] = personel.PersonelId;
                Session["PersonelBirimId"] = personel.personelBirimId;
                Session["PersonelYetkiTurId"] = personel.personelYetkiTurId;

                // bu komutta eğer null 'sa bu bir sistem yöneticisidir sistem yönetici sayfasına yönlenmesi gerekir.

                if (birim == null)
                {
                    return RedirectToAction("Index", "SistemYoneticisi");
                }

                // Eğer birim aktifse, aşağıdaki işlemler gerçekleştirilir. Eğer birim aktif değilse, kullanıcıya bir hata mesajı gösterilir.
                if (birim.aktiflik == true)
                {
                    // Sistem Yöneticisi tarafınsan bir hesap oluşturulduktan sonra şifreyi değiştirene kadar şifre değiştirme sayfasına atamasını sağlıyoruz @@@@@@@@@@@@@
                    if (personel.yeniPersonel == true)
                    {
                        return RedirectToAction("Index", "SifreKontrol");
                    }

                    switch (personel.personelYetkiTurId)
                    {
                        case 1:
                            return RedirectToAction("Index", "Yonetici");
                        case 2:
                            return RedirectToAction("Index", "Calisan");
                        default:
                            return View();
                    }
                }
                else
                {
                    ViewBag.mesaj = "Biriminiz silindiği için giriş yapamazsınız!";
                    return View();
                }
            }
            else
            {
                ViewBag.mesaj = "Kullanıcı Adı ya da parola yanlış!";
                return View();

            }

        }
    }
}