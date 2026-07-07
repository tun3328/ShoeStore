using EshopperMCV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;


namespace EshopperMCV.Controllers
{
    public class BlogController : Controller
    {
    // Danh sách comment tạm (demo)
    private static List<CommentViewModel> comments = new List<CommentViewModel>();
    
        // GET: Blog
        public ActionResult Index() // Hiển thị trang danh sách blog
        {
            return View();
        }
        [HttpGet]
        public ActionResult Single() // Hiển thị trang blog chi tiết
        {
            return View(comments); // Truyền danh sách comment vào view
        }
        [HttpPost]
        public ActionResult PostComment(CommentViewModel model) // Nhận dữ liệu comment từ form
        {
            if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrEmpty(model.Message)) // Kiểm tra dữ liệu hợp lệ
            {
                model.CreatedAt = DateTime.Now;
                comments.Add(model);
            }

            return RedirectToAction("Single");
        }
    }

}
