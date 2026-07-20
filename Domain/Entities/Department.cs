using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
  public class Department
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public decimal ConversionFactor { get; set; }

    public string MeterCode { get; set; } = string.Empty;

    public decimal Discount { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public bool IsFixed { get; set; }

    public decimal FixedValue { get; set; } = 0;
  }

}
