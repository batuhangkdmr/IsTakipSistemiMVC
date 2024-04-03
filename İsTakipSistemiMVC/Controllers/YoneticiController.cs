using İsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace İsTakipSistemiMVC.Controllers
{
    public class YoneticiController : Controller
    {
        İsTakipDBEntities entity = new İsTakipDBEntities();
        // GET: Yonetici

        // Index adında ActionResult dönen bir method oluşturuyoruz
        public ActionResult Index()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();

                ViewBag.birimAd = birim.birimAd;
                // personelleri p olarak adlandırdık ardından join diyerek işler ile birleştiriyioruz ardından on diyerek birşelme işleminin nasıl olacağını söylüyoruz.  group by olduğu için bu personeller içerisinde işleri guruplamamız lazım bunun için de into isler diyoruz.
                // select new de bizim ihtiyacımız olan presonel ad soyad ve toplam iş 
                // guruplaya bilmemiz için Join den sonra İnto diyerek işleri gurupluyoruz
                // kısıtlamamız olduğu için where diyerek gerçekleştiriyoruz.
                var personeller = from p in entity.Personeller
                                  join i in entity.Isler on p.PersonelId equals i.isPersonelId into isler
                                  where p.personelBirimId == birimId && p.personelYetkiTurId != 1
                                  select new
                                  {
                                      personelAd = p.PersonelAdSoyad,
                                      isler = isler
                                  };
                List<ToplamIs> list = new List<ToplamIs>();

                // burada foreach döngüsü oluşturuyoruz ve gelen personeller için döngüyü kurmak
                foreach (var personel in personeller)
                {
                    ToplamIs toplamIs = new ToplamIs();
                    toplamIs.PersonelAdSoyad = personel.personelAd;

                    if (personel.isler.Count() == 0)
                    {
                        toplamIs.toplamIs = 0;

                    }
                    else
                    { //kaç işi olduğunı toplamamız için int toplam değişkenini oluşturuyoruz.
                        int toplam = 0;
                        foreach (var item in personel.isler)
                        {// yapılan iş sayısını öğrenmek için yapılanTarih'e bakmak zorunayız.
                            if (item.yapilanTarih != null)
                            {
                                toplam++;
                            }
                        }
                        // foreach döngüsünden çıktıktan sonra bu işlem uygulanıyor.
                        toplamIs.toplamIs = toplam;
                    }
                    //list'e toplam iş nesnesini aktarıyoruz.
                    list.Add(toplamIs);
                }

                // sıralama yaptığımız yer en son çoktan aza doğru sıralanacak
                IEnumerable<ToplamIs> siraliListe = new List<ToplamIs>();
                siraliListe = list.OrderByDescending(i => i.toplamIs);

                return View(siraliListe);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        // Ata adında ActironResult dönen bir method oluşturuyoruz
        public ActionResult Ata()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var calisanlar = (from p in entity.Personeller where p.personelBirimId == birimId && p.personelYetkiTurId == 2 select p).ToList();

                ViewBag.personeller = calisanlar;

                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();

                ViewBag.birimAd = birim.birimAd;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        // iş ata bölgesini düzenliyoruz.
        [HttpPost]
        public ActionResult Ata(FormCollection formCollection)
        {
            string isBaslik = formCollection["isBaslik"];
            string isAciklama = formCollection["isAciklama"];
            int secilenParsonelId = Convert.ToInt32(formCollection["selectPer"]);

            // Models den isTakipModel içerisinde Isler.cs içerisinde bununan public partial class Isler class'ından bir nesne üretip propertyleri değiştirme alanımız. Isler class'ından yeniIs adında bir nesne ürettik.

            Isler yeniIs = new Isler();
            // yeniIs'in alanlarını belirliyoruz.
            yeniIs.isBaslik = isBaslik;
            yeniIs.isAciklama = isAciklama;
            yeniIs.isPersonelId = secilenParsonelId;
            yeniIs.iletilenTarih = DateTime.Now;
            yeniIs.isDurumId = 1;
            yeniIs.isOkunma = false;

            // Add metodu ile yeniIs'i ekliyoruz. 
            entity.Isler.Add(yeniIs);
            // veri tabanını tetiklememiz için ise dememiz gerekecek.
            entity.SaveChanges();


            return RedirectToAction("Takip", "Yonetici");
        }
        // bir takip action'ı oluşturuyoruz Views den _LayoutYonetici içerisinde iş takibe tıklandığında Takip action'ın çalışıcağını söylemiştik

        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

                var calisanlar = (from p in entity.Personeller where p.personelBirimId == birimId && p.personelYetkiTurId == 2 && p.aktiflik == true select p).ToList();

                ViewBag.personeller = calisanlar;

                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();

                ViewBag.birimAd = birim.birimAd;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        // httppost olarak attiribute olarak veriyoruz post işleminde çalışacağını belitiyoruz.  public actionresult dönen takip Action'ını oluşturuyoruz. 
        // selectPer içerisinde personel Id si tutulacak bize dönderilecek personel Id sine göre personelin bütün bilgilerini getirmek amacımız.
        // 1 tane veri dönecek bu yüzden FirstOrDefault kullanıyoruz.
        // ViewBag   public ActionResult Takip buradaki method'dan View'e veri göndermek @@@@@@@@@@@@@@@@@@@@
        [HttpPost]
        public ActionResult Takip(int selectPer)
        {
            var secilenPersonel = (from p in entity.Personeller where p.PersonelId == selectPer select p).FirstOrDefault();
            // Takip method'undan Listele method'una veri göndermek.
            TempData["secilen"] = secilenPersonel;

            return RedirectToAction("Listele", "Yonetici");
        }

        [HttpGet]
        public ActionResult Listele()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                // personellet tipinde secilenpersonel değişkeni oluşturuyoruz ordan da tempdatanın verisini personellere dönüştürüyoruz.
                Personeller secilenPersonel = (Personeller)TempData["secilen"];
                // Orderby yaptıgımızda azdan çoğa sıralıyacak@@ 
                // OrderByDescending çoktan aza sıralayacak.@@


                // Link üzerinden manuel bir şekilde Yonetici/Listele sayfasına erişmek istediğimizde bizi Takip sayfasına yönlendirmesini sağlamak için TRY catch kodu yazıyoruz.
                try
                {
                    var isler = (from i in entity.Isler where i.isPersonelId == secilenPersonel.PersonelId select i).ToList().OrderByDescending(i => i.iletilenTarih);
                    // isler bilgisini viewbag'e aktarabiliriz.
                    ViewBag.isler = isler;
                    ViewBag.personel = secilenPersonel;
                    ViewBag.isSayisi = isler.Count();

                    return View();
                }
                catch (Exception)
                {

                    return RedirectToAction("Takip", "Yonetici");
                }


            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }
        public ActionResult AyinElemani()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int simdikiYil = DateTime.Now.Year;

                List<int> yillar = new List<int>();

                // şimdiki yıldan 2024 yılına azalan  döngü oluşturuyoruz.
                for (int i = simdikiYil; i >= 2024; i--)
                {
                    yillar.Add(i);
                }
                // yıllar'ı Viev'e gönderiyoruz.
                ViewBag.yillar = yillar;
                ViewBag.ayinElemani = null;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public ActionResult AyinElemani(int aylar, int yillar)
        {
            int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

            var personeller = entity.Personeller
                                    .Where(p => p.personelBirimId == birimId)
                                    .Where(p => p.personelYetkiTurId != 1);

            // Seçilen ayın ilk günü
            DateTime baslangicTarih = new DateTime(yillar, aylar, 1);

            // Seçilen ayın son günü
            DateTime bitisTarih = baslangicTarih.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

            var isler = entity.Isler
                             .Where(i => i.yapilanTarih >= baslangicTarih)
                             .Where(i => i.yapilanTarih <= bitisTarih);
            var groupJoin = personeller.GroupJoin(isler, p => p.PersonelId, i => i.isPersonelId, (p, group) => new
            {
                sonucIsler = group,
                personelAd = p.PersonelAdSoyad
            });
            List<ToplamIs> list = new List<ToplamIs>();
            foreach (var personel in groupJoin)
            {
                ToplamIs toplamIs = new ToplamIs();
                toplamIs.PersonelAdSoyad = personel.personelAd;

                if (personel.sonucIsler.Count() == 0)
                {  // toplamIs nesnemizin içerisinedeki toplamIs property'sine 0 'ı atıyoruz. 
                    toplamIs.toplamIs = 0;

                }
                else
                {
                    int toplam = 0;
                    foreach (var item in personel.sonucIsler)
                    {
                        if (item.yapilanTarih != null)
                        {
                            toplam++;
                        }
                    }
                    toplamIs.toplamIs = toplam;
                }
                list.Add(toplamIs);
            }
            IEnumerable<ToplamIs> siraliListe = new List<ToplamIs>();
            siraliListe = list.OrderByDescending(i => i.toplamIs);
            ViewBag.ayinElemani=siraliListe.FirstOrDefault();
            int simdikiYil = DateTime.Now.Year;

            List<int> sonucYillar = new List<int>();

            // şimdiki yıldan 2024 yılına azalan  döngü oluşturuyoruz.
            for (int i = simdikiYil; i >= 2024; i--)
            {
                sonucYillar.Add(i)  ;
            }
            // yıllar'ı Viev'e gönderiyoruz.
            ViewBag.yillar = sonucYillar;

            return View();
        }




    }
}