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

    public UserClaimModel FromClaimsDic(IDictionary<string, object> claimsDic)
    {
        // 取得 uid
        long userId = 0;
        if (claimsDic.TryGetValue("uid", out var uidObj))
        {
            if (uidObj is long l) userId = l;
            else if (uidObj is string s && long.TryParse(s, out var l2)) userId = l2;
            else if (uidObj != null && long.TryParse(uidObj.ToString(), out var l3)) userId = l3;
        }

        // // 取得 company id
        // string companyId = claimsDic.TryGetValue("cid", out var cidObj) ? cidObj?.ToString() ?? "" : "";

        // 取得 device
        string device = claimsDic.TryGetValue("device", out var deviceObj) ? deviceObj?.ToString() ?? "" : "";

        return new UserClaimModel
        {
            UserId = userId,
            // CompanyId = companyId,
            Device = device
        };
    }

    public Dictionary<string, object> ToClaimsDic(UserClaimModel model)
    {
        var dic = new Dictionary<string, object>
        {
            ["uid"] = model.UserId,
            // ["cid"] = model.CompanyId,
            ["device"] = model.Device ?? ""
        };
        return dic;
    }
}