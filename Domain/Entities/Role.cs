using System;
using System.Collections.Generic;

namespace WebAPI;

public partial class Role
{
    public int Id { get; set; }

    public string RoleCode { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public int CreateId { get; set; }

    public DateTime CreateDate { get; set; }

    public int UpdateId { get; set; }

    public DateTime UpdateDate { get; set; }

    public virtual ICollection<RoleModule> RoleModules { get; set; } = new List<RoleModule>();
}
