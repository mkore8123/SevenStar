using Common.Api.Authen.Jwt.Enum;

namespace Common.Api.Authen.Jwt.Interface;

/// <summary>
/// JWT envelope 型態解析器介面（可單測/替換/Mock）
/// </summary>
public interface IJwtEnvelopeTypeResolver
{
    JwtEnvelopeType Resolve(string jwtToken);
}
