using iakademi47_proje.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using PagedList.Core;//Nuget paketinden indirdim.
using XAct;
using System.Net;
using Microsoft.CodeAnalysis.Differencing;
using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;
using System.Diagnostics;

namespace iakademi47_proje.Controllers
{
    public class HomeController : Controller
    {


        //slider-> products status kolon değeri =1
        //yeni ürünler - products addate kolonu

        //günün ürünü -> products status kolon değeri = 2
        //ÖZELLER - products status kolon değeri = 3
        //EN ÇOK İNDİRİM ->products DİSCOUNT kolon
        //ÖNE ÇIKANLAR ->PRODUCTS Highlighted kolonu
        //ÇOK SATANLAR -> products Topseller kolonu
        //Yıldızlı ürünler - products status kolon değeri= 4
        //fırsat ürünler - products status kolon değeri = 5

        //related = buna bakanlar bunlarada baktı 
        //dikkat çeken ürün = products status kolon değeri = 6 notable

        //SİPARİŞLERİM -> Order Tablosu
        //SEPETİM      -> Çerezler - Cookie 
        //https://colorhunt.co/

        Cls_Product p = new Cls_Product();//new diyerek nesne oluşturduk.
        MainPageModel mpm = new MainPageModel();
        iakademi47Context context = new iakademi47Context();
        Cls_Order cls_order = new Cls_Order();

        int mainpageCount = 0;
        public HomeController()
        {
            this.mainpageCount = context.Settings.FirstOrDefault(s => s.SettingID == 1).MainPageCount;
        }

        public IActionResult Index()
        {
            mpm.SliderProducts = p.ProductSelect("slider", "", 0);
            mpm.NewProducts = p.ProductSelect("new", "", 0);//new=ana sayfa, ""=alt sayfa, 0=AJAX için parametre
                                                            //mpm.Productofday = p.ProductDetails();	
            mpm.SpecialProducts = p.ProductSelect("Special", "", 0);//StatusID = 3 olanlar.özel ürünler
            mpm.DiscountedProducts = p.ProductSelect("Discounted", "", 0);//indirimli ürünler
            mpm.HighlightedProducts = p.ProductSelect("Highlighted", "", 0);//öne çıkan ürünler
            mpm.TopSellerProducts = p.ProductSelect("Topseller", "", 0);//çok satan ürünler
            mpm.StarProducts = p.ProductSelect("Star", "", 0);//yıldızlı ürünler
            mpm.FeaturedProducts = p.ProductSelect("Featured", "", 0);//fırsat ürünleri
            mpm.NotableProducts = p.ProductSelect("Notable", "", 0); //Dikkat çeken ürünler
            return View(mpm);

        }


        public IActionResult NewProducts()
        {
            mpm.NewProducts = p.ProductSelect("new", "new", 0);//new =ana sayfa için,2.new alt sayfa için
            return View(mpm);
        }


        public PartialViewResult _partialNewProducts(string nextpagenumber)
        {
            int pagenumber = Convert.ToInt32(nextpagenumber);
            mpm.NewProducts = p.ProductSelect("new", "new", pagenumber);
            return PartialView(mpm);

        }



        public IActionResult SpecialProducts()
        {
            mpm.SpecialProducts = p.ProductSelect("Special", "Special", 0);//new =ana sayfa için,2.new alt sayfa için
            return View(mpm);
        }


        public PartialViewResult _partialSpecialProducts(string nextpagenumber)
        {
            int pagenumber = Convert.ToInt32(nextpagenumber);
            mpm.SpecialProducts = p.ProductSelect("Special", "Special", pagenumber);
            return PartialView(mpm);

        }


        public IActionResult DiscountedProducts()
        {
            mpm.DiscountedProducts = p.ProductSelect("Discounted", "Discounted", 0);//new =ana sayfa için,2.new alt sayfa için
            return View(mpm);
        }


        public PartialViewResult _partialDiscountedProducts(string nextpagenumber)
        {
            int pagenumber = Convert.ToInt32(nextpagenumber);
            mpm.DiscountedProducts = p.ProductSelect("Discounted", "Discounted", pagenumber);
            return PartialView(mpm);

        }




        public IActionResult HighlightedProducts()
        {
            mpm.HighlightedProducts = p.ProductSelect("Highlighted", "Highlighted", 0);//new =ana sayfa için,2.new alt sayfa için
            return View(mpm);
        }


        public PartialViewResult _partialHighlightedProducts(string nextpagenumber)
        {
            int pagenumber = Convert.ToInt32(nextpagenumber);
            mpm.HighlightedProducts = p.ProductSelect("Highlighted", "Highlighted", pagenumber);
            return PartialView(mpm);

        }


        public IActionResult TopSellerProducts(int page = 1, int pageSize = 8)
        {
            PagedList<Product> model = new PagedList<Product>(context.Products.OrderByDescending(p => p.Topseller), page, pageSize);
            return View("TopSellerProducts", model);
        }

        public IActionResult CartProcess(int id)
        {
            //sepetim
            //10=1&
            //20=1&
            //30=4
            //ürün detayına tıklanınca,sepete eklenince HighLigted kolonunun değerini 1 arttıracagız
            Cls_Product.Highlighted_Increase(id);

            cls_order.ProductID = id;
            cls_order.Quantity = 1;

            var cookieOptions = new CookieOptions();
            //tarayıcıdan okuma
            var cookie = Request.Cookies["sepetim"];
            if (cookie == null)
            {
                //sepet boş
                cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddDays(7); //7 günlük çerez süresi
                cookieOptions.Path = "/";
                cls_order.MyCart = "";
                cls_order.AddToMyCart(id.ToString());
                Response.Cookies.Append("sepetim", cls_order.MyCart, cookieOptions);
                HttpContext.Session.SetString("Message", "Ürün Sepetinize Eklendi");
                TempData["Message"] = "Ürün Sepetinize Eklendi.";
            }
            else
            {
                //sepet doluysa
                cls_order.MyCart = cookie; //tarayıcıdan aldım,property ye koydum
                if (cls_order.AddToMyCart(id.ToString()) == false)
                {
                    //sepet dolu,aynı ürün değil
                    Response.Cookies.Append("sepetim", cls_order.MyCart, cookieOptions);
                    cookieOptions.Expires = DateTime.Now.AddDays(7);
                    HttpContext.Session.SetString("Message", "Ürün Sepetinize Eklendi");
                    TempData["Message"] = "Ürün Sepetinize Eklendi.";
                    //o an hangi sayfadaysam sayfanın linkini yakalıyorum
                }
                else
                {
                    HttpContext.Session.SetString("Message", "Ürün Sepetinize Zaten Var");
                    TempData["Message"] = "Ürün Sepetinize Zaten Var.";
                }
            }
            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);
        }

        public IActionResult Cart()
        {
            List<Cls_Order> MyCart;

            //silme butonu ile geldim
            if (HttpContext.Request.Query["scid"].ToString() != "")
            {
                int scid = Convert.ToInt32(HttpContext.Request.Query["scid"].ToString());
                cls_order.MyCart = Request.Cookies["sepetim"];
                cls_order.DeleteFromMyCart(scid.ToString());

                var cookieOptions = new CookieOptions();
                Response.Cookies.Append("sepetim", cls_order.MyCart, cookieOptions);
                cookieOptions.Expires = DateTime.Now.AddDays(7);
                TempData["Message"] = "Ürün Sepetinizden Silindi";
                MyCart = cls_order.SelectMyCart();
                ViewBag.MyCart = MyCart;
                ViewBag.MyCart_Table_Details = MyCart;
            }
            else
            {
                //sag üst kösedeki Sepet sayfama git butonu ile geldim
                var cookie = Request.Cookies["sepetim"];
                if (cookie == null)
                {
                    //sepette hic ürün olmayabilir
                    var cookieOptions = new CookieOptions();
                    cls_order.MyCart = "";
                    MyCart = cls_order.SelectMyCart();
                    ViewBag.MyCart = MyCart;
                    ViewBag.MyCart_Table_Details = MyCart;
                }
                else
                {
                    //sepette ürün var
                    var cookieOptions = new CookieOptions();
                    cls_order.MyCart = Request.Cookies["sepetim"];
                    MyCart = cls_order.SelectMyCart();
                    ViewBag.MyCart = MyCart;
                    ViewBag.MyCart_Table_Details = MyCart;
                }
            }

            if (MyCart.Count == 0)
            {
                ViewBag.MyCart = null;
            }

            return View();
        }

        [HttpGet]
        public IActionResult Order()
        {
            if (HttpContext.Session.GetString("Email") != null)//login girişi yaparak buraya gelmiş müşteri email null değilse
            {
                User? user = Cls_User.SelectMemberInfo(HttpContext.Session.GetString("Email").ToString());
                return View(user);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        //metod overload = Aynı parametre sırasıyla , aynı isimli metodu yazamayız
        //metod overload etmek için,parametre sırası farklı olmalı
        [HttpPost]
        public IActionResult Order(IFormCollection frm)//buraya string aaaaa IFormCollection yerine yazabiliriz
        {
            // string? kredikartno = Request.Form["kredikartno"]; IFormCollection olmadan yerine string a yazarsak böyle yakalıyoruz.
            string? kredikartno = frm["kredikartno"].ToString();//IFormCollection zorunlu
            string? kredikartay = frm["kredikartay"].ToString();
            string? kredikartyil = frm["kredikartyil"].ToString();
            string? kredikartcvs = frm["kredikartcvs"].ToString();

            //Bankaya git,eger true gelirse (onay alırsa kredi kartı bankadan)
            //order tablosuna kayıt atacağız.
            //digital-planet e (e-fatura)bilgilerini gönder

            //payu,iyzico

            string? txt_tckimlikno = frm["txt_tckimlikno"].ToString();
            string? txt_vergino = frm["txt_vergino"].ToString();

            if (txt_tckimlikno != "")
            {
                WebServiseController.tckimlikno = txt_tckimlikno;
                //fatura bilgilerini digital-planet şirketine gönderirsiniz(xml formatında)
                //sizin e-faturanınızı oluşturacak
            }
            else
            {
                WebServiseController.vergino = txt_vergino;
            }

            //.Net,payu,WindowsForm ile yazılmış bir proje gönderiyor
            NameValueCollection data = new NameValueCollection();

            string url = "https://www.ozgurdogru.com/backref";

            data.Add("BACK_REF", url);
            data.Add("CC_CVV", kredikartcvs);
            data.Add("CC_NUMBER", kredikartno);
            data.Add("EXP_MONTH", kredikartay);
            data.Add("EXP_YEAR", kredikartyil);


            var deger = "";
            foreach (var item in data)
            {
                var value = item as string;
                var ByteCount = Encoding.UTF8.GetBytes(data.Get(value));
                deger += ByteCount + data.Get(value);
            }

            var signatureKey = "payu üyeliğinde size verilen SECRET_KEY burada olacak";
            var hash = HashWithSignature(deger, signatureKey);

            data.Add("ORDER_HASH", hash);

            var x = POSTFormPAYU("https://secure.payu.com.tr/order/...", data);
            //sanal Kredi kartı
            if (x.Contains("<STATUS>SUCCESS</STATUS>") && x.Contains("<RETURN_CODE>3DS_ENROLLED</RETURN_CODE>"))
            {
                //SANAL KREDİ KART OK
            }
            else
            {
                //gerçek kredi kartı

            }


            return RedirectToAction("backref");
        }

        public static string HashWithSignature(string deger, string signatureKey)
        {
            return "";
        }

        public IActionResult backref()
        {
            Confirm_Order();
            return RedirectToAction("Confirm");
        }

        public static string? OrderGroupGUID = "";
        /*
         1- aaaa-  19092023193345   sipariş numarası- sipariş adı- sipariş kodu
         2 bbbb-   19092023193345   sipariş numarası- sipariş adı- sipariş kodu
         3 cccc-   19092023193345   sipariş numarası- sipariş adı- sipariş kodu
        75 eeee-   19092023223345
        76 ffff-   19092023223345
         */

        public IActionResult Confirm_Order()
        {
            //10=1&20=1&30=3 verilen siparişler tarayıcıda duruyor
            //sipariş tablosuna kaydet
            //cookie sepetini sileceğiz
            //e-fatura oluşturucaz,e-faturayı oluşturan xml metodu çağırıcaz
            var cookieOptions = new CookieOptions();           
            var cookie = Request.Cookies["sepetim"]; //daha sonra 10=1&20=1&30=3 verilen siparişler sepetim e geldi

            if (cookie != null)
            {
                cls_order.MyCart = cookie; //tarayıcıdaki sepet bilgilerini property e koydum
               OrderGroupGUID = cls_order.WriteToOrderTable(HttpContext.Session.GetString("Email"));
                cookieOptions.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Delete("sepetim");

                bool result = Cls_User.SendSms(OrderGroupGUID);
                if (result == false)
                {
                    //Orders tablosunda sms kolonuna false değeri basılır,admin panele menü yapılır
                    //Orders tablosunda sms kolonu = false olan siparişleri getir
                }

                //Cls_User.SendEmail(OrderGroupGUID);

                //1) ozgurdogru.com sitesinden kredi kart bilgileri alınır
                //2) Bu bilgiler payu ya da iyzico sitesine gönderilir(bankayla haberleşiyor bu firmalar
                //3) kredi kart bilgileri bankaya geldiğinde, banka kullanıcıya sms onayı gönderir
                //4) banka backref metoduna geri dönüş yapar,baanka kredi karta OK vermişse,
                // siz bir sms firmasıyla anlaştınız(netgsm),SendSms metodu müşteriye siparişiniz onaylandı sms gönderir
                // biz sms içeriklerini sms firmasına göndereceğiz(xml formatında),o firma sms gönderme işlemi yapacak
                // digital planet müşteriye e-fatura gönderir.Ben digital planet şirketine siparişin içeriğini gönderririm(xml formatında)

            }  

            return RedirectToAction("Confirm");
        }

        public IActionResult Confirm()
        {
            ViewBag.OrderGroupGUID = OrderGroupGUID;
            return View();
        }

        public static string POSTFormPAYU(string url, NameValueCollection data)
        {
            return "";
        }



        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (Cls_User.loginEmailControl(user) == false)
            {
                bool answer = Cls_User.AddUser(user);

                if (answer)
                {
                    TempData["Message"] = "Kaydedildi.";
                    return RedirectToAction("Login");
                }
                TempData["Message"] = "Hata.Tekrar deneyiniz.";
            }
            else
            {
                TempData["Message"] = "Bu Email Zaten mevcut.Başka Deneyiniz.";
            }
            return View();

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            string answer = Cls_User.MemberControl(user);

            if (answer == "error")
            {
                TempData["Message"] = "Hata.Email ve/veya Şifre Yanlış.";
            }
            else if (answer == "admin") 
            {
                //email şifre doğru,admin
                HttpContext.Session.SetString("Admin", "Admin");
                HttpContext.Session.SetString("Email", answer);
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                //email şifre doğru,sitemizden alışveriş yapan normal kullanıcı
                HttpContext.Session.SetString("Email", answer);
                return RedirectToAction("Index", "Home");

            }

            return View();
        }


        public IActionResult MyOrders()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                List<vw_MyOrders> orders = cls_order.SelectMyOrders(HttpContext.Session.GetString("Email").ToString());
                return View(orders);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult DetailedSearch() 
        {
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Suppliers = context.Suppliers.ToList();
            return View();
        }

        public IActionResult DProducts(int CategoryID, string[] SupplierID, string price, string IsInStock)
        {
            price = price.Replace(" ", "");
            string[] PriceArray = price.Split('-');
            string startprice = PriceArray[0];
            string endprice = PriceArray[1];

            string sign = ">";
            if (IsInStock == "0")
            {
                sign = ">=";
            }

            int count = 0;
            string suppliervalue = ""; //1,2,4
            for (int i = 0; i < SupplierID.Length; i++)
            {
                if (count == 0)
                {
                    suppliervalue = "SupplierID =" + SupplierID[i];
                    count++;
                }
                else
                {
                    suppliervalue += " or SupplierID =" + SupplierID[i];
                }
            }

            string query = "select * from Products where  CategoryID = " + CategoryID + " and (" + suppliervalue + ") and (UnitPrice > " + startprice + " and UnitPrice < " + endprice + ") and Stock " + sign + " 0 order by ProductName";

            ViewBag.Products = p.SelectProductsByDetails(query);
            return View();
        }


        public IActionResult ContactUs()
        {
            return View();
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Remove("Admin");
            return RedirectToAction("Index");
        }



        public IActionResult CategoryPage(int id)
        {
            List<Product> products = p.ProductSelectWithCategoryID(id);
            return View(products);
        }


        public IActionResult SupplierPage(int id)
        {
            List<Product> products = p.ProductSelectWithSupplierID(id);
            return View(products);
        }



        public IActionResult AboutUs()
        {
            return View();
        }






            public IActionResult Details(int id)
            {
                 // ORM = ado.net, entityframeworkcore,linq,dapper
                //efcore
                //mpm.ProductDetails = context.Products.FirstOrDefault(p => p.ProductID == id);

                //select * from Products where ProductID = id  ado.net , dapper

                //linq  - 4 nolu ürünün bütün kolon (sütün) bilgileri elimde
                mpm.ProductDetails = (from p in context.Products where p.ProductID == id select p).FirstOrDefault();

                //linq
                mpm.CategoryName = (from p in context.Products
                                    join c in context.Categories
                                  on p.CategoryID equals c.CategoryID
                                    where p.ProductID == id
                                    select c.CategoryName).FirstOrDefault();

                //linq
                mpm.BrandName = (from p in context.Products
                                 join s in context.Suppliers
                               on p.SupplierID equals s.SupplierID
                                 where p.ProductID == id
                                 select s.BrandName).FirstOrDefault();

                //select * from Products where Related = 2 and ProductID != 4
                mpm.RelatedProducts = context.Products.Where(p => p.Related == mpm.ProductDetails!.Related && p.ProductID != id).ToList();

                Cls_Product.Highlighted_Increase(id);

                return View(mpm);
            }


        public PartialViewResult gettingProducts(string id)
        {
            id = id.ToUpper(new System.Globalization.CultureInfo("tr-TR"));
            List<sp_arama> ulist = Cls_Product.gettingSearchProducts(id);
            string json = JsonConvert.SerializeObject(ulist);
            var response = JsonConvert.DeserializeObject<List<Search>>(json);
            return PartialView(response);
        }


        public IActionResult PharmacyOnDuty()
        {
            /*
            https://openfiles.izmir.bel.tr/111324/docs/ibbapi-WebServisKullanimDokumani_1.0.pdf
            https://openapi.izmir.bel.tr/api/ibb/cbs/wizmirnetnoktalari
            https://acikveri.bizizmir.com/dataset/kablosuz-internet-baglanti-noktalari/resource/982875a4-2bb6-4178-8ee2-3f07641156bb
            https://acikveri.bizizmir.com/dataset/izban-banliyo-hareket-saatleri
            */

            //https://openapi.izmir.bel.tr/api/ibb/nobetcieczaneler

            string json = new WebClient().DownloadString("https://openapi.izmir.bel.tr/api/ibb/nobetcieczaneler");

            var pharmacy = JsonConvert.DeserializeObject<List<Pharmacy>>(json);

            return View(pharmacy);
        }


        public IActionResult ArtAndCulture()
        {
            //https://openapi.izmir.bel.tr/api/ibb/kultursanat/etkinlikler

            string json = new WebClient().DownloadString("https://openapi.izmir.bel.tr/api/ibb/kultursanat/etkinlikler");

            var activite = JsonConvert.DeserializeObject<List<Activite>>(json);

            return View(activite);
        }

    }

}