using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Common.Api.Auth;

public class MyUserModel
{
    public string UserId { get; set; } = default!;
    public string CompanyId { get; set; } = default!;
    public string Device { get; set; } = default!; // mobile or web
}


public class MyUserClaimsMapper : IClaimsMapper<MyUserModel>
{
    public IEnumerable<Claim> ToClaims(MyUserModel model)
    {
        yield return new Claim("uid", model.UserId);
        yield return new Claim("cid", model.CompanyId);
        yield return new Claim("device", model.Device);
    }

    public MyUserModel FromClaims(ClaimsPrincipal principal)
    {
        return new MyUserModel
        {
            UserId = principal.FindFirstValue("uid") ?? "",
            CompanyId = principal.FindFirstValue("cid") ?? "",
            Device = principal.FindFirstValue("device") ?? ""
        };
    }
}