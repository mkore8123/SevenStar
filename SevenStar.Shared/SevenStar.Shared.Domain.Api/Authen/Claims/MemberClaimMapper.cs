using Common.Api.Auth;
using Common.Enums;
using System.Security.Claims;

namespace SevenStar.Shared.Domain.Api.Authen.Claims;

public class MemberClaimMapper : IClaimsMapper<MemberClaimModel>
{
    public IEnumerable<Claim> ToClaims(MemberClaimModel model)
    {
        yield return new Claim("uid", model.UserId.ToString());
        yield return new Claim("cid", model.CompanyId);
        yield return new Claim("device", model.Device.ToString());
    }

    public MemberClaimModel FromClaims(ClaimsPrincipal principal)
    {
        return new MemberClaimModel
        {
            UserId = long.Parse(principal.FindFirstValue("uid")),
            CompanyId = principal.FindFirstValue("cid") ?? "",
            Device =  DeviceTypeEnum.Web, //principal.FindFirstValue("device") ?? ""
        };
    }

    public Dictionary<string, object> ToClaimsDic(MemberClaimModel model)
    {
        throw new NotImplementedException();
    }

    public MemberClaimModel FromClaimsDic(IDictionary<string, object> claims)
    {
        throw new NotImplementedException();
    }
}
