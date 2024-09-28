using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser<int>
{
    public required string RaNome { get; set; }

    public required string UserProtheusId { get; set; }

    public string? Discriminator { get; set; }
    public string Email { get; set; }
}