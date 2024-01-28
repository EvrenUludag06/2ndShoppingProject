using AlobarShopp.Data;
using AlobarShopp.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace AlobarShopp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        [BindProperty]
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }
        public CartController(UserManager<IdentityUser> userManager, IEmailSender emailSender, ApplicationDbContext db)
        {
            _db = db;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _db.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product)
            };
            foreach (var item in ShoppingCartViewModel.ListCart)
            {
                item.Price = item.Product.Price;
                ShoppingCartViewModel.OrderHeader.OrderTotal += (item.Count * item.Product.Price);
            }
            return View(ShoppingCartViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCartViewModel model)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartViewModel.ListCart = _db.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product);
            ShoppingCartViewModel.OrderHeader.OrderStatus = Diger.Durum_Beklemede;
            ShoppingCartViewModel.OrderHeader.ApplicationUserId = claim.Value;
            ShoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;
            _db.OrderHeaders.Add(ShoppingCartViewModel.OrderHeader);
            _db.SaveChanges();
            foreach (var item in ShoppingCartViewModel.ListCart)
            {
                item.Price = item.Product.Price;
                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = item.ProductId,
                    OrderId = ShoppingCartViewModel.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Count
                };
                ShoppingCartViewModel.OrderHeader.OrderTotal += item.Count * item.Product.Price;
                model.OrderHeader.OrderTotal += item.Count * item.Product.Price;
                _db.OrderDetails.Add(orderDetails);
            }
            var payment = PaymentProccess(model);
            _db.ShoppingCarts.RemoveRange(ShoppingCartViewModel.ListCart);
            _db.SaveChanges();
            HttpContext.Session.SetInt32(Diger.ssShoppingCart, 0);
            return RedirectToAction("SiparisTamam");
        }

        private Payment PaymentProccess(ShoppingCartViewModel model)
        {
            Options options = new Options();
            options.ApiKey = "sandbox-131fdeJq260WR1TrfG6glEHPxlqlrALf";
            options.SecretKey = "sandbox-xhd1ket04WnxpDFqS57iciYXZkXS2F0o";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(1111, 9999).ToString();
            request.Price = model.OrderHeader.OrderTotal.ToString();
            request.PaidPrice = model.OrderHeader.OrderTotal.ToString();
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            //PaymentCard paymentCard = new PaymentCard();
            //paymentCard.CardHolderName = "John Doe";
            //paymentCard.CardNumber = "5528790000000008";
            //paymentCard.ExpireMonth = "12";
            //paymentCard.ExpireYear = "2030";
            //paymentCard.Cvc = "123";
            //paymentCard.RegisterCard = 0;
            //request.PaymentCard = paymentCard;

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.OrderHeader.CartName;
            paymentCard.CardNumber = model.OrderHeader.CardNumber;
            paymentCard.ExpireMonth = model.OrderHeader.ExpiriationMonth;
            paymentCard.ExpireYear = model.OrderHeader.ExpiriationYear;
            paymentCard.Cvc = model.OrderHeader.Cvc;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = model.OrderHeader.Id.ToString();
            buyer.Name = model.OrderHeader.Name;
            buyer.Surname = model.OrderHeader.Surname;
            buyer.GsmNumber = model.OrderHeader.PhoneNumber;
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = model.OrderHeader.Adress;
            buyer.Ip = "85.34.78.112";
            buyer.City = model.OrderHeader.Sehir;
            buyer.Country = "Turkey";
            buyer.ZipCode = model.OrderHeader.Postakodu;
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            foreach (var item in _db.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product))
            {
                basketItems.Add(new BasketItem()
                {
                    Id = item.Id.ToString(),
                    Name = item.Product.Title,
                    Category1 = item.Product.CategoryId.ToString(),
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = (item.Price * item.Count).ToString()
                });
            }
            request.BasketItems = basketItems;

            return Payment.Create(request, options);
        }

        public IActionResult SiparisTamam()
        {
            return View();
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _db.ShoppingCarts.Where(i => i.ApplicationUserId == claim.Value).Include(i => i.Product)
            };
            ShoppingCartViewModel.OrderHeader.OrderTotal = 0;
            ShoppingCartViewModel.OrderHeader.ApplicationUser = _db.ApplicationUsers.FirstOrDefault(i => i.Id == claim.Value);
            foreach (var item in ShoppingCartViewModel.ListCart)
            {
                ShoppingCartViewModel.OrderHeader.OrderTotal += (item.Count * item.Product.Price);
            }
            return View(ShoppingCartViewModel);
        }
        public IActionResult Add(int cartId)
        {
            var cart = _db.ShoppingCarts.FirstOrDefault(i => i.Id == cartId);
            cart.Count += 1;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Decrease(int cartId)
        {
            var cart = _db.ShoppingCarts.FirstOrDefault(i => i.Id == cartId);
            if (cart.Count == 1)
            {
                var count = _db.ShoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
                _db.ShoppingCarts.Remove(cart);
                _db.SaveChanges();
                HttpContext.Session.SetInt32(Diger.ssShoppingCart, count - 1);
            }
            else
            {
                cart.Count -= 1;
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cart = _db.ShoppingCarts.FirstOrDefault(i => i.Id == cartId);
            var count = _db.ShoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
            _db.ShoppingCarts.Remove(cart);
            _db.SaveChanges();
            HttpContext.Session.SetInt32(Diger.ssShoppingCart, count - 1);

            return RedirectToAction(nameof(Index));
        }
    }
}