using System;
using System.Collections.Generic;

namespace WebAPI;

public partial class Item
{
    public int Id { get; set; }

    public string? ItemCode { get; set; }

    public string? ItemName { get; set; }

    public int CreateId { get; set; }

    public DateTime CreateDate { get; set; }

    public int UpdateId { get; set; }

    public DateTime UpdateDate { get; set; }
}
