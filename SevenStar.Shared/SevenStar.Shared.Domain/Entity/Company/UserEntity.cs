using SevenStar.Shared.Domain.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Entity.Company;

public class UserEntity : ICompanyDbContext
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;
}