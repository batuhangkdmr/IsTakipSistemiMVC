using İsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using İsTakipSistemiMVC.Filters;

namespace İsTakipSistemiMVC.Controllers
{
    public class SistemYoneticisiController : Controller
    {
        İsTakipDBEntities entity = new İsTakipDBEntities();
        // GET: SistemYoneticisi
        [AuthFilter(1003)]
        // Chart 
        public ActionResult Index()
        {
            var birimler=(from b in entity.Birimler where b.aktiflik==true select b).ToList();

            string labelBirim = "[";
            foreach (var birim in birimler)
            {
                labelBirim += "'" + birim.birimAd + "',";
            }

            labelBirim += "]";

            ViewBag.labelBirim = labelBirim;

            List<int> islerToplam=new List<int>();

            foreach (var birim in birimler)
            {
                int toplam = 0;

                var personeller = (from p in entity.Personeller where p.personelBirimId==birim.birimId && p.aktiflik==true select p).ToList();

                foreach (var personel in personeller)
                {
                    var isler = (from i in entity.Isler where i.isPersonelId == personel.PersonelId select i).ToList();

                    toplam += isler.Count();
                }
                islerToplam.Add(toplam);
            }
            // AKTİF VERİLERİ GÖRMEK İÇİN Chart kodlar sayesinde Yönetici hesabı ile görüyoruz
            string dataIs = "[";

            foreach(var i in islerToplam)
            {
                dataIs += "'" + i + "',";
            }
            dataIs += "]";

            ViewBag.dataIs = dataIs;

           return View();
        }

        public ActionResult Birim()
        {
            // index içerisinde de yetki tür bilgisine göre kotrol sağlıyoruz. sadece yetki tür ıd si 1003 olanlarının görmesini istiyoruz
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1003)
            {
                var birimler=(from b in entity.Birimler where b.aktiflik==true select b).ToList();
                return View(birimler);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            } 

        }

        public ActionResult Olustur()
        {
            // index içerisinde de yetki tür bilgisine göre kotrol sağlıyoruz. sadece yetki tür ıd si 1003 olanlarının görmesini istiyoruz
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1003)
            {
                
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost,ActFilter("Yeni Birim Eklendi")]
        public ActionResult Olustur(string birimAd)
        {
            Birimler yeniBirim = new Birimler();
            // using System.Globalization;  en yukarıya bu kodu ve burayada string yeniAd Culturainfo verdiğimiz için EKLE'nilen yeniAd'lar baş harfleri BÜYÜMESİNİ sağlıyoruz.
            string yeniAd =CultureInfo.CurrentCulture.TextInfo.ToTitleCase(birimAd);
            yeniBirim.birimAd = yeniAd;
            yeniBirim.aktiflik = true;

            entity.Birimler.Add(yeniBirim);
            entity.SaveChanges();

            TempData["bilgi"] = yeniBirim.birimAd;

            return RedirectToAction("Birim");

        }

        public ActionResult Guncelle(int id)
        {
            // index içerisinde de yetki tür bilgisine göre kotrol sağlıyoruz. sadece yetki tür ıd si 1003 olanlarının görmesini istiyoruz
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1003)
            {
                var birim=(from b in entity.Birimler where b.birimId==id select b).FirstOrDefault();

                return View(birim);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost,ActFilter("Birim Güncellendi")]
        public ActionResult Guncelle(FormCollection fc)
        {
            int birimId = Convert.ToInt32(fc["birimId"]);
            string yeniAd = fc["birimAd"];
            var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();
            birim.birimAd=CultureInfo.CurrentCulture.TextInfo.ToTitleCase(yeniAd);
            entity.SaveChanges();

            TempData["bilgi"]=birim.birimAd;

            return RedirectToAction("Birim");

        }
        [ActFilter("Birim Silindi")]
        // parametre olarak int id veriyoruz
        public ActionResult Sil(int id)
        {
            // index içerisinde de yetki tür bilgisine göre kotrol sağlıyoruz. sadece yetki tür ıd si 1003 olanlarının görmesini istiyoruz
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1003)
            {
                var birim = (from b in entity.Birimler where b.birimId == id select b).FirstOrDefault();
                // Remove silmek için kullanılır.
                // entity.SaveChanges ile de veri tabanına bu işlemi gönderiyoruz.
                birim.aktiflik = false;
                entity.SaveChanges();

                TempData["bilgi"] = birim.birimAd;

                return RedirectToAction("Birim");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [AuthFilter(1003)]
        public ActionResult Loglar()
        {
            var loglar = (from l in entity.Loglar orderby l.tarih descending select l).ToList();
            return View(loglar);
        }
    }
}