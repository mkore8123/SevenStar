using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Service;

public interface IDomainService
{
    public IUserService User { get; }
}
