using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Common.Api.Auth;

public interface IClaimsMapper<TModel>
{
    IEnumerable<Claim> ToClaims(TModel model);
    TModel FromClaims(ClaimsPrincipal principal);
}
