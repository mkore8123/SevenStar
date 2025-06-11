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

    /// <summary>
    /// 將指定的 <typeparamref name="TModel"/> 資料模型轉換為 <see cref="Dictionary{String, Object}"/> 型態的 Claims 字典。
    /// 用於產生 JWT Token 或其他協議時，將模型屬性對應映射為 Key-Value 格式，便於序列化、加密或跨系統傳遞。
    /// </summary>
    /// <param name="model">
    /// 欲轉換的模型資料實例，其屬性內容將映射為字典的鍵值對。
    /// 常用於組合 JWT Payload 或進行自訂 Token 產生時使用。
    /// </param>
    /// <returns>
    /// 包含模型屬性對應之 Claims 資料的 <see cref="Dictionary{String, Object}"/>，
    /// Key 通常為 Claim 的名稱，Value 為對應的屬性值。
    /// </returns>
    Dictionary<string, object> ToClaimsDic(TModel model);

    /// <summary>
    /// 根據指定的 <see cref="Claim"/> 集合，還原對應的 <typeparamref name="TModel"/> 模型。
    /// 常用於直接將 JWT Token 或 ClaimsIdentity 解析出來的 claims 欄位，還原成業務邏輯模型，
    /// 無需包裝成 <see cref="ClaimsPrincipal"/> 物件時可直接使用，適合自訂解密、跨系統協議解析等情境。
    /// </summary>
    /// <param name="principal">
    /// 含有多個 <see cref="Claim"/> 的集合，通常來自 Token 解密、手動組裝或其他驗證流程結果。
    /// 每個 <see cref="Claim"/> 代表一個權限、屬性或身份資訊，
    /// 本方法會將這些 claims 映射還原成 <typeparamref name="TModel"/> 型別的實體物件。
    /// </param>
    /// <returns>
    /// 還原後的 <typeparamref name="TModel"/> 實例，
    /// 包含 claims 中對應欄位轉回模型屬性的資料。
    /// </returns>
    TModel FromClaimsDic(IDictionary<string, object> claims);
}
