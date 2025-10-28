using System;
using Microsoft.AspNetCore.Identity;

namespace BLE.Data;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? DisplayName { get; set; }
    public bool IsAdmin { get; set; }
}
