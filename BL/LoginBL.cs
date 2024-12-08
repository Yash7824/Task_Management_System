using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Task_Management_System.Constants;
using Task_Management_System.Models;
using Task_Management_System.Repositories;
using Task_Management_System.Services;

namespace Task_Management_System.BL
{
    public class LoginBL : ITokenRepository
    {
        public async Task<LoginRS> ValidateUser(Login login, IUserRepository userRepository, HttpClient client)
        {
            var usersObj = new GetUsers();
            var currentUser = new User();
            var response = new LoginRS();

            try
            {
                usersObj = await userRepository.GetUsersAsync();
                if(usersObj.Users.Count > 0)
                {
                    currentUser = usersObj.Users.FirstOrDefault(x => x.user_email == login.user_email && CommonMethod.DecryptAES(x.user_password) == login.user_password);
                }

                if (currentUser == null) return new LoginRS
                {
                    status = "Failed",
                    statusCode = 0,
                    statusMessage = "No such user exists",
                    Token = ""
                };

                var token = GenerateToken(currentUser);
                
                if (token == null)
                {
                    response.status = "Failed";
                    response.statusCode = 0;
                    response.statusMessage = "Unsuccessfull attempt to login since token didn't get generated.";
                    response.Token = "";
                }
                else
                {
                    response.status = "Success";
                    response.statusCode = 1;
                    response.statusMessage = "Successfully logged in";
                    response.Token = token;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return response;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(TaskConstant.JWT_Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("user_id", user.user_id.ToString()),
                    new Claim("username", user.username),
                    new Claim("user_email", user.user_email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string? GenerateTokenPrev(User user)
        {
            var key = TaskConstant.JWT_Key;
            var issuer = TaskConstant.JWT_Issuer;
            var audience = TaskConstant.JWT_Audience;

            if (key != null && user.username != null && user.user_email != null)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                #region OldClaims
                //var claims = new[]
                //{
                //    new Claim(ClaimTypes.NameIdentifier, user.user_id.ToString()),
                //    new Claim(ClaimTypes.Name, user.username),
                //    new Claim(ClaimTypes.Email, user.user_email),
                //};
                #endregion

                var claims = new[]
                {
                    new Claim("user_id", user.user_id.ToString()),
                    new Claim("username", user.username),
                    new Claim("user_email", user.user_email),
                };

                var token = new JwtSecurityToken(issuer,
                    audience,
                    claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);

            }

            return null;

        }
        public async Task<ValidateTokenRS> ValidateTokenAsync(string token)
        {
            var oValidateTokenRS = new ValidateTokenRS();
            try
            {
                var _secretKey = TaskConstant.JWT_Key;
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                    ValidateLifetime = true
                };

                var principal = await Task.Run(() =>
                    tokenHandler.ValidateToken(token, validationParameters, out _)
                );

                var userId = principal.FindFirst("user_id")?.Value;

                oValidateTokenRS.IsValid = true;
                oValidateTokenRS.user_id = Guid.Parse(!string.IsNullOrEmpty(userId) ? userId : Guid.Empty.ToString());
                oValidateTokenRS.errorMessage = "";
            }
            catch (Exception ex)
            {
                oValidateTokenRS.IsValid = false;
                oValidateTokenRS.user_id = Guid.Empty;
                oValidateTokenRS.errorMessage = ex.Message;
            }

            return oValidateTokenRS;

            
        }

    }
}
