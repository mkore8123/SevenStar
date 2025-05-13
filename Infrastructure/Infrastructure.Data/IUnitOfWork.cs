using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
}
