//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace meetings_app_server.Services;

//public class JwtService
//{
//    public string GenerateToken(string email, Guid userId, string dynamicKey)
//    {
//        // Generate a security key from the dynamicKey
//        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(dynamicKey));
//        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//        // Define claims for the JWT
//        var claims = new[]
//        {
//            new Claim(JwtRegisteredClaimNames.Sub, email),
//            new Claim("userId", userId.ToString())
//        };

//        // Create the JWT token
//        var token = new JwtSecurityToken(
//            issuer: "localhost:5000",
//            audience: "localhost:5000",
//            claims: claims,
//            expires: DateTime.UtcNow.AddHours(1),
//            signingCredentials: credentials
//        );

//        // Return the serialized token
//        return new JwtSecurityTokenHandler().WriteToken(token);
//    }
//}


using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace meetings_app_server.Services
{
    public class JwtService
    {
        public string GenerateToken(string email, Guid userId, string dynamicKey)
        {
            // Generate a security key from the dynamicKey
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(dynamicKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Define claims for the JWT
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim("userId", userId.ToString())
            };

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: "localhost:5000",
                audience: "localhost:5000",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            // Return the serialized token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
