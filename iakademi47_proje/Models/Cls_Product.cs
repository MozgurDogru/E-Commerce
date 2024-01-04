using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace iakademi47_proje.Models
{
    public class Cls_Product
    {


        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public string? PhotoPath { get; set; }

        iakademi47Context context = new iakademi47Context();
        public async Task<List<Product>> productSelect()
        {
            List<Product> products = await context.Products.ToListAsync();
            return products;
        }


        public static string? ProductInsert(Product product)
        {
            //metod static oldugu icin
            using (iakademi47Context context = new iakademi47Context())
            {
                try
                {
                    Product pro = context.Products.FirstOrDefault(c => c.ProductName!.ToLower() == product.ProductName!.ToLower())!;

                    if (pro == null)
                    {
                        product.AddDate = DateTime.Now;
                        context.Add(product);
                        context.SaveChanges();
                        return "başarılı";
                    }
                    else
                    {
                        return "zaten var";
                    }
                }
                catch (Exception)
                {
                    return "başarısız";
                }
            }
        }


        public List<Product> ProductSelect(string mainPageName, string subPageName, int pageNumber)
        {
            List<Product> products;
            if (mainPageName == "slider")
            {
                products = context.Products.Where(p => p.StatusID == 1).Take(8).ToList();
            }
            else if (mainPageName == "new")
            {
                if (subPageName == "")
                {
                    //Home/Index
                    products = context.Products.OrderByDescending(p => p.AddDate).Take(8).ToList();
                    //select top 8 * from Products order by AddDate desc-- ado.net ile
                    // Bu da entity Framework.Core sorgusu 
                    //Proje açılışı = .NetCore (Önceki versiiyon :.Net Framework)
                }
                else
                {
                    if (pageNumber == 0)
                    {
                        //alt sayfa
                        products = context.Products.OrderByDescending(p => p.AddDate).Take(4).ToList();

                    }
                    else
                    {
                        //alt sayfada  AJAX
                        products = context.Products.OrderByDescending(p => p.AddDate).Skip(4 * pageNumber).Take(4).ToList();

                    }

                }


            }
            else if (mainPageName == "Special")
            {
                if (subPageName == "")
                {
                    //home/index	

                    products = context.Products.Where(p => p.StatusID == 3).Take(8).ToList();

                }
                else
                {
                    if (pageNumber == 0)
                    {
                        //alt sayfa
                        products = context.Products.Where(p => p.StatusID == 3).Take(4).ToList();

                    }
                    else
                    {
                        //alt sayfada  AJAX
                        products = context.Products.Where(p => p.StatusID == 3).Skip(4 * pageNumber).Take(4).ToList();

                    }

                }


            }
            else if (mainPageName == "Discounted")
            {
                if (subPageName == "")
                {
                    //Home/Index
                    products = context.Products.OrderByDescending(p => p.Discount).Take(8).ToList();

                }
                else
                {
                    if (pageNumber == 0)
                    {
                        //alt sayfa
                        products = context.Products.OrderByDescending(p => p.Discount).Take(4).ToList();

                    }
                    else
                    {
                        //alt sayfada  AJAX
                        products = context.Products.OrderByDescending(p => p.Discount).Skip(4 * pageNumber).Take(4).ToList();

                    }

                }


            }
            else if (mainPageName == "Highlighted")
            {
                if (subPageName == "")
                {
                    //Home/Index
                    products = context.Products.OrderByDescending(p => p.Highlighted).Take(8).ToList();

                }
                else
                {
                    if (pageNumber == 0)
                    {
                        //alt sayfa
                        products = context.Products.OrderByDescending(p => p.Highlighted).Take(4).ToList();

                    }
                    else
                    {
                        //alt sayfada  AJAX
                        products = context.Products.OrderByDescending(p => p.Highlighted).Skip(4 * pageNumber).Take(4).ToList();

                    }

                }
            }




            else if (mainPageName == "Topseller")
            {

                products = context.Products.OrderByDescending(p => p.Topseller).Take(8).ToList();
            }

            else if (mainPageName == "Star")
            {

                products = context.Products.Where(p => p.StatusID == 4).OrderBy(p => p.ProductName).Take(8).ToList();
            }

            else if (mainPageName == "Featured")
            {

                products = context.Products.Where(p => p.StatusID == 5).OrderBy(p => p.ProductName).Take(8).ToList();
            }



            else
            {
                products = context.Products.Where(p => p.StatusID == 6).OrderBy(p => p.ProductName).Take(8).ToList();
            }
            return products;
        }

        public Product ProductDetails()
        {
            Product? product = context.Products.FirstOrDefault(p => p.StatusID == 2);
            return product!;
        }

        public static void Highlighted_Increase(int id)
        {
            using (iakademi47Context context = new iakademi47Context())
            {

                Product? product = context.Products.FirstOrDefault(p => p.ProductID == id);

                product!.Highlighted += 1;
                context.Update(product);
                context.SaveChanges();
            }
        }


        public List<Cls_Product> SelectProductsByDetails(string query)
        {
            List<Cls_Product> products = new List<Cls_Product>();
            SqlConnection sqlConnection = Connection.ServerConnect;
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            sqlConnection.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                Cls_Product product = new Cls_Product();
                product.ProductID = Convert.ToInt32(sqlDataReader["ProductID"]);
                product.ProductName = sqlDataReader["ProductName"].ToString();
                product.UnitPrice = Convert.ToDecimal(sqlDataReader["UnitPrice"]);
                product.PhotoPath = sqlDataReader["PhotoPath"].ToString();
                products.Add(product);
            }
            return products;
        }


        public List<Product> ProductSelectWithCategoryID(int id)
        {
            List<Product> products = context.Products.Where(p => p.CategoryID == id).OrderBy(p => p.ProductName).ToList();
            return products;
        }


        public List<Product> ProductSelectWithSupplierID(int id)
        {
            List<Product> products = context.Products.Where(p => p.SupplierID == id).OrderBy(p => p.ProductName).ToList();
            return products;
        }

        public static List<sp_arama> gettingSearchProducts(string id)
        {
           
            using (iakademi47Context context = new iakademi47Context())    
                
            {
                var products = context.sp_Aramas.FromSql($"sp_arama {id}").ToList();
                return products;
            }

        }


        public async Task<Product> ProductDetails(int? id)
        {
            Product? product = await context.Products.FindAsync(id);
            return product;
        }


        public static bool ProductUpdate(Product product)
        {
            try
            {
                using (iakademi47Context context = new iakademi47Context()) 
                {
                    context.Update(product);
                    context.SaveChanges();
                    return true;

                }
            }
            catch (Exception)
            {

                return false;
            }
        }


        public static bool ProductDelete(int id)
        {
            try
            {
                using (iakademi47Context context = new iakademi47Context())
                {
                    Product product = context.Products.FirstOrDefault(c => c.ProductID == id);
                    product.Active = false;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }



}
