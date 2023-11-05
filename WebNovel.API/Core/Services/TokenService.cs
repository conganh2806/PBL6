using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Services.Schemas;
using WebNovel.API.Databases.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using WebNovel.API.Areas.Models.Accounts;
using WebNovel.API.Databases.Entitites;
using WebNovel.API.Areas.Models.Accounts.Schemas;

namespace WebNovel.API.Core.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GetTokenAsync(string userId);

        Task<TokenResponse> RefreshTokenAsync(RefreshTokenQuery request);

        Task<TokenResponse> GetGoogleAccountCreateUpdateEntityAsync(string oauthCode);
    }
    public class TokenService : ITokenService
    {
        private readonly IAccountModel _accountModel;
        private readonly JwtSettings _jwtSettings;
        public TokenService(IOptions<JwtSettings> jwtSettings, IAccountModel accountModel)
        {
            _jwtSettings = jwtSettings.Value;
            _accountModel = accountModel;
        }
        public Task<TokenResponse> GetGoogleAccountCreateUpdateEntityAsync(string oauthCode)
        {
            throw new NotImplementedException();
        }

        public async Task<TokenResponse> GetTokenAsync(string userId)
        {
            return await _accountModel.GetAccount(userId) is not { } user
                ? throw new UnauthorizedAccessException("Authentication Failed.")
                : await GenerateTokensAndUpdateUser(user);
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenQuery request)
        {
            var userPrincipal = GetPrincipalFromExpiredToken(request.Token);
            string? userEmail = userPrincipal.FindFirstValue(NovelClaim.Email);
            var user = await _accountModel.FindByEmailAsync(userEmail!) ?? throw new UnauthorizedAccessException("Authentication Failed.");
            return user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow
                ? throw new UnauthorizedAccessException("Invalid Refresh Token.")
                : await GenerateTokensAndUpdateUser(user);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = NovelClaim.Role,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new UnauthorizedAccessException("Invalid Token.");
            }

            return principal;
        }

        private async Task<TokenResponse> GenerateTokensAndUpdateUser(AccountDto user)
        {
            string token = GenerateJwt(user);

            var updateUser = new AccountCreateUpdateEntity {
                Id = user.Id,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays)
            };

            await _accountModel.UpdateToken(updateUser);
            var tokenRespon = new TokenResponse {
                Token = token,
                RefreshToken = updateUser.RefreshToken,
                RefreshTokenExpiryTime = updateUser.RefreshTokenExpiryTime
            };

            return tokenRespon;
        }

        private string GenerateJwt(AccountDto user) =>
            GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user));


        private static IEnumerable<Claim> GetClaims(AccountDto user) =>
        new List<Claim>
        {
            new(NovelClaim.NameIdentifier, user.Id),
            new(NovelClaim.Email, user.Email!),
        };

        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
                signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private SigningCredentials GetSigningCredentials()
        {
            byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }
    }
}