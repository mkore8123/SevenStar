using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authen.Enum;

/// <summary>
/// JWT 包裝技術型態（JWS / JWE / 純 JWT / 嵌套 JWT）。
/// <para>
/// 本列舉用於區分 JWT Token 的實際封裝方式。  
/// 各成員與 JWT header 的 typ 欄位、分段結構（. 分隔數量）、header 其他欄位（alg, enc）之關係如下：
/// <list type="table">
/// <item><term>PlainJwt</term>
///   <description>
///     三段結構，無簽章、無加密；header typ="JWT" 或省略。<br/>
///     極少見，僅用於測試或協議內部溝通，安全性低。
///   </description>
/// </item>
/// <item><term>Jws</term>
///   <description>
///     三段結構，有簽章、無加密；header 通常 typ="JWT" 或 "JWS"，alg 有值、enc 無值。<br/>
///     業界預設 typ="JWT"。是現今最常見的 JWT 包裝型態。
///   </description>
/// </item>
/// <item><term>JwsJwt</term>
///   <description>
///     三段結構，有簽章且 typ="JWT"；alg 有值。<br/>
///     實際與 Jws 用法幾乎一致，標準 OAuth2 / OpenID Connect token 皆為此型態。
///   </description>
/// </item>
/// <item><term>Jwe</term>
///   <description>
///     五段結構，有加密（enc 有值），typ 可為 "JWE" 或 "JWT"（多數框架預設 typ="JWT"），alg、enc 都有值。<br/>
///     用於傳遞高度敏感資訊。
///   </description>
/// </item>
/// <item><term>JweJwt</term>
///   <description>
///     五段結構，有加密且 typ="JWT"；alg、enc 都有值。<br/>
///     實務上與 Jwe 用法接近，僅 typ 欄位差異。
///   </description>
/// </item>
/// <item><term>NestedJwsJwe</term>
///   <description>
///     嵌套型 JWT：「先簽章、後加密」。流程為：<br/>
///     1. 先產生 JWS（3段，有簽章，typ="JWT"）；<br/>
///     2. 再將其作為 JWE payload，進行加密（最終 token 5段）。<br/>
///     typ 通常仍設為 "JWT"。詳見 RFC 7519 §9.2。<br/>
///     此模式可同時兼顧不可否認性（簽章）與機密性（加密），但實務應用極少見。
///   </description>
/// </item>
/// <item><term>Custom</term>
///   <description>
///     其他自訂型態；typ 欄位可為 "at+jwt"、"id_token"、"rt+jwt" 等特殊用途字串，也可能省略 typ。<br/>
///     適用於協議擴充或非標準情境。
///   </description>
/// </item>
/// </list>
/// <br/>
/// <b>說明：</b><br/>
/// - typ 欄位主要為「提示」用途，**無法唯一決定包裝型態**，判斷時應結合分段數與 alg、enc 等欄位。<br/>
/// - 現今主流 JWT 框架預設 typ="JWT"（即便是 JWE 或 JWS）。<br/>
/// - 若需自動判斷，建議依分段數（3段/5段）、alg/enc 是否存在，和 typ 綜合判斷。
/// </para>
/// </summary>
public enum JwtEnvelopeType
{
    /// <summary>
    /// 純 JWT，無簽章、無加密。三段結構，typ="JWT" 或省略。極少用。
    /// </summary>
    PlainJwt,
    /// <summary>
    /// JWS，僅簽章、無加密。三段結構，typ="JWT" 或 "JWS"。
    /// </summary>
    Jws,
    /// <summary>
    /// JWE，僅加密、無簽章。五段結構，typ="JWE" 或 "JWT"。
    /// </summary>
    Jwe,

    /// <summary>
    /// 嵌套型 JWT：「先簽章再加密」，RFC 7519 §9.2 支援。實際 typ 通常仍為 "JWT"。
    /// </summary>
    NestedJwsJwe,
    /// <summary>
    /// 其他自訂型態（如 "at+jwt"、"id_token"、自訂 typ、特殊協議…）。
    /// </summary>
    Custom
}

