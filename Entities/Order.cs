using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManagement.Entities;

public class Order
{
    [Key] public Guid Id { get; set; }
    public Customer Customer { get; set; }
    public string CustomerId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;
    public string OrderNo { get; set; } = Guid.NewGuid().ToString();

    [Column(TypeName = "decimal(18,2)")] public decimal TotalPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")] public decimal Tax { get; set; }

    public Guid DeliveryAddressId { get; set; }
    public CustomerAddress DeliveryAddress { get; set; }

    public Guid InvoiceAddressId { get; set; }
    public CustomerAddress InvoiceAddress { get; set; }

    public bool IsActive { get; set; } = true;
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}