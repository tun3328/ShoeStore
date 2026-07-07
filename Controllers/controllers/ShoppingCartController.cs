using EshopperMCV.Models;
using EShopperMCV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EshopperMCV.Controllers
{
    public class ShoppingCartController : Controller //Giỏ hàng
    {
        private EShopperDBContext dbContext = new EShopperDBContext(); //Kết nối cơ sở dữ liệu
        private string strCart = "Cart"; //Tên session giỏ hàng

        // GET: ShoppingCart
        public ActionResult Index() //Hiển thị giỏ hàng
        {
            return View(); //Trả về View hiển thị giỏ hàng
        }

        public ActionResult OrderNow(int? Id)
        {
            if (Id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var ListCart = GetCart();
            var product = dbContext.Products.Find(Id);

            int check = ListCart.FindIndex(x => x.Product.ProId == Id);
            if (check == -1)
                ListCart.Add(new Cart(product, 1));
            else
                ListCart[check].Quantity++;

            return RedirectToAction("Index");
        }

        private int IsExistingCheck(int? Id)
        {
            var ListCart = GetCart();
            for (int i = 0; i < ListCart.Count; i++)
            {
                if (ListCart[i].Product.ProId == Id)
                    return i;
            }
            return -1;
        }


        public ActionResult RemoveItem(int? Id)
        {
            if (Id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.Product.ProId == Id);

            if (item != null)
                cart.Remove(item);

            if (!cart.Any())
                Session.Remove(strCart);

            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult UpdateCart(FormCollection field)
        {
            var cart = GetCart();
            string[] quantities = field.GetValues("quantity");

            for (int i = 0; i < cart.Count; i++)
            {
                cart[i].Quantity = Convert.ToInt32(quantities[i]);
            }

            return RedirectToAction("Index");
        }


        public ActionResult ClearCart() //Xóa toàn bộ giỏ hàng
        {
            Session[strCart] = null; //Cập nhật session giỏ hàng về null
            return RedirectToAction("Index"); //Chuyển hướng về trang hiển thị giỏ hàng
        }

        public ActionResult CheckOut()
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index");

            return View();
        }


        [HttpPost]
        public ActionResult ProcessOrder(FormCollection field) //Xử lý đơn hàng sau khi khách hàng thanh toán
        {
            List<Cart> ListCart = (List<Cart>)Session[strCart]; //Lấy giỏ hàng từ session

            // 1. Save the order into Order table
            var order = new EshopperMCV.Models.Order() //Tạo đơn hàng ghi xuống bảng Order (lưu thông tin người mua ) 
            {
                CustomerName = field["cusName"], //Lấy thông tin từ form
                CustomerPhone = field["cusPhone"],
                CustomerEmail = field["cusEmail"],
                CustomerAddress = field["cusAddress"],
                OrderDate = DateTime.Now, //Ngày đặt hàng là ngày hiện tại
                PaymentType = "Cash", //Phương thức thanh toán là tiền mặt
                Status = "Processing" //Trạng thái đơn hàng là đang xử lý
            };

            dbContext.Orders.Add(order); //Thêm đơn hàng vào DbSet Orders
            dbContext.SaveChanges(); //cập nhật để lấy OrderId

            // 2. Save the order detail into OrderDetail table
            foreach (Cart cart in ListCart) // lưu chi tiết đơn hàng
            {
                OrderDetail orderDetail = new OrderDetail() //Tạo chi tiết đơn hàng ghi xuống bảng OrderDetail
                {
                    OrderId = order.OrderId, //Lấy mã đơn hàng từ đơn hàng vừa tạo
                    ProductId = cart.Product.ProId, //Lấy mã sản phẩm từ giỏ hàng
                    Quantity = Convert.ToInt32(cart.Quantity), //Lấy số lượng sản phẩm từ giỏ hàng
                    Price = Convert.ToDouble(cart.Product.ProPrice) //Lấy giá sản phẩm từ giỏ hàng
                };

                dbContext.OrderDetails.Add(orderDetail); //Thêm chi tiết đơn hàng vào DbSet OrderDetails
                dbContext.SaveChanges(); //Lưu thay đổi xuống cơ sở dữ liệu
            }

            // 3. Remove shopping cart session
            Session.Remove(strCart); //Xóa session giỏ hàng

            return View("OrderSuccess"); //Trả về View hiển thị thông báo đặt hàng thành công
        }
        private List<Cart> GetCart() 
        {
            if (Session[strCart] == null)
            {
                Session[strCart] = new List<Cart>();
            }
            return (List<Cart>)Session[strCart];
        }

    }
}
