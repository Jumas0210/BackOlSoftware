using System;
using System.Collections.Generic;

namespace BackOlSoftware.Models;

public partial class Establishment
{
    public int EstablishmentId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Revenue { get; set; }

    public int EmployeeCount { get; set; }

    public DateTime LastUpdated { get; set; }

    public int UpdatedByUser { get; set; }

    public virtual ICollection<Merchant> Merchants { get; set; } = new List<Merchant>();
}
