using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManagement.Entities;

public class Stock
{
    [Key] public Guid Id { get; set; }
    public string Name { get; set; }
    public string Unit { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    public string Barcode { get; set; } = Guid.NewGuid().ToString();
    public bool IsActive { get; set; } = true;
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}