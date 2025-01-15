using System;
using System.Collections.Generic;

namespace WebAPI;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Gender { get; set; }

    public int RoleId { get; set; }

    public int CreateId { get; set; }

    public DateTime CreateDate { get; set; }

    public int UpdateId { get; set; }

    public DateTime UpdateDate { get; set; }
}
