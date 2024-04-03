using İsTakipSistemiMVC.Filters;
using İsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace İsTakipSistemiMVC.Controllers
{
    public class SifreKontrolController : Controller
    {
        İsTakipDBEntities entity = new İsTakipDBEntities();
        // GET: SifreKontrol
        public ActionResult Index()
        {
            // LoginControllerden Session["PersonelId"] alıp giriş yapan kullanıcının personelId sini yakalamak için.
            int personelId = Convert.ToInt32(Session["PersonelId"]);

            if (personelId == 0) return RedirectToAction("Index", "Login");

            var personel = (from p in entity.Personeller where p.PersonelId == personelId select p).FirstOrDefault();
            //  ilk açıldığında görünmemesi için null veriyoruz.
            ViewBag.mesaj = null;
            ViewBag.yetkiTurId = null;
            ViewBag.sitil = null;


            return View(personel);
        }

        [HttpPost, ActFilter("Parola Değiştirildi")]
        public ActionResult Index(int personelId, string eskiParola, string yeniParola, string yeniParolaKontrol)
        {
            var personel = (from p in entity.Personeller where p.PersonelId == personelId select p).FirstOrDefault();

            if (eskiParola != personel.PersonelParola)
            {
                ViewBag.mesaj = "Eski parolanızı yanlış girdiniz";
                ViewBag.sitil = "alert alert-danger";

                return View(personel);
            }

            if (yeniParola != yeniParolaKontrol)
            {
                ViewBag.mesaj = "Yeni parola ve yeni parola tekrarı tekrarı eşleşmedi";
                ViewBag.sitil = "alert alert-danger";

                return View(personel);
            }

            if (yeniParola.Length < 6 || yeniParola.Length > 15)
            {
                ViewBag.mesaj = "Yeni parola en az 6 karakter en çok 15 karakter olmalıdır";
                ViewBag.sitil = "alert alert-danger";

                return View(personel); 
            }

            personel.PersonelParola = yeniParola;
            personel.yeniPersonel = false;
            entity.SaveChanges();

            TempData["bilgi"] = personel.personelKullanıcıAd;

            ViewBag.mesaj = "Parolanız başarıyla değiştirildi. Anasayfaya yönlendiriliyorsunuz.";
            ViewBag.sitil = "alert alert-success";
            ViewBag.yetkiTurId = personel.personelYetkiTurId;

            return View(personel);
        }
    }
}