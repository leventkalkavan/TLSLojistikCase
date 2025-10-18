using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManagement.Entities;

public class OrderDetail
{
    [Key] public Guid Id { get; set; }
    public Order Order { get; set; }
    public Guid OrderId { get; set; }
    public Stock Stock { get; set; }
    public Guid StockId { get; set; }
    public int Amount { get; set; }
    public bool IsActive { get; set; } =  true;
}