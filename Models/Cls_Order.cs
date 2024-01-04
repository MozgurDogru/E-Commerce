using Microsoft.CodeAnalysis.Differencing;
using XAct.Users;

namespace iakademi47_proje.Models
{
    public class Cls_Order 
    {
        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public string? MyCart { get; set; }

        public decimal UnitPrice { get; set; }

        public string? ProductName { get; set; }

        public int  Kdv { get; set; }

        public string? PhotoPath { get; set; }//10=1 & 20=1 & 30=1

        iakademi47Context context = new iakademi47Context();


        //sepete ekle
        public bool AddToMyCart(string id)
        {
            bool exists = false;

            if (MyCart == "")
            {
                MyCart = id + "=1";
            }
            else
            {
                string[] MyCartArray = MyCart.Split('&');
                //10=1
                //20=1
                //30=1
                for (int i = 0; i < MyCartArray.Length; i++)
                {
                    string[] MyCartArrayLoop = MyCartArray[i].Split('=');
                    //10
                    //1
                    if (MyCartArrayLoop[0] == id)
                    {
                        //bu ürün zaten sepette var
                        exists = true;
                    }
                }

                if (exists == false)
                {
                    MyCart = MyCart + "&" + id.ToString() + "=1";
                }
            }
            return exists;
        }
        //sepetten sil
        public void DeleteFromMyCart(string scid)
        {
            string[] MyCartArray = MyCart!.Split('&');
            string NewMyCart = "";
            int count = 1;

            for (int i = 0; i < MyCartArray.Length; i++)
            {
                string[] MyCartArrayLoop = MyCartArray[i].Split('=');
                if (count==1)
                {
                    //yeni sepetin içine silinmeyecek ürünleri koyacağım
                    //yeni sepete ilk ürünü koyuyorum
                    if (MyCartArrayLoop[0]!=scid)
                    {
                        NewMyCart += MyCartArrayLoop[0] + "=" + Convert.ToInt32(MyCartArrayLoop[1]);
                        count++;
                    }
                }
                else
                {
                    //count 1 den büyük,yeni sepette en az bir ürün var
                    if (MyCartArrayLoop[0] != scid)
                    {
                        NewMyCart += "&" + MyCartArrayLoop[0] + "=" + Convert.ToInt32(MyCartArrayLoop[1]);
                        count++;
                    }

                }
                
            }
             MyCart = NewMyCart;
        }

        //sağ üst köşedeki sepet sayfasına git tıklanınca
        public List<Cls_Order> SelectMyCart()
        {
            List<Cls_Order> list= new List<Cls_Order>();
            string[] MyCartArray = MyCart!.Split('&');

            if (MyCartArray[0]!="")
            {
                for (int i = 0; i < MyCartArray.Length; i++)
                {
                    string[] MyCartArrayLoop = MyCartArray[i].Split('=');
                    int sepetid = Convert.ToInt32(MyCartArrayLoop[0]);

                    Product? product = context.Products.FirstOrDefault(p => p.ProductID == sepetid);

                    Cls_Order pr= new Cls_Order();
                    pr.ProductID = product!.ProductID;//database den aldım,property ye gönderdim
                    pr.Quantity = Convert.ToInt32(MyCartArrayLoop[1]);
                    pr.UnitPrice = product.UnitPrice;
                    pr.ProductName = product.ProductName;
                    pr.Kdv = product.Kdv;
                    pr.PhotoPath = product.PhotoPath;
                    list.Add(pr);//Her bir ürünün bütün kolon bilgileri listeye ekleniyor
                }
            }
            return list;
        }

        //Cls_Order cls_order = new Cls_Order();
        //OrderGroupGUID = cls_order.WriteToOrderTable(HttpContext.Session.Getstring("Email"));
        //Formül
        //1- public(private,protected)
        //private =(sadece bu class içindeki metodlar erişebilir)
        //protected = (sadece kalıtsallık almış bu class içindeki metodlar erişebilir)

        //2- static var ya da yok()
        //new diye nesne oluşturup,oluşturduğumuz nesne üzerinden çağırdıysam static DEĞİL,
        //eğer new diye nesne oluşturmadan yani Classİsmi.Metodİsmi(Cls_Order.WriteToOrderTable) static olacak.

        //3- geriye dönüş tipi
        //metod içinden bir sonuç döndürüyorsak onun tipi (string,int,float,decimal vs...)bir şey döndürüyorsam return ZORUNLU
        //metod içinden bir sonuç döndürmüyorsak dönüş tipi void olacak,metod içinde return olmaz,boş döner!

        //4- metod ismi

        //5- varsa parantez içinde parametreler(id int,Email, vs...)

        //6- Scope aç kapa,metod geriye bir şey döndürüyorsa return

        public string WriteToOrderTable(string Email)
        {
            string OrderGroupGUID = DateTime.Now.ToString().Replace(":", "").Replace(".", "").Replace(" ", "")
             .Replace(",", "");

            DateTime OrderDate = DateTime.Now;

            try
            {
                List<Cls_Order> orders = SelectMyCart();
                foreach (var item in orders)
                {
                    Order order = new Order();

                    order.OrderDate = OrderDate;
                    order.OrderGroupGUID = OrderGroupGUID;
                    order.UserID = context.Users.FirstOrDefault(u => u.Email == Email).UserID;
                    order.ProductID = item.ProductID;
                    order.Quantity = item.Quantity;
                   

                    context.Orders.Add(order);
                    context.SaveChanges();
                }

            }
            catch (Exception)
            {
                OrderGroupGUID = "Error";
            }
            return OrderGroupGUID;
        }

        public List<vw_MyOrders> SelectMyOrders(string Email)
        {
          // select UserID from Users where Email = 'ozgurr_dogru2@hotmail.com'
            int UserID = context.Users.FirstOrDefault(u => u.Email == Email).UserID;

            List<vw_MyOrders> myOrders = context.vw_MyOrders.Where(o => o.UserID == UserID).ToList();

            return myOrders;
        }
    }

}













