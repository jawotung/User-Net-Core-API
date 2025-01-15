using System;
using System.Collections.Generic;

namespace WebAPI;

public partial class RoleModule
{
    public int Id { get; set; }

    public int? RolesId { get; set; }

    public int? ModuleId { get; set; }

    public int CreateId { get; set; }

    public DateTime CreateDate { get; set; }

    public int UpdateId { get; set; }

    public DateTime UpdateDate { get; set; }

    public virtual Module? Module { get; set; }

    public virtual Role? Roles { get; set; }
}
