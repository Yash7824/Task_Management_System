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

                if (currentUser == null) 
                {
                    response.status = "Failed";
                    response.statusCode = 1;
                    response.statusMessage = "No such user exists";
                    response.Token = string.Empty;
                    return response;
                };

                var token = GenerateToken(currentUser);
                
                if (token == null)
                {
                    response.status = "Failed";
                    response.statusCode = 1;
                    response.statusMessage = "Unsuccessfull attempt to login since token didn't get generated.";
                    response.Token = string.Empty;
                }
                else
                {
                    response.status = "Success";
                    response.statusCode = 0;
                    response.statusMessage = "Successfully Logged in";
                    response.Token = token;
                }

            }
            catch (Exception ex)
            {
                response.status = "Failed";
                response.statusCode = 2;
                response.statusMessage = $"Exception occured in LoginBL.ValidateUser(): {ex.Message}";
                throw new Exception(ex.Message);
            }

            return response;
        }

        public string GenerateToken(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(TaskConstant.JWT_Key);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim("user_id", user.user_id),
                    new Claim("username", user.username),
                    new Claim("user_email", user.user_email)
                }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = TaskConstant.JWT_Issuer,
                    Audience = TaskConstant.JWT_Audience,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch(Exception ex)
            {
                throw new Exception($"Exception occurred in LoginBL.GenerateToken(): {ex.Message}", ex);
            }
        }

        public async Task<ValidateTokenRS> ValidateTokenAsync(string token)
        {
            var oValidateTokenRS = new ValidateTokenRS();
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    oValidateTokenRS.status = "Failed";
                    oValidateTokenRS.statusCode = 1;
                    oValidateTokenRS.statusMessage = $"Token Is Empty Or Null!";
                    return oValidateTokenRS;
                }

                var _secretKey = TaskConstant.JWT_Key;
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                    ValidIssuer = TaskConstant.JWT_Issuer,      
                    ValidAudience = TaskConstant.JWT_Audience,
                    ValidateLifetime = true
                };

                var principal = await Task.Run(() =>
                    tokenHandler.ValidateToken(token, validationParameters, out _)
                );

                var userId = principal.FindFirst("user_id")?.Value;

                oValidateTokenRS.status = "Success";
                oValidateTokenRS.statusCode = 0;
                oValidateTokenRS.statusMessage = "User Validated Successfully!";
                oValidateTokenRS.user_id = userId ?? string.Empty;
                
            }
            catch (Exception ex)
            {
                oValidateTokenRS.status = "Failed";
                oValidateTokenRS.statusCode = 2;
                oValidateTokenRS.statusMessage = $"Exception occurred in LoginBL.ValidateTokenAsync(): {ex.Message}";
                oValidateTokenRS.user_id = string.Empty;
            }

            return oValidateTokenRS;

            
        }

    }
}
