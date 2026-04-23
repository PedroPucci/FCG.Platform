using FCG.Platform.Domain.Entities.Dto;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Interfaces.Services;
using FCG.Platform.Domain.OperationResult;
using FCG.Platform.Infrastracture.Repository.RepositoryUoW;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FCG.Platform.Application.Services
{
    public class AuthenticationService : IAuthenticationUserService
    {
        private readonly IRepositoryUoW _repositoryUoW;

        public AuthenticationService(IRepositoryUoW repositoryUoW)
        {
            _repositoryUoW = repositoryUoW;
        }

        public async Task<Result<string>> Login(UserForAuthenticationDTO userEntity)
        {
            var response = await _repositoryUoW.UserRepository.GetByEmail(userEntity.Email);
            var result = await _repositoryUoW.UserRepository.CheckPassword(response, userEntity.Password);

            var token = await CreateAccessTokenAsync(response);
            return Result<string>.Ok(token);
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken
            (
                issuer: "PedroIghor",
                audience: "https://localhost:5001",
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(3600),
                signingCredentials: signingCredentials
            );

            return tokenOptions;
        }
        private SymmetricSecurityKey JwtSecret() => new(Encoding.UTF8.GetBytes("EAA4Cf4JnqYwBP9MSZC8cHvMSvmShHZBU27qQxZBS3ORNSoIdEz3me0QHZABLNBiEWtDmVLZBVeMF8QZCd"));


        private async Task<string> CreateAccessTokenAsync(UserEntity user)
        {
            var signingCredentials = new SigningCredentials(JwtSecret(), SecurityAlgorithms.HmacSha256);
            var claims = await GetUserClaimsAsync(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private async Task<List<Claim>> GetUserClaimsAsync(UserEntity user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Id.ToString())
            };

            if (!string.IsNullOrWhiteSpace(user.Name))
                claims.Add(new Claim(ClaimTypes.GivenName, user.Name));

            return claims;
        }
    }
}