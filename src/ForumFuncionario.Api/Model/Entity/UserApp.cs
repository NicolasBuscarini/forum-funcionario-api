using Microsoft.AspNetCore.Identity;

public class UserApp : IdentityUser<int>
{
    public required string RaNome { get; set; }

    public required string UserProtheusId { get; set; }

    public string? Discriminator { get; set; }
    public string Email { get; set; }
    public byte[]? Foto { get; set; }
}