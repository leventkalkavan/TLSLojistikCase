using System.ComponentModel.DataAnnotations;

namespace StockManagement.Entities;

public class CustomerAddress
{
    [Key] public Guid Id { get; set; }

    public bool IsActive { get; set; } = true;
    public string Type { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Town { get; set; }
    public string Address { get; set; }
    public int PostalCode { get; set; }
    public string CustomerId { get; set; }
    public Customer Customer { get; set; }
}