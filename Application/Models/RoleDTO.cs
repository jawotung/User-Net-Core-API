using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Models;
public class RoleDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Role Code is required.")]
    [StringLength(50, ErrorMessage = "Role Code cannot be longer than 50 characters.")]
    public string RoleCode { get; set; } = null!;

    [Required(ErrorMessage = "Role Name is required.")]
    [StringLength(50, ErrorMessage = "Role Name cannot be longer than 50 characters.")]
    public string RoleName { get; set; } = null!;
}
