using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Auth.Enum;

/// <summary>
/// 表示系統支援的存取 Token 型態。
/// 不同的認證機制可透過此列舉值選擇，以決定授權驗證的方式。
/// </summary>
public enum TokenType
{
    /// <summary>
    /// JSON Web Token (JWT)。
    /// 以 Base64Url 編碼的 Header.Payload.Signature 格式攜帶身分資訊與簽章，
    /// 通常透過 Bearer Token 傳遞於 Authorization Header。
    /// 適合無狀態（stateless）驗證機制，支援多服務解耦。
    /// </summary>
    Jwt,

    /// <summary>
    /// Cookie-Based Token。
    /// 通常將 Session ID 或短期存取 Token 儲存在瀏覽器 Cookie 中，
    /// 由伺服器維護 Session 狀態。適合 Web Browser 與同域存取。
    /// </summary>
    Cookie,

    /// <summary>
    /// API Key。
    /// 使用者或應用程式在 Header（例如 X-API-Key）中提供預先發行的金鑰。
    /// 適用於服務間調用、第三方集成、或公開 API 存取限制。
    /// </summary>
    ApiKey,

    /// <summary>
    /// Opaque Token。
    /// 代表一個無法自行解碼的隨機字串（例如 GUID、UUID 或雜湊值），
    /// 驗證與權限需由伺服器端查詢資料庫決定。
    /// 常見於 OAuth 2.0 Access Token 的設計。
    /// </summary>
    Opaque
}
