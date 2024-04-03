using İsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace İsTakipSistemiMVC.Controllers
{
    // calisan controller class'ında bir class oluşturuyoruz.
    public class IsDurum
    {
        public string isBaslik { get; set; }
        public string isAciklama { get; set; }
        // ? işareti sayesinde boş geçilebilir oluyor.
        public DateTime? iletilenTarih { get; set; }
        public DateTime? yapılanTarih { get; set; }
        public string durumAd { get; set; }
        public string isYorum { get; set; }
    }
    public class CalisanController : Controller
    {
        İsTakipDBEntities entity = new İsTakipDBEntities();
        // GET: Calisan
        public ActionResult Index()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 2)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();

                ViewBag.birimAd = birim.birimAd;
                int PersonelId = Convert.ToInt32(Session["PersonelId"]);

                var isler=(from i in entity.Isler where i.isPersonelId== PersonelId && i.isOkunma==false orderby i.iletilenTarih descending select i ).ToList();

                ViewBag.isler = isler;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        // OKUNDU ALANINA TIKLANDIĞINDA OKUNDU OLARAK DEĞİŞTİRME YERİMİZ.
        [HttpPost]
        public ActionResult Index(int isId)
        {
            var tekIs =(from i in entity.Isler where i.isId==isId select i).FirstOrDefault();
            tekIs.isOkunma = true;
            entity.SaveChanges();

            return RedirectToAction("Yap", "Calisan");

        }
        public ActionResult yap()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 2)
            {
                int PersonelId = Convert.ToInt32(Session["PersonelId"]);

                var isler = (from i in entity.Isler where i.isPersonelId == PersonelId && i.isDurumId == 1 select i).ToList().OrderByDescending(i => i.iletilenTarih);

                ViewBag.isler = isler;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public ActionResult Yap(int isId, string isYorum)
        {
            var tekIs = (from i in entity.Isler where i.isId == isId select i).FirstOrDefault();
            // eğer işyorum boş gelmiş ise doldurmak istiyoruz.
            if (isYorum == "") isYorum = "Kullanıcı Yorum Yapmadı";
            // yapılan işin tarihini değiştirme şuanki tarih olması için gerekli kod ve işin durumunu değiştirme işlemi
            tekIs.yapilanTarih = DateTime.Now;
            tekIs.isDurumId = 2;
            tekIs.isYorum = isYorum;

            // güncelleme işleminin veritabanına etklilemesi için gereken kod
            entity.SaveChanges();

            // yapma işlemi gerçekleştirdikten sonra ana sayfaya yönlendiriyoruz.
            return RedirectToAction("Index", "Calisan");

        }
        // _LayoutCalisan Takip actionunu oluşturuyoruz.
        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 2)
            {
                int PersonelId = Convert.ToInt32(Session["PersonelId"]);
                // burada iş durumunu kontrol etmemize gerek yok
                var isler = (from i in entity.Isler join d in entity.Durumlar on i.isDurumId equals d.durumId where i.isPersonelId == PersonelId select i).ToList().OrderByDescending(i => i.iletilenTarih);

                IsDurumModel model = new IsDurumModel();

                List<IsDurum> List = new List<IsDurum>();

                foreach (var i in isler)
                {
                    IsDurum isDurum = new IsDurum();

                    isDurum.isBaslik = i.isBaslik;
                    isDurum.isAciklama = i.isAciklama;
                    isDurum.iletilenTarih = i.iletilenTarih;
                    isDurum.yapılanTarih = i.yapilanTarih;
                    isDurum.durumAd = i.Durumlar.durumAd;
                    isDurum.isYorum = i.isYorum;
                    // is durum nesnemizi listeye aktarıyoruz.
                    List.Add(isDurum);
                }
                // iş durumlar içerisine listeyi aktarıyoruz modelimiz hazır 
                model.isDurumlar = List;


                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}