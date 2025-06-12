using SevenStar.Data.Platform.PostgreSql.Repository;
using SevenStar.Shared.Domain.DbContext.Company.Repository;
using SevenStar.Shared.Domain.DbContext.Platform.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Data.Platform.PostgreSql;

public partial class PlatformDb
{
    private ICompanyRepository? _company;

    public ICompanyRepository Company => _company ??= new CompanyRepository(Connection);

    private IJwtSigningKeyRepository? _jwtSigningKey;

    public IJwtSigningKeyRepository JwtSigningKey => _jwtSigningKey ??= new JwtSigningKeyRepository(Connection);

    private IJwtTokenConfigRepository? _jwtTokenConfig;

    public IJwtTokenConfigRepository JwtTokenConfig => _jwtTokenConfig ??= new JwtTokenConfigRepository(Connection);

    private IJwtEncryptingKeyRepository? _jwtEncryptingKey;

    public IJwtEncryptingKeyRepository JwtEncryptingKey => _jwtEncryptingKey ??= new JwtEncryptingKeyRepository(Connection);
}
