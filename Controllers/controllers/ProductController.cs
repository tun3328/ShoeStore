using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EshopperMCV.Models;

namespace EshopperMCV.Controllers
{
    public class ProductController : Controller
    {
        private EShopperDBContext dbContext = new EShopperDBContext();
        // GET: Product

        //Hiển thị danh sách sản phẩm theo Category
        public ActionResult Index(int? id) 
        {
            // Nếu không có category nào
            if (!dbContext.Categories.Any())
            {
                return View(new List<Product>());
            }

            // Nếu chưa chọn category → lấy cái đầu tiên
            if (id == null)
            {
                id = dbContext.Categories.Select(c => c.Id).First(); //Lấy Id của Category đầu tiên
            }
            //Lấy danh sách sản phẩm theo Category
            var products = dbContext.Products.Where(p => p.CatId == id).ToList(); //Lấy danh sách sản phẩm từ cơ sở dữ liệu theo CatId
            return View(products ?? new List<Product>()); //Trả về View với danh sách sản phẩm
        }

        //Hiển thị chi tiết một sản phẩm cụ thể. GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var product = dbContext.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        public ActionResult GetCategories() //Lấy danh sách Category để hiển thị ở Sidebar / Menu.
        {
            var categories = dbContext.Categories.ToList(); //Lấy danh sách Category từ cơ sở dữ liệu
            return PartialView("_Sidebar", dbContext.Categories.ToList()); //Trả về PartialView với danh sách Category
        }
    }
}