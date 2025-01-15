using System;
using System.Collections.Generic;

namespace WebAPI;

public partial class Module
{
    public int Id { get; set; }

    public string PageName { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string ParentName { get; set; } = null!;

    public int OrderBy { get; set; }

    public int CreateId { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual ICollection<RoleModule> RoleModules { get; set; } = new List<RoleModule>();
}
