using System.Security.Claims;

namespace Common.Api.Authentication;

/// <summary>
/// 定義加解密各類型權杖（Token）的通用服務介面。
/// 支援泛型 <typeparamref name="T"/> 作為資料模型，適用於 JWT、Cookie Token、API Token、Session Token 等多種身分驗證或授權場景。
/// </summary>
/// <typeparam name="T">
/// 欲產生或解析的資料模型型別，通常對應於使用者、帳號、租戶、設備等身分資訊結構。
/// </typeparam>
public interface ITokenService<T>
{
    /// <summary>
    /// 驗證並解析指定的權杖字串（如 JWT、Cookie Token），轉換為 <typeparamref name="T"/> 資料模型。
    /// 若驗證失敗、已過期或格式錯誤，將拋出例外。
    /// </summary>
    /// <param name="token">待解析的權杖字串（可為 JWT、Cookie Token 等）。</param>
    /// <returns>解析後對應的 <typeparamref name="T"/> 資料模型。</returns>
    /// <exception cref="SecurityTokenException">當驗證失敗或內容錯誤時拋出。</exception>
    Task<T> DecrypteToken(string token);

    /// <summary>
    /// 根據指定 <typeparamref name="T"/> 模型內容產生權杖字串（如 JWT、Cookie Token）。
    /// 產生過程發生錯誤將拋出例外。
    /// </summary>
    /// <param name="model">作為權杖內容來源的資料模型。</param>
    /// <returns>產生的權杖字串。</returns>
    /// <exception cref="Exception">產生過程發生例外時拋出。</exception>
    Task<string> GenerateToken(T model);

    /// <summary>
    /// 嘗試根據指定模型產生權杖字串。
    /// 成功時回傳 true，並將產生的權杖指派給 <paramref name="token"/>；失敗時回傳 false，<paramref name="token"/> 為 null。
    /// </summary>
    /// <param name="model">欲產生權杖的資料模型。</param>
    /// <param name="token">產生成功時，輸出對應的權杖字串；否則為 null。</param>
    /// <returns>產生成功時回傳 true，否則 false。</returns>
    async Task<(bool, string?)> TryGenerateToken(T model)
    {
        try
        {
            var token = await GenerateToken(model);
            return (true, token);
        }
        catch
        {
            return (false, null);
        }
    }

    /// <summary>
    /// 嘗試將指定的權杖字串解密並還原為 <typeparamref name="T"/> 資料模型。
    /// 成功時回傳 true，並將解析結果指派給 <paramref name="model"/>；失敗時回傳 false，<paramref name="model"/> 為 null。
    /// </summary>
    /// <param name="token">待解密的權杖字串（如 JWT、Cookie Token）。</param>
    /// <param name="model">解密成功時，輸出對應的資料模型；否則為 null。</param>
    /// <returns>解密成功時回傳 true，否則 false。</returns>
    async Task<(bool, T? model)> TryDecrypteToken(string token)
    {
        try
        {
            var model = await DecrypteToken(token);
            return (true, model);
        }
        catch
        {
            return (false, default(T));
        }
    }
}
