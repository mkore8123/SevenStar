using Common.Api.Authentication;
using Infrastructure.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SevenStar.Shared.Domain.Api.Auth;
using SevenStar.Shared.Domain.DbContext;
using SevenStar.Shared.Domain.DbContext.Entity.Platform;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Extensions;

public static class JwtOptionExtension
{
}