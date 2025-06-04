using System.Security.Claims;

namespace Common.Api.Auth.Claims;

public class UserClaimMapper : IClaimsMapper<UserClaimModel>
{
    public IEnumerable<Claim> ToClaims(UserClaimModel model)
    {
        yield return new Claim("uid", model.UserId.ToString());
        // yield return new Claim("cid", model.CompanyId);
        yield return new Claim("device", model.Device);
    }

    public UserClaimModel FromClaims(ClaimsPrincipal principal)
    {
        return new UserClaimModel
        {
            UserId = long.Parse(principal.FindFirstValue("uid")),
            // CompanyId = principal.FindFirstValue("cid") ?? "",
            Device = principal.FindFirstValue("device") ?? ""
        };
    }
}