using System;
namespace BackOlSoftware.Models
{
    public class MerchantReport
    {
        public string BusinessName { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Status { get; set; } = null!;
        public int EstablishmentCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalEmployees { get; set; }
    }
}
