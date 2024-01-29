using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FormsApp.Models{
    public class Product{

        [Display(Name="Urun Id")] // dataAnnotations ile productıd propunun sayfada gorunen adı Urun Id olsun dedik
        [BindNever] //bind olmasın dedik yani post ederken id bilgisini almaz
        public int ProductId { get; set; }

        [Required(ErrorMessage ="İsim alanı gerekli")] // name alanını zorunlu yapar(validation hata kontrolleri)
        [Display(Name="Urun Adı")]
        public string Name { get; set; } = null!; // null! diyerek name alanını bos birakmiyacagimiza dair soz vermis gibi oluruz bunu yapma nedenimiz ise ornegin homecontroller da name.ToLover() komutunu kullandık burda name null olsa tolover metodu calısmaz ondan uyarı verir bu uyarıyı vermesin diye yaptık

        [Required]
        [Display(Name="Fiyat")]
        [Range(0,100000)] // fiyat 0-100.000 arasında olsun dedik 
        public decimal? Price { get; set; }

        [Display(Name="Resim")]
        public string? Image { get; set; }= string.Empty;

        public bool IsActive { get; set; }

        [Required]
        [Display(Name="Kategori")]
        public int? CategoryId { get; set; } // veri tabanındaki fk gibi suanlik veri tabanıyla yapmadık ama
    }

}
