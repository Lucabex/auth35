using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using auth35.Models;
using Microsoft.IdentityModel.Tokens;

namespace auth35.Services;

public class JwtServices
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;

    public JwtServices(IConfiguration config)
    {
        _config = config;
        var secretKey = _config["JwtSettings:SecretKey"];
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));


    }
    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Name,user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())

        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _config["JwtSettings:Issuer"],
            Audience = _config["JwtSettings:Audience"],
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JwtSettings:ExpInMinutes"])),
            SigningCredentials = new SigningCredentials(_key , SecurityAlgorithms.HmacSha256),
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}