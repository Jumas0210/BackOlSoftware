using System;
using System.Collections.Generic;

namespace BackOlSoftware.Models;

public partial class Merchant
{
    public int MerchantId { get; set; }

    public string BusinessName { get; set; } = null!;

    public string City { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string Status { get; set; } = null!;

    public DateTime LastUpdated { get; set; }

    public int UpdatedByUser { get; set; }

    public virtual ICollection<Establishment> Establishments { get; set; } = new List<Establishment>();
}
