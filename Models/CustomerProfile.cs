namespace UcheifBackend.Models;

public class CustomerProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string? PreferredLanguage { get; set; }
    public string? DeliveryAddress { get; set; }

    public AppUser User { get; set; } = null!;
}
