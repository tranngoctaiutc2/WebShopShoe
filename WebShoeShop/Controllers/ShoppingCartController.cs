using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebShoeShop.Models;
using WebShoeShop.Models.EF;

namespace WebShoeShop.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ShoppingCartController()
        {
        }

        public ShoppingCartController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        
        // GET: ShoppingCart
        public ActionResult Index()
        {

            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }
        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var itemOrder = db.Orders.FirstOrDefault(x => x.Code == orderCode);
                        if (itemOrder != null)
                        {
                            itemOrder.Status = 2;//đã thanh toán    
                            db.Orders.Attach(itemOrder);
                            db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
							ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
						}
                        //Thanh toan thanh cong
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý. Xin vui lòng thử lại";
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                    //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                    //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                    //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                    //ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                    //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
                }
            }
            return View();
        }

        
        public ActionResult CheckOut()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }
        
        public ActionResult Partial_Item_ThanhToan()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }
        
        public ActionResult Partial_Item_Cart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }
        
        public ActionResult ShowCount()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return Json(new { Count = cart.Items.Count }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Partial_CheckOut()
        {
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            if (user != null)
            {
                ViewBag.User = user;
            }
            return PartialView();
        }
        
        public ActionResult CheckOutSuccess()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(OrderViewModel req)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            if (ModelState.IsValid)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null)
                {
                    Models.EF.Order order = new Models.EF.Order();
                    order.CustomerName = req.CustomerName;
                    order.Phone = req.Phone;
                    order.Address = req.Address;
                    order.Email = req.Email;
                    order.Status = 1; // chưa duyệt
                    order.StatusPayMent = 1; // Chưa thanh toán

                    cart.Items.ForEach(x => order.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        Price = x.Price,
                        Size = x.Size
                    }));
                    order.Quantity = cart.GetTotalQuantity();
                    if (req.TypeShip == 1)
                    {
                        order.TotalAmount = cart.Items.Sum(x => (x.Price * x.Quantity));
                    }
                    else if (req.TypeShip == 2)
                    {
                        order.TotalAmount = cart.Items.Sum(x => (x.Price * x.Quantity) + 70000);
                    }
                    order.TypePayment = req.TypePayment;
                    order.TypeShip = req.TypeShip;
                    order.CreatedDate = DateTime.Now;
                    order.ModifiedDate = DateTime.Now;
                    order.CreatedBy = req.Phone;
                    if (User.Identity.IsAuthenticated)
                        order.CustomerId = User.Identity.GetUserId();
                    Random rd = new Random();
                    order.Code = "DH" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
                    //order.E = req.CustomerName;
                    db.Orders.Add(order);
                    db.SaveChanges();
                    //send mail cho khachs hang
                    var strSanPham = "";
                    var thanhtien = decimal.Zero;
                    var TongTien = decimal.Zero;
                    var strPrice = "";
                    var strProductName = "";
                    foreach (var sp in cart.Items)
                    {
                        strSanPham += "<tr>";
                        /*    strSanPham += "<td>" + sp.ProductName + "</td>";*/
                        /*         strSanPham += "<td>" + WebShoeShop.Common.Common.FormatNumber(sp.Price,0) + "</td>";*/
                        /* strSanPham += "<td>" + sp.Quantity + "</td>";*/
                        /*strSanPham += "<td>" + WebShoeShop.Common.Common.FormatNumber(sp.TotalPrice, 0) + "</td>";*/
                        strSanPham += "<td style=\"border-bottom:1px solid #e8e8e8; border-collapse:collapse; padding:10px;\">" + sp.ProductName + "</td>";
                        strSanPham +=  "<td style=\"color:#000; font-family:&#39;Roboto&#39;, Arial, Helvetica, sans-serif; border-bottom:1px solid #e8e8e8; border-collapse:collapse; font-size:13px; font-weight:normal; letter-spacing:0.5px; line-height:1.5; text-align:center; padding:10px; margin:0 0 0;\">" 
                            + WebShoeShop.Common.Common.FormatNumber(sp.Price, 0) + "</td>";
               

                        strSanPham += "<td style=\"color:#000; font-family:&#39;Roboto&#39;, Arial, Helvetica, sans-serif; border-bottom:1px solid #e8e8e8; border-collapse:collapse; font-size:13px; font-weight:normal; letter-spacing:0.5px; line-height:1.5; text-align:center; padding:10px; margin:0 0 0;\">"
                            + sp.Quantity + "</td>";
                      
                        /*strSanPham += "<td>" + sp.Size + "</td>";*/
                       strSanPham +=  "<td style =\"color:#000; font-family:&#39;Roboto&#39;, Arial, Helvetica, sans-serif; border-bottom:1px solid #e8e8e8; border-collapse:collapse; font-size:13px; font-weight:500; letter-spacing:0.5px; line-height:1.5; text-align:center; padding:10px; margin:0 0 0;\">" 
                            + WebShoeShop.Common.Common.FormatNumber(sp.TotalPrice, 0) + "</td>";
                       
                        strSanPham += "</tr>";
                        thanhtien += sp.Price * sp.Quantity;
                        var product = db.Products.Find(sp.ProductId);
                        if (product != null)
                        {
                            product.Quantity -= sp.Quantity;
                            db.Entry(product).State = EntityState.Modified;
                        }
                        strPrice = WebShoeShop.Common.Common.FormatNumber(sp.TotalPrice, 0);
                        strProductName = sp.ProductName;
                    }
                    db.SaveChanges();
                    if (req.TypeShip == 1)
                    {
                        TongTien = thanhtien;
                    }
                    else if (req.TypeShip == 2)
                    {
                        TongTien = thanhtien + 70000;
                    }
                                  
                        string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/invoice-1.html"));
                    contentCustomer = contentCustomer.Replace("{{MaDon}}", order.Code);
                    contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                    contentCustomer = contentCustomer.Replace("{{Gia}}", strPrice);
                    contentCustomer = contentCustomer.Replace("{{TenSanPham}}", strProductName);
                    contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                    contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.CustomerName);
                    contentCustomer = contentCustomer.Replace("{{Phone}}", order.Phone);
                    contentCustomer = contentCustomer.Replace("{{Email}}", req.Email);
                    contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.Address);
                    contentCustomer = contentCustomer.Replace("{{ThanhTien}}", WebShoeShop.Common.Common.FormatNumber(thanhtien, 0));
                    if (req.TypeShip == 1)
                    {
                        contentCustomer = contentCustomer.Replace("{{PhiVanChuyen}}", "0");
                    }
                    else if (req.TypeShip == 2)
                    {
                        contentCustomer = contentCustomer.Replace("{{PhiVanChuyen}}", WebShoeShop.Common.Common.FormatNumber(70000, 0));
                    }
                    contentCustomer = contentCustomer.Replace("{{TongTien}}", WebShoeShop.Common.Common.FormatNumber(TongTien, 0));
                    WebShoeShop.Common.Common.SendMail("Double 2T-2Q Store", "Đơn hàng #" + order.Code, contentCustomer.ToString(), req.Email);

                    string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                    contentAdmin = contentAdmin.Replace("{{MaDon}}", order.Code);
                    contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
                    contentAdmin = contentAdmin.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                    contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", order.CustomerName);
                    contentAdmin = contentAdmin.Replace("{{Phone}}", order.Phone);
                    contentAdmin = contentAdmin.Replace("{{Email}}", req.Email);
                    contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.Address);
                    contentAdmin = contentAdmin.Replace("{{ThanhTien}}", WebShoeShop.Common.Common.FormatNumber(thanhtien, 0));
                    if (req.TypeShip == 1)
                    {
                        contentAdmin = contentAdmin.Replace("{{PhiVanChuyen}}", "0");
                    }
                    else if (req.TypeShip == 2)
                    {
                        contentAdmin = contentAdmin.Replace("{{PhiVanChuyen}}", WebShoeShop.Common.Common.FormatNumber(70000, 0));
                    }
                    contentAdmin = contentAdmin.Replace("{{TongTien}}", WebShoeShop.Common.Common.FormatNumber(TongTien, 0));
                    WebShoeShop.Common.Common.SendMail("Double 2T-2Q Store", "Đơn hàng mới #" + order.Code, contentAdmin.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);
                    cart.ClearCart();
                    code = new { Success = true, Code = req.TypePayment, Url = "" };                    
                    if (req.TypePayment == 2)
                    {
                        var url = UrlPayment(req.TypePaymentVN, order.Code);
                        code = new { Success = true, Code = req.TypePayment, Url = url };
                    }
                }
            }
            return Json(code);
        }
        
        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {

            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            if (Request.IsAuthenticated == false)
            {
                code = new { Success = false, msg = "Yêu cầu đăng nhập mới được thêm vào giỏ hàng", code = -1, Count = 0 };
            }
            var db = new ApplicationDbContext();
            var checkProduct = db.Products.FirstOrDefault(x => x.Id == id);
            if (checkProduct != null)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart == null)
                {
                    cart = new ShoppingCart();
                }
                ShoppingCartItem item = new ShoppingCartItem
                {
                    ProductId = checkProduct.Id,
                    ProductName = checkProduct.Title,
                    CategoryName = checkProduct.ProductCategory.Title,
                    Alias = checkProduct.Alias,
                    Quantity = quantity
                };
                if (checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault) != null)
                {
                    item.ProductImg = checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault).Image;
                }
                item.Price = checkProduct.Price;
                if (checkProduct.PriceSale > 0)
                {
                    item.Price = (decimal)checkProduct.PriceSale;
                }
                item.TotalPrice = item.Quantity * item.Price;
                cart.AddToCart(item, quantity);
                Session["Cart"] = cart;
                code = new { Success = true, msg = "Thêm sản phẩm vào giỏ hàng thành công!", code = 1, Count = cart.Items.Count };
            }
            return Json(code);
        }
        
        [HttpPost]
        public ActionResult Update(int id, int quantity, int size)
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.UpdateQuantity(id, quantity, size);
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
        
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };

            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                var checkProduct = cart.Items.FirstOrDefault(x => x.ProductId == id);
                if (checkProduct != null)
                {
                    cart.Remove(id);
                    code = new { Success = true, msg = "", code = 1, Count = cart.Items.Count };
                }
            }
            return Json(code);
        }


        
        [HttpPost]
        public ActionResult DeleteAll()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.ClearCart();
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }


        #region Thanh toán vnpay
        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = db.Orders.FirstOrDefault(x => x.Code == orderCode);
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TotalAmount * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.Code);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
        #endregion

/*        #region Thanh Toan Paypal
        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/shoppingcart/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("FailureView");
            }
            //on successful payment, show success page to user.  
            return View("SuccessView");
        }
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        public Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            foreach (var item in cart.Items)
            {


                itemList.items.Add(new Item()
                {
                    name = item.ProductName,
                    currency = "USD",
                    price = Math.Round(item.TotalPrice / 25000).ToString(), // Định dạng giá trị tiền tệ với 2 chữ số thập phân
                    quantity = item.Quantity.ToString(),
                    sku = item.ProductId.ToString(),
                });
            }

            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = cart.GetTotalPrice().ToString()
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = cart.GetTotalPrice().ToString(), // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(), //Generate an Invoice No    
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }

        #endregion


        public ActionResult FailureView()
        {
            return View();
        }
        public ActionResult SuccessView()
        {
            return VnpayReturn();
        }



        public ActionResult PaypalReturn(string payerId, string paymentId)
        {
            // Kiểm tra tham số trả về
            if (string.IsNullOrEmpty(payerId) || string.IsNullOrEmpty(paymentId))
            {
                ViewBag.Message = "Invalid payment details.";
                return View();
            }

            // Lấy thông tin API context từ cấu hình
            var apiContext = GetAPIContext(); // Giả sử phương thức này đọc thông tin từ config

            try
            {
                // Thực hiện xác nhận thanh toán
                var payment = ExecutePayment(apiContext, payerId, paymentId);

                // Kiểm tra trạng thái thanh toán
                if (payment.state == "approved")
                {
                    // Lấy thông tin đơn hàng từ payment.transactions[0] (giả sử chỉ có 1 giao dịch)
                    var transaction = payment.transactions[0];
                    var orderCode = transaction.related_resources[0].sale.id; // Cần xác định chính xác cách lấy mã đơn hàng từ cấu trúc trả về của PayPal

                    // Tìm kiếm đơn hàng theo mã đơn hàng
                    var itemOrder = db.Orders.FirstOrDefault(x => x.Code == orderCode);

                    if (itemOrder != null)
                    {
                        // Cập nhật trạng thái đơn hàng thành công
                        itemOrder.Status = 2;
                        db.Orders.Attach(itemOrder);
                        db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        ViewBag.Message = "Thanh toán thành công. Số tiền thanh toán: " + transaction.amount.total;
                    }
                    else
                    {
                        ViewBag.Message = "Không tìm thấy đơn hàng.";
                    }
                }
                else
                {
                    ViewBag.Message = $"Thanh toán thất bại. Trạng thái: {payment.state}";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Có lỗi xảy ra: {ex.Message}";
            }

            return View();
        }*/
    }
}