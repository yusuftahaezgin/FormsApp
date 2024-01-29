using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FormsApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace FormsApp.Controllers;

public class HomeController : Controller
{
    public HomeController()
    {
        
    }

    [HttpGet]
    public IActionResult Index(string searchString, string category)
    {
        var products = Repository.Products;
        
        if(!String.IsNullOrEmpty(searchString)){
            ViewBag.SearchString = searchString;
            products = products.Where(p => p.Name.ToLower(new CultureInfo("en-US", false)).Contains(searchString)).ToList();
        }

        if(!String.IsNullOrEmpty(category) && category!="0"){
            products = products.Where(p => p.CategoryId == int.Parse(category)).ToList();
        }

        // ViewBag.Categories= new SelectList(Repository.Categories,"CategoryId","Name",category); //kategori listesini viewbag icine attık diger iki parametre ise sırasıyla CategorId value degeridir Name ise text kısmında yazacak degerdir
        var model = new ProductViewModel{
            Products = products,
            Categories = Repository.Categories,
            SelectedCategory = category
        };
        return View(model);
    }

    [HttpGet] // yazmasak da olurdu defaultu httpget zaten
    public IActionResult Create()
    {
        ViewBag.Categories= new SelectList(Repository.Categories,"CategoryId","Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product model, IFormFile imageFile)
    {

       
        var extension="";
        
        if(imageFile!= null) // yukarda allowedextensions daki uzantılar dışında uzantı girilirse hata versin dedik
        {
            var allowedExtensions= new [] {".jpg",".jpeg",".png"};

            extension = Path.GetExtension(imageFile.FileName); //uzantıyı alır ornegin abc.jpg deki .jpg kısmını alır

            if(!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("","Geçerli bir resim seçiniz.");
            }
        }


        if(ModelState.IsValid) //ModelState.IsValid in true olması demek forma girilen tum bilgiler validation kurallarına uygun demektir 
        {
                var randomFileName= string.Format($"{Guid.NewGuid().ToString()}{extension}"); // random dosya adı olusturur guidle radnom ad olusturur extension ise ustten aldıgımız uzantısını getirir dosyanın

                var path= Path.Combine(Directory.GetCurrentDirectory(),"wwroot/img",randomFileName); // post edilecek resimin nereye post edilecegini yani dosya yolunu tutan degisken

                using(var stream= new FileStream(path, FileMode.Create)) // dosyayı olusturma
                {
                    await imageFile!.CopyToAsync(stream); // buradaki ünlemi koyma nedenim null olabilir uyarısını kaldırmak için ! koyunca ben null olmayacagına dair programa komut vermiş oldum unlemi kaldırırsak null olabilir diye uyarı verecek
                }

            model.Image=randomFileName;
            model.ProductId = Repository.Products.Count+1;// yeni eklenen urunun id si urun sayısı +1 olsun dedik
            Repository.CreateProduct(model);
            return RedirectToAction("Index");
        }

        ViewBag.Categories= new SelectList(Repository.Categories,"CategoryId","Name"); // kategoriler de tekrar yuklensin dedik

        return View(model); // eger forma girilen bilgiler validation kurallarına takılırsa create formunu tekrar gonder dedik | (model) ise kullanıcının forma girdigi bilgiler sıfırlanmasın kalsın diye gonderdik
    }

    public IActionResult Edit(int? id)
    {
        if(id==null)
        {
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(p=>p.ProductId==id);

        if(entity==null)
        {
            return NotFound();
        }
        ViewBag.Categories= new SelectList(Repository.Categories,"CategoryId","Name");
        return View(entity);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Product model, IFormFile? imageFile)
    {

        if(id != model.ProductId)
        {
            return NotFound();
        }

        if(ModelState.IsValid)
        {
            
            if(imageFile!=null)
            {
                var extension = Path.GetExtension(imageFile.FileName);  
                var randomFileName= string.Format($"{Guid.NewGuid().ToString()}{extension}"); 
                var path= Path.Combine(Directory.GetCurrentDirectory(),"wwroot/img",randomFileName);

                using(var stream= new FileStream(path, FileMode.Create)) // dosyayı olusturma
                {
                    await imageFile.CopyToAsync(stream); // buradaki ünlemi koyma nedenim null olabilir uyarısını kaldırmak için ! koyunca ben null olmayacagına dair programa komut vermiş oldum unlemi kaldırırsak null olabilir diye uyarı verecek
                }
                model.Image=randomFileName;
            }
            Repository.EditProduct(model);
            return RedirectToAction("Index");
        }

        ViewBag.Categories= new SelectList(Repository.Categories,"CategoryId","Name");
        return View(model);

    }


    public IActionResult Delete(int? id)
    {
        if(id == null)
        {
            return NotFound();
        }

        var entity = Repository.Products.FirstOrDefault(p=>p.ProductId==id);

        if(entity==null)
        {
            return NotFound();
        }
        return View("DeleteConfirm",entity);
    }

    [HttpPost]
    public IActionResult Delete(int id,int ProductId)
    {
        if(id!=ProductId)
        {
            return NotFound();
        }

        var entity = Repository.Products.FirstOrDefault(p=>p.ProductId==id);

        if(entity==null)
        {
            return NotFound();
        }
        Repository.DeleteProduct(entity);
        return RedirectToAction("Index");

    }

    [HttpPost]
    public IActionResult EditProducts(List<Product> Products)
    {
        foreach(var product in Products)
        {
            Repository.EditIsActive(product);
        }
        return RedirectToAction("Index");
    }

}
