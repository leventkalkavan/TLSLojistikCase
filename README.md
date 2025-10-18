# StockManagement – Sipariş Yönetim Sistemi (.NET 9 MVC + PostgreSQL + SignalR)

Bu proje, verilen teknik test senaryosundaki gereksinimleri karşılamak amacıyla geliştirilmiş bir sipariş yönetim sistemidir. Uygulama, kullanıcıların sisteme giriş yapabildiği, sipariş oluşturabildiği, sipariş kalemlerini ekleyebildiği ve oluşturulan siparişleri görüntüleyip filtreleyebildiği bir web projesidir.

Proje .NET 9 SDK kullanılarak ASP.NET Core MVC mimarisiyle yazılmıştır. Veritabanı olarak PostgreSQL tercih edilmiştir. Database’in local ortamla sınırlı kalmaması için Supabase üzerinde barındırılmıştır. Supabase, PostgreSQL tabanlı olduğu için Entity Framework Core üzerinde Npgsql sağlayıcısı kullanılarak tam uyumlu hale getirilmiştir.

---

## Kullanılan Teknolojiler
- .NET 9 – ASP.NET Core MVC (katmanlı mimari)
- Entity Framework Core (Npgsql Provider)
- PostgreSQL (Supabase üzerinde)
- ASP.NET Identity (Login / Logout / Authentication)
- SignalR (Gerçek zamanlı sipariş bildirimi)
- Razor View Engine (CSHTML tabanlı frontend)
- C# SeedData (Test verisi otomatik üretimi)

---

## Proje Yapısı

```
StockManagement/
│
├── Controllers/
│   ├── AccountController.cs
│   ├── HomeController.cs
│   └── OrderController.cs
│
├── Services/
│   ├── Interfaces/
│   ├── OrderService.cs
│   ├── OrderDetailService.cs
│   ├── StockService.cs
│   └── CustomerAddressService.cs
│
├── Entities/
│   ├── Customer.cs
│   ├── CustomerAddress.cs
│   ├── Stock.cs
│   ├── Order.cs
│   └── OrderDetail.cs
│
├── Models/
│   └── ViewModels/
│       ├── OrderModels/
│       ├── OrderDetailModels/
│       └── AccountModels/
│
├── Data/
│   └── ApplicationDbContext.cs
│
├── Hubs/
│   └── OrderHub.cs
│
└── Views/
    ├── Account/
    │   └── Login.cshtml
    │
    ├── Order/
    │   ├── CreateOrder.cshtml
    │   ├── GetAllOrders.cshtml
    │   └── Modals/
    │       ├── _TLSOrdersModal.cshtml
    │       ├── _IstanbulOrdersModal.cshtml
    │       ├── _CustomerOrdersModal.cshtml
    │       ├── _DifferentAddressOrdersModal.cshtml
    │       ├── _MultiQuantityOrdersModal.cshtml
    │       ├── _ProductCustomersModal.cshtml
    │       ├── _MyOrdersModal.cshtml
    │       └── _OrderTableBody.cshtml
```


---

## Veritabanı Şeması
Customer → Müşteri bilgileri  
CustomerAddress → Teslimat ve fatura adresleri  
Stock → Ürün bilgileri (isim, fiyat, birim, barkod)  
Order → Sipariş başlığı (müşteri, adresler, toplam fiyat, vergi, tarih)  
OrderDetail → Sipariş kalemleri (ürün, miktar, fiyat)

İlişkiler:
- Customer → Addresses (1:N)
- Customer → Orders (1:N)
- Order → OrderDetails (1:N)
- Stock → OrderDetails (1:N)
- Order → DeliveryAddress & InvoiceAddress (1:1)

---

## Özellikler
1. **Giriş Sistemi**
   - ASP.NET Identity tabanlı login/logout işlemleri
   - Aktif olmayan kullanıcılar giriş yapamaz
   - LoginViewModel ile doğrulama yapılır

2. **Sipariş Oluşturma**
   - Stok listesi dinamik olarak dropdown’da fiyat bilgisiyle gelir
   - Bir sipariş birden fazla kalem (ürün) içerebilir
   - OrderService.CreateAsync() metodu üzerinden kayıt yapılır
   - Kayıt sonrası SignalR üzerinden “Yeni sipariş oluşturuldu” bildirimi gönderilir

3. **Sipariş Listeleme ve Raporlama**
   - GetAllOrders.cshtml sayfasında tüm siparişler listelenir
   - AJAX destekli partial view yapısı sayesinde sayfa yenilenmeden tablo güncellenir
   - Aşağıdaki özel modallar hazırlanmıştır:
     - Aynı ürünü alan müşteriler
     - Miktarı 1’den büyük siparişler
     - Fatura ve teslimat adresi farklı siparişler
     - TLS adlı müşterinin siparişleri
     - İstanbul şehrine ait siparişler

4. **Gerçek Zamanlı Bildirimler**
   - SignalR Hub yapısı ile sipariş oluşturma ve iptal olayları anlık olarak bildirilir
   - Dashboard ekranında eşzamanlı güncelleme sağlanır

5. **Seed Data**
   - İlk çalıştırmada 20 müşteri, 20 stok ve 20+ sipariş otomatik olarak eklenir
   - Test senaryosundaki tüm sorguların doğru sonuç verebilmesi için veriler dengeli şekilde üretilmiştir

---

## Test Gereksinimlerinin Karşılığı
Mailde ek olarak gönderilen excel dosyasındaki tüm sorgular hem SQL hem LINQ ile karşılanmıştır. SQL sorguları DbSorguları.txt dosyasında bulunuyor. LINQ sorguları ise:

1. “Bu üründen alan müşteriler”  
   → OrderDetailService.GetProductCustomersAsync()  
   → _ProductCustomersModal.cshtml

2. “Siparişteki ürün miktarı 1’den büyük olan müşteriler ve sipariş detayları”  
   → OrderDetailService.GetMultiQuantityOrdersAsync()  
   → _MultiQuantityOrdersModal.cshtml

3. “Fatura adresi ve teslimat adresi aynı olmayan müşteri listesi”  
   → OrderDetailService.GetDifferentAddressOrdersAsync()  
   → _DifferentAddressOrdersModal.cshtml

4. “Müşteri adı TLS olan müşterinin siparişleri”  
   → OrderDetailService.GetTLSOrdersAsync()  
   → _TLSOrdersModal.cshtml

5. “İstanbul şehrine ait kaç sipariş var?”  
   → OrderDetailService.GetIstanbulOrdersAsync()  
   → _IstanbulOrdersModal.cshtml

---

## Kurulum
### Gereksinimler
- .NET SDK 9.0+
- PostgreSQL 16+ veya Supabase bağlantısı
- Visual Studio / VS Code

### Adımlar
1. Projeyi klonla:
   git clone https://github.com/leventkalkavan/StockManagement.git
   cd StockManagement

2. appsettings.json dosyasına Supabase bağlantı stringini ekle:
   "ConnectionStrings": {
     "DefaultConnection": "Host=db.supabase.co;Port=5432;Database=stockdb;Username=postgres;Password=***"
   }

3. Migration’ları uygula:
   dotnet ef database update

4. Uygulamayı çalıştır:
   dotnet run

5. Tarayıcıdan aç:
   http://localhost:5160
