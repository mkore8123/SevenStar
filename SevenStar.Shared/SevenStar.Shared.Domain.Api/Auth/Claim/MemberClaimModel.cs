using Common.Api.Auth;
using Common.Api.Auth.Claims;
using System.Security.Claims;

namespace SevenStar.Shared.Domain.Api.Auth.Claims;

public class MemberClaimModel : UserClaimModel
{
    public string CompanyId { get; set; } = default!;

    public string TokenVersion { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;
    
    public string? Email { get; set; } 
    
    public List<string> Roles { get; set; } = new List<string>();
}
