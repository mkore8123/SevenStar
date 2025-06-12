using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.DbContext.Platform.Entity;

/// <summary>
/// JWT 加密金鑰資料實體，對應資料表 jwt_encrypting_key。
/// </summary>
public class JwtEncryptingKeyEntity
{
    /// <summary>
    /// JWT 加密金鑰主鍵，自動編號（資料表 id）。
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 對應 JWT 設定主鍵（jwt_token_config.id），
    /// 一組加密金鑰對應一組 JWT 設定，可多把金鑰並存輪替。
    /// </summary>
    public int ConfigId { get; set; }

    /// <summary>
    /// Key ID（kid），JWT Header 內標示的加密金鑰編號，需全域唯一。
    /// </summary>
    public string KeyId { get; set; } = default!;

    /// <summary>
    /// 金鑰加密演算法（alg），如 "RSA-OAEP"、"A256KW"。
    /// 指定 JWE 金鑰管理方式，對應 JWE header 的 "alg" 欄位。
    /// </summary>
    public string Algorithm { get; set; } = default!;

    /// <summary>
    /// 內容加密演算法（enc），如 "A256CBC-HS512"、"A256GCM"。
    /// 指定實際 payload 加密方式，對應 JWE header 的 "enc" 欄位。
    /// </summary>
    public string ContentAlg { get; set; } = default!;

    /// <summary>
    /// 公鑰內容（PEM 格式/明文），僅用於非對稱加密金鑰（如 RSA/ECDH）。
    /// 對稱加密時可留空。
    /// </summary>
    public string? PublicKey { get; set; }

    /// <summary>
    /// 私鑰內容（明文或加密/PEM），
    /// 對稱金鑰時存放密鑰本體，非對稱金鑰時僅 JWT 發 token 端需存。
    /// </summary>
    public string? PrivateKey { get; set; }

    /// <summary>
    /// 金鑰啟用時間（UTC），僅限於此時間之後簽發/解密有效。
    /// </summary>
    public DateTime ValidFrom { get; set; }

    /// <summary>
    /// 金鑰失效時間（UTC），到期後不再簽發/解密（null 表示永久有效）。
    /// </summary>
    public DateTime? ValidTo { get; set; }

    /// <summary>
    /// 金鑰是否現役啟用，可控制金鑰輪替、灰度測試等情境。
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 金鑰建立時間（UTC），資料表預設 now()。
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
