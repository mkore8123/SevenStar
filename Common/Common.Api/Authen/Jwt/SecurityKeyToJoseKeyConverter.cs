using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Common.Api.Authen.Jwt;

public static class SecurityKeyToJoseKeyConverter
{
    public static object ToJoseKey(SecurityKey key)
    {
        switch (key)
        {
            case SymmetricSecurityKey symKey:
                return symKey.Key; // byte[]

            case RsaSecurityKey rsaKey:
                if (rsaKey.Rsa != null)
                    return rsaKey.Rsa;
                // 若無 Rsa 但有 Parameters
                if (!IsEmpty(rsaKey.Parameters))
                    return RSAFromParameters(rsaKey.Parameters);
                throw new ArgumentException("RsaSecurityKey 既無 Rsa 屬性，也無 Parameters 資料");

            case ECDsaSecurityKey ecdsaKey:
                if (ecdsaKey.ECDsa != null)
                    return ecdsaKey.ECDsa;
                throw new ArgumentException("ECDsaSecurityKey 必須有 ECDsa 屬性", nameof(key));

            default:
                throw new NotSupportedException($"不支援的 SecurityKey 型別：{key?.GetType().Name}");
        }
    }

    private static bool IsEmpty(RSAParameters param)
    {
        // 只要 Modulus 有值就代表參數有內容
        return param.Modulus == null || param.Modulus.Length == 0;
    }

    private static RSA RSAFromParameters(RSAParameters param)
    {
        var rsa = RSA.Create();
        rsa.ImportParameters(param);
        return rsa;
    }
}