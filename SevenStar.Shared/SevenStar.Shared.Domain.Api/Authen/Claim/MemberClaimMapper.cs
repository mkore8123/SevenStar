using Common.Api.Auth;
using System.Security.Claims;

namespace SevenStar.Shared.Domain.Api.Authen.Claim;

public class MemberClaimMapper : IClaimsMapper<MemberClaimModel>
{
    public IEnumerable<Claim> ToClaims(MemberClaimModel model)
    {
        yield return new Claim("uid", model.UserId.ToString());
        yield return new Claim("cid", model.CompanyId);
        yield return new Claim("device", model.Device);
    }

    public MemberClaimModel FromClaims(ClaimsPrincipal principal)
    {
        return new MemberClaimModel
        {
            UserId = long.Parse(principal.FindFirstValue("uid")),
            CompanyId = principal.FindFirstValue("cid") ?? "",
            Device = principal.FindFirstValue("device") ?? ""
        };
    }
}
