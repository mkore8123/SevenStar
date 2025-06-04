using System.Security.Claims;

namespace Common.Api.Auth;

/// <summary>
/// 定義模型與 JWT Claims 之間轉換的介面。
/// 通常用於將應用程式的資料模型（如 User, Tenant, Device 等）轉換為 <see cref="Claim"/> 集合，
/// 或將 Claims 還原回模型，便於自訂 Claims 結構、支援多租戶或自訂權限。
/// </summary>
/// <typeparam name="TModel">
/// 欲轉換的資料模型型別，通常為用於產生 JWT Token 的物件型別。
/// </typeparam>
public interface IClaimsMapper<TModel>
{
    /// <summary>
    /// 將指定的 <typeparamref name="TModel"/> 資料模型轉換為 <see cref="Claim"/> 集合。
    /// 用於產生 JWT Token 時，將模型屬性映射到對應 Claims。
    /// </summary>
    /// <param name="model">來源模型實例，內容將被映射至多個 Claim。</param>
    /// <returns>映射後的 <see cref="Claim"/> 集合。</returns>
    IEnumerable<Claim> ToClaims(TModel model);

    /// <summary>
    /// 根據 <see cref="ClaimsPrincipal"/> 解析還原對應的 <typeparamref name="TModel"/> 模型。
    /// 常用於驗證或解密 JWT Token 後，還原業務邏輯所需的模型物件。
    /// </summary>
    /// <param name="principal">含有多個 Claim 的 <see cref="ClaimsPrincipal"/> 實例，通常由 Token 解析而得。</param>
    /// <returns>還原後的 <typeparamref name="TModel"/> 實例。</returns>
    TModel FromClaims(ClaimsPrincipal principal);
}
