using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Token.Jwt;

public class SampleMemberModel
{
    public string UserId { get; set; } = default!;

    public string Role { get; set; } = default!;

    public string? Email { get; set; }

    public int TokenVersion { get; set; } = 0;
}
