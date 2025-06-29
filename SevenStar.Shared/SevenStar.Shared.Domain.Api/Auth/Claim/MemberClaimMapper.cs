using Common.Api.Authen;
using Common.Enums;
using System.Security.Claims;

namespace SevenStar.Shared.Domain.Api.Auth.Claims;

public class MemberClaimMapper : IClaimsMapper<MemberClaimModel>
{
    public IEnumerable<Claim> ToClaims(MemberClaimModel model)
    {
        yield return new Claim("uid", model.Id.ToString());
        yield return new Claim("cid", model.CompanyId);
        yield return new Claim("device", model.Device.ToString());
    }

    public Dictionary<string, object> ToClaimsDic(MemberClaimModel model)
    {
        // 呼叫 ToClaims 再轉成 Dictionary
        return ToClaims(model).ToDictionary(claim => claim.Type, claim => (object)claim.Value);
    }

    public MemberClaimModel FromClaimsDic(IDictionary<string, object> claims)
    {
        return FromClaimsDictionary(claims);
    }

    public MemberClaimModel FromClaimsDictionary(IDictionary<string, object> claims)
    {
        return new MemberClaimModel
        {
            Id = long.Parse(claims["uid"]?.ToString() ?? "0"),
            CompanyId = claims["cid"]?.ToString() ?? "",
            Device = DeviceTypeEnum.Web // 或 Enum.Parse
        };
    }
}
