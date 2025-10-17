using Microsoft.AspNetCore.Identity;

namespace StockManagement.Entities;

public class Customer : IdentityUser
{
    public bool IsActive { get; set; } = true;
    public ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}