using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Service;

public interface IUserService
{
    public Task CreateAsync(string name);
}
