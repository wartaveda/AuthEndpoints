﻿using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthEndpoints.SimpleJwt.Core.Services;

public class AccessTokenGenerator<TUser> : IAccessTokenGenerator<TUser>
    where TUser : class
{
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly SimpleJwtOptions _options;
    private readonly IClaimsProvider<TUser> _claimsProvider;

    public AccessTokenGenerator(JwtSecurityTokenHandler tokenHandler, IOptions<SimpleJwtOptions> options, IClaimsProvider<TUser> claimsProvider)
    {
        _tokenHandler = tokenHandler;
        _options = options.Value;
        _claimsProvider = claimsProvider;
    }

    public string GenerateAccessToken(TUser user)
    {
        var signingOptions = _options.AccessSigningOptions!;
        var credentials = new SigningCredentials(signingOptions.SigningKey, signingOptions.Algorithm);
        var header = new JwtHeader(credentials);
        var payload = new JwtPayload(
            _options.Issuer!,
            _options.Audience!,
            _claimsProvider.ProvideAccessClaims(user),
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(signingOptions.ExpirationMinutes)
        );
        return _tokenHandler.WriteToken(new JwtSecurityToken(header, payload));
    }
}
