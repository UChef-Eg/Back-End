namespace UcheifBackend.Models;

public class CookProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string KitchenName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsVerified { get; set; } = false;

    public AppUser User { get; set; } = null!;
}
