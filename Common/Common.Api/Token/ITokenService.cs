
namespace Common.Api.Token;

/// <summary>
/// 加密,解密 jwt 的服務介面
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITokenService<T>
{
    /// <summary>
    /// 根據參數生成 jwt，若成功則為回傳 jwt，若失敗則拋出例外。
    /// </summary>
    /// <param name="model"></param>
    /// <param name="jwt"></param>
    /// <returns></returns>
    string GenerateToken(T model);

    /// <summary>
    /// 解密 jwt，若成功則為回傳指定 Model，若失敗則拋出例外
    /// </summary>
    /// <param name="jwt"></param>
    /// <returns></returns>
    T DecrypteToken(string jwt);

    /// <summary>
    /// 嘗試根據參數生成 jwt，若成功則為 true，並將 jwt 賦值給 jwtToken，若失敗則為 false，jwt 為 null。
    /// </summary>
    /// <param name="model"></param>
    /// <param name="jwt"></param>
    /// <returns></returns>
    bool TryGenerateToken(T model, out string jwtToken)
    {
        try
        {
            jwtToken = GenerateToken(model);
            return true;
        }
        catch
        {
            jwtToken = null!;
            return false;
        }
    }

    /// <summary>
    /// 嘗試解密 jwt，若成功則為 true，並將物件賦值給 model，若失敗則為 false，model 為 null。
    /// </summary>
    /// <param name="jwtToken"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    bool TryDecrypteToken(string jwtToken, out T? model)
    {
        try
        {
            model = DecrypteToken(jwtToken);
            return true;
        }
        catch
        {
            model = default(T);
            return false;
        }
    }
}