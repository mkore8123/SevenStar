using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Common.Api.Authen.Jwt.Exception;

/// <summary>
/// 表示 JWT 格式錯誤的例外。
/// </summary>
[Serializable]
public class InvalidJwtException : FormatException
{
    public InvalidJwtException() { }

    public InvalidJwtException(string message)
        : base(message) { }

    public InvalidJwtException(string message, System.Exception innerException)
        : base(message, innerException) { }

    protected InvalidJwtException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
