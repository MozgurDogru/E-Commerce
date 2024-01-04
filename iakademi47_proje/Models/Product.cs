using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iakademi47_proje.Models
{
	public class Product
	{
		[Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ProductID { get; set; }

		[Required]
		[StringLength(100)]
		[DisplayName("Ürün Adı")]
		public string? ProductName { get; set; }


		[DisplayName("Fiyat")]
		[Required]
		public decimal UnitPrice { get; set; }

		[DisplayName("Kategori")]
		public int CategoryID { get; set; }

		[DisplayName("Marka")]
		public int SupplierID { get; set; }

		[DisplayName("Stok")]
		public int Stock { get; set; }

		[DisplayName("İndirim")]
		public int Discount { get; set; }

		[DisplayName("Statü")]
		public int StatusID { get; set; }

		public DateTime AddDate { get; set; }

		[DisplayName("Anahtar Kelimeler")]
		public string? Keywords { get; set; }

		private int _Kdv { get; set; }
		public int Kdv {
			get { return _Kdv; }
			set { _Kdv =Math.Abs(value); }
		}

		public int Highlighted { get; set; }// öne çıkanlar


		public int Topseller { get; set; }//çok satanlar


		[DisplayName("Buna Bakanlar")]
		public int Related { get; set; }//Buna bakanlar bunada baktı


		[DisplayName("Notlar")]
		public string? Notes { get; set; }


		[DisplayName("Resim")]
		public string? PhotoPath  { get; set; }


		[DisplayName("Aktif")]
		public bool Active { get; set; }

	}
}
