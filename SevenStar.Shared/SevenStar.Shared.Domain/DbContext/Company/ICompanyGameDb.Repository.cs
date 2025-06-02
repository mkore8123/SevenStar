using SevenStar.Shared.Domain.DbContext.Company.Repository;
using SevenStar.Shared.Domain.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.DbContext.Company;

public partial interface ICompanyGameDb
{
    IUserRepository User { get; }
}
