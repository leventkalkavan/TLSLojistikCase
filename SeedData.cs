using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockManagement.Context;
using StockManagement.Entities;

namespace StockManagement;

public class SeedData
{
    public static async Task EnsureSeedCustomerAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Customer>>();

        if (await context.Users.AnyAsync())
        {
            return;
        }

        var customers = new List<(string FullName, string Email, string Phone)>
        {
            ("Levent Kalkavan", "levent.kalkavan@mail.com", "+905001111001"),
            ("Mehmet Yılmaz", "mehmet.yilmaz@mail.com", "+905001111002"),
            ("Ayşe Kaya", "ayse.kaya@mail.com", "+905001111003"),
            ("Fatma Demir", "fatma.demir@mail.com", "+905001111004"),
            ("Can Öztürk", "can.ozturk@mail.com", "+905001111005"),
            ("Merve Çelik", "merve.celik@mail.com", "+905001111006"),
            ("Selim Koç", "selim.koc@mail.com", "+905001111007"),
            ("Ece Aydın", "ece.aydin@mail.com", "+905001111008"),
            ("Berk Aksoy", "berk.aksoy@mail.com", "+905001111009"),
            ("Deniz Şahin", "deniz.sahin@mail.com", "+905001111010"),
            ("Zeynep Arslan", "zeynep.arslan@mail.com", "+905001111011"),
            ("Emre Kılıç", "emre.kilic@mail.com", "+905001111012"),
            ("Seda Uçar", "seda.ucar@mail.com", "+905001111013"),
            ("Ozan Polat", "ozan.polat@mail.com", "+905001111014"),
            ("Nazlı Erdem", "nazli.erdem@mail.com", "+905001111015"),
            ("Tunç Vural", "tunc.vural@mail.com", "+905001111016"),
            ("İrem Bulut", "irem.bulut@mail.com", "+905001111017"),
            ("Barış Arı", "baris.ari@mail.com", "+905001111018"),
            ("Leyla Gün", "leyla.gun@mail.com", "+905001111019"),
            ("TLS Ornek", "tls.ornek@mail.com", "+905001111020"),
        };


        foreach (var (FullName, Email, Phone) in customers)
        {
            var existing = await userManager.FindByEmailAsync(Email);
            if (existing != null)
            {
                continue;
            }

            var password = $"{FullName.Replace(" ", "")}1.";

            var user = new Customer
            {
                UserName = Email,
                Email = Email,
                Name = FullName,
                EmailConfirmed = true,
                PhoneNumber = Phone,
                IsActive = true
            };

            await userManager.CreateAsync(user, password);
        }
    }

    public static async Task EnsureSeedCustomerAddressAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Customer>>();

        if (await context.CustomerAddresses.AnyAsync())
            return;

        var customers = await context.Users.ToListAsync();
        if (!customers.Any())
            return;

        var addresses = new List<CustomerAddress>();

        var cityData = new[]
        {
            ("İstanbul", "Kadıköy", 34710),
            ("Ankara", "Çankaya", 6510),
            ("İzmir", "Bornova", 35030),
            ("Bursa", "Nilüfer", 16110),
            ("Antalya", "Muratpaşa", 7010),
            ("Samsun", "Atakum", 55200),
            ("Trabzon", "Ortahisar", 61030),
            ("Konya", "Selçuklu", 42060),
            ("Eskişehir", "Odunpazarı", 26010),
            ("Adana", "Seyhan", 1010),
            ("Mersin", "Yenişehir", 33110),
            ("Gaziantep", "Şehitkamil", 27060),
            ("Denizli", "Pamukkale", 20070),
            ("Kayseri", "Melikgazi", 38030),
            ("Balıkesir", "Karesi", 10020),
            ("Manisa", "Yunusemre", 45030),
            ("Sakarya", "Serdivan", 54055),
            ("Kocaeli", "İzmit", 41010),
            ("Tekirdağ", "Çorlu", 59860),
            ("Zonguldak", "Merkez", 67100)
        };

        var rnd = new Random();
        int cityIndex = 0;

        foreach (var customer in customers.Take(14))
        {
            var (city, town, postal) = cityData[cityIndex++ % cityData.Length];
            addresses.Add(new CustomerAddress
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                Type = "Ev",
                Country = "Türkiye",
                City = city,
                Town = town,
                Address = $"{town} Mah. {rnd.Next(1, 200)}. Sok. No:{rnd.Next(1, 50)}",
                PostalCode = postal
            });
        }

        foreach (var customer in customers.Skip(14).Take(3))
        {
            var (city1, town1, postal1) = cityData[rnd.Next(cityData.Length)];
            var (city2, town2, postal2) = cityData[rnd.Next(cityData.Length)];

            addresses.Add(new CustomerAddress
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                Type = "Ev",
                Country = "Türkiye",
                City = city1,
                Town = town1,
                Address = $"{town1} Mah. {rnd.Next(1, 150)}. Sok. No:{rnd.Next(1, 40)}",
                PostalCode = postal1
            });

            addresses.Add(new CustomerAddress
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                Type = "İş",
                Country = "Türkiye",
                City = city2,
                Town = town2,
                Address = $"{town2} Cd. No:{rnd.Next(50, 250)} Kat:{rnd.Next(1, 5)}",
                PostalCode = postal2
            });
        }

        await context.CustomerAddresses.AddRangeAsync(addresses);
        await context.SaveChangesAsync();
    }

    public static async Task EnsureSeedStockAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await context.Stocks.AnyAsync())
            return;

        var stocks = new List<Stock>
        {
            new() { Id = Guid.NewGuid(), Name = "Dizüstü Bilgisayar", Unit = "Adet", Price = 32500.50m },
            new() { Id = Guid.NewGuid(), Name = "Mekanik Klavye", Unit = "Adet", Price = 950.90m },
            new() { Id = Guid.NewGuid(), Name = "Oyun Mouse'u", Unit = "Adet", Price = 620.00m },
            new() { Id = Guid.NewGuid(), Name = "USB Bellek 64GB", Unit = "Adet", Price = 240.75m },
            new() { Id = Guid.NewGuid(), Name = "Monitör 27''", Unit = "Adet", Price = 4950.00m },
            new() { Id = Guid.NewGuid(), Name = "HDMI Kablosu", Unit = "Adet", Price = 120.00m },
            new() { Id = Guid.NewGuid(), Name = "Ethernet Kablosu 10m", Unit = "Adet", Price = 95.00m },
            new() { Id = Guid.NewGuid(), Name = "Ofis Sandalyesi", Unit = "Adet", Price = 2890.00m },
            new() { Id = Guid.NewGuid(), Name = "Masaüstü Bilgisayar", Unit = "Adet", Price = 29500.00m },
            new() { Id = Guid.NewGuid(), Name = "Akıllı Telefon", Unit = "Adet", Price = 21999.99m },
            new() { Id = Guid.NewGuid(), Name = "Yazıcı", Unit = "Adet", Price = 3490.00m },
            new() { Id = Guid.NewGuid(), Name = "Fotokopi Kağıdı A4", Unit = "Koli", Price = 450.00m },
            new() { Id = Guid.NewGuid(), Name = "Toner Kartuşu", Unit = "Adet", Price = 1150.00m },
            new() { Id = Guid.NewGuid(), Name = "Su Şişesi 0.5L", Unit = "Paket", Price = 55.00m },
            new() { Id = Guid.NewGuid(), Name = "Kahve 1kg", Unit = "Kg", Price = 380.00m },
            new() { Id = Guid.NewGuid(), Name = "Çay 1kg", Unit = "Kg", Price = 290.00m },
            new() { Id = Guid.NewGuid(), Name = "El Dezenfektanı 1L", Unit = "Litre", Price = 95.00m },
            new() { Id = Guid.NewGuid(), Name = "Kağıt Havlu", Unit = "Paket", Price = 120.00m },
            new() { Id = Guid.NewGuid(), Name = "Masa Lambası", Unit = "Adet", Price = 675.00m },
            new() { Id = Guid.NewGuid(), Name = "Projeksiyon Cihazı", Unit = "Adet", Price = 8850.00m }
        };

        await context.Stocks.AddRangeAsync(stocks);
        await context.SaveChangesAsync();
    }

    public static async Task EnsureSeedOrderAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await context.Orders.AnyAsync())
            return;

        var customers = await context.Users.ToListAsync();
        var addresses = await context.CustomerAddresses.ToListAsync();
        if (!customers.Any() || !addresses.Any())
            return;

        var rnd = new Random();
        var orders = new List<Order>();

        var multiOrderCustomers = customers
            .Where(c => c.Name is "Levent Kalkavan" or "Mehmet Yılmaz" or "Ayşe Kaya")
            .ToList();

        var allCustomers = customers.Take(20).ToList();
        var orderCount = 0;

        foreach (var customer in allCustomers)
        {
            int ordersToCreate = multiOrderCustomers.Contains(customer) ? 2 : 1;

            for (int i = 0; i < ordersToCreate; i++)
            {
                var customerAddresses = addresses.Where(a => a.CustomerId == customer.Id).ToList();
                if (!customerAddresses.Any()) continue;

                var deliveryAddress = customerAddresses[rnd.Next(customerAddresses.Count)];
                var invoiceAddress = deliveryAddress;

                bool useDifferentAddress = orderCount >= 15 && customerAddresses.Count > 1;
                if (useDifferentAddress)
                {
                    invoiceAddress = customerAddresses.FirstOrDefault(a => a.Id != deliveryAddress.Id)
                                     ?? deliveryAddress;
                }

                decimal total = rnd.Next(1000, 30000);
                decimal tax = Math.Round(total * 0.18m, 2);

                orders.Add(new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    OrderDate = DateTime.UtcNow.AddDays(-rnd.Next(0, 60)),
                    OrderNo = Guid.NewGuid().ToString()[..8],
                    TotalPrice = total,
                    Tax = tax,
                    DeliveryAddressId = deliveryAddress.Id,
                    InvoiceAddressId = invoiceAddress.Id,
                    IsActive = true
                });

                orderCount++;
                if (orderCount >= 20)
                    break;
            }

            if (orderCount >= 20)
                break;
        }

        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();
    }

    public static async Task EnsureSeedOrderDetailAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await context.OrderDetails.AnyAsync())
            return;

        var orders = await context.Orders.ToListAsync();
        var stocks = await context.Stocks.ToListAsync();

        if (!orders.Any() || !stocks.Any())
            return;

        var rnd = new Random();
        var orderDetails = new List<OrderDetail>();

        foreach (var order in orders)
        {
            int detailCount = rnd.Next(1, 4);
            var selectedStocks = stocks.OrderBy(x => Guid.NewGuid()).Take(detailCount).ToList();

            foreach (var stock in selectedStocks)
            {
                int quantity = rnd.Next(1, 11);
                decimal total = stock.Price * quantity;

                orderDetails.Add(new OrderDetail
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    StockId = stock.Id,
                    Amount = quantity,
                    IsActive = true
                });
                
                order.TotalPrice += total;
                order.Tax = Math.Round(order.TotalPrice * 0.20m, 2);
            }
        }

        await context.OrderDetails.AddRangeAsync(orderDetails);
        await context.SaveChangesAsync();
    }
}