using Infrastructure.Data;
using SevenStar.Shared.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Repository;

public interface IUserRepository : IBaseRepository<IUserRepository>
{
    Task<List<UserEntity>> GetUsersAsync();
}
