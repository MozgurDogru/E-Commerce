using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace iakademi47_proje.Models
{
    public class Cls_Supplier
    {
        iakademi47Context context = new iakademi47Context();
        public async Task<List<Supplier>> SupplierSelect()
        {
            List<Supplier> suppliers = await context.Suppliers.ToListAsync();
            return suppliers;
        }

        public static string SupplierInsert(Supplier supplier)
        {
            //metod static olduğu için
            using (iakademi47Context context = new iakademi47Context())
            {
                try
                {
                    Supplier sup = context.Suppliers.FirstOrDefault(c => c.BrandName.ToLower() == supplier.BrandName.ToLower());

                    if (sup == null)
                    {
                        context.Add(supplier);
                        context.SaveChanges();
                        return "Başarılı";
                    }
                    else
                    {
                        return "Zaten var";
                    }

                }
                catch (Exception)
                {
                    return "Başarısız!";

                }
            }
        }

        public async Task<Supplier?> SupplierDetails(int? id)
        {
            Supplier? supplier = await context.Suppliers.FindAsync(id);
            return supplier;
        }

        public static bool SupplierUpdate(Supplier supplier)
        {

            using (iakademi47Context context = new iakademi47Context())
            {
                try
                {
                    context.Update(supplier);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }



        }


		public static bool SupplierDelete(int id)
		{
			try
			{
				using (iakademi47Context context = new iakademi47Context())
				{
					Supplier supplier = context.Suppliers.FirstOrDefault(s => s.SupplierID == id);
					supplier.Active = false;

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