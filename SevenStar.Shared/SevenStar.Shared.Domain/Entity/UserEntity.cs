using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Entity;

public class UserEntity
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;
}