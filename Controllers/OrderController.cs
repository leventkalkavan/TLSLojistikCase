using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagement.Entities;
using StockManagement.Models.ViewModels.OrderModels;
using StockManagement.Services.Interfaces;

namespace StockManagement.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<Customer> _userManager;
        private readonly ICustomerAddressService _addressService;
        private readonly IStockService _stockService;
        private readonly IOrderDetailService _orderDetailService;

        public OrderController(
            IOrderService orderService,
            UserManager<Customer> userManager,
            ICustomerAddressService addressService,
            IStockService stockService, IOrderDetailService orderDetailService)
        {
            _orderService = orderService;
            _userManager = userManager;
            _addressService = addressService;
            _stockService = stockService;
            _orderDetailService = orderDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var customerSummaries = await _orderService.GetCustomerOrderSummaryAsync();
            return View(customerSummaries);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerOrders(string customerId)
        {
            var orderDetails = await _orderDetailService.GetCustomerOrderDetailsAsync(customerId);
            return PartialView("_CustomerOrdersModal", orderDetails);
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrder()
        {
            var model = new OrderCreateViewModel();
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                model.CustomerId = currentUser.Id;
                
                var addresses = await _addressService.GetAddressesByCustomerIdAsync(currentUser.Id);
                ViewBag.DeliveryAddresses = new SelectList(addresses, "Id", "Address");
                ViewBag.InvoiceAddresses = new SelectList(addresses, "Id", "Address");
            }
            
            var stocks = await _stockService.GetAllStocksAsync();
            ViewBag.Stocks = stocks.Select(s => new { Value = s.Id.ToString(), Text = s.Name, Price = s.Price }).ToList();
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    var addresses = await _addressService.GetAddressesByCustomerIdAsync(currentUser.Id);
                    ViewBag.DeliveryAddresses = new SelectList(addresses, "Id", "Address");
                    ViewBag.InvoiceAddresses = new SelectList(addresses, "Id", "Address");
                }
                
                var stocks = await _stockService.GetAllStocksAsync();
                ViewBag.Stocks = stocks.Select(s => new { Value = s.Id.ToString(), Text = s.Name, Price = s.Price }).ToList();
                
                return View(model);
            }

            await _orderService.CreateAsync(model);
            return RedirectToAction("GetAllOrders");
        }
        
        [HttpGet]
        public async Task<IActionResult> ProductCustomers()
        {
            var productCustomers = await _orderDetailService.GetProductCustomersAsync();
            return PartialView("_ProductCustomersModal", productCustomers);
        }

        [HttpGet]
        public async Task<IActionResult> MultiQuantityOrders()
        {
            var multiQuantityOrders = await _orderDetailService.GetMultiQuantityOrdersAsync();
            return PartialView("_MultiQuantityOrdersModal", multiQuantityOrders);
        }

        [HttpGet]
        public async Task<IActionResult> DifferentAddressOrders()
        {
            var differentAddressOrders = await _orderDetailService.GetDifferentAddressOrdersAsync();
            return PartialView("_DifferentAddressOrdersModal", differentAddressOrders);
        }

        [HttpGet]
        public async Task<IActionResult> TLSOrders()
        {
            var tlsOrders = await _orderDetailService.GetTLSOrdersAsync();
            return PartialView("_TLSOrdersModal", tlsOrders);
        }

        [HttpGet]
        public async Task<IActionResult> IstanbulOrders()
        {
            var istanbulOrders = await _orderDetailService.GetIstanbulOrdersAsync();
            return PartialView("_IstanbulOrdersModal", istanbulOrders);
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            
            var myOrders = await _orderDetailService.GetCustomerOrderDetailsAsync(currentUser.Id);
            return PartialView("_MyOrdersModal", myOrders);
        }
    }
}