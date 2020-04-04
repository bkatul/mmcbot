using CoreApiAdoDemo.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MMCApi.Controllers;
using MMCApi.Model;
using MMCApi.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MMCApi.AuthorizeServices
{
    public interface IUserService
    {
        Task Authenticate(string username, string password, HttpContext context);
        string GenerateTokenForSignup(string username, string password);
    }
    public class UserService : IUserService
    {
        #region Variable

        private readonly MySettingsModel _appSettings;
        private readonly JsonSerializerSettings _serializerSettings;
        UseDbClient objClient = new UseDbClient();
        List<UserModel> lstClient = new List<UserModel>();
        private SecurityController _security;


        #endregion

        #region Constructor
        public UserService(IOptions<MySettingsModel> appSettings)
        {
            _appSettings = appSettings.Value;
            _security = new SecurityController();

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }
        #endregion

        #region Authentication and Token Access

        // Authenticate User and Get Access Token
        public Task Authenticate(string username, string password, HttpContext context)
        {
            if (!context.Request.Method.Equals("POST"))
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync(JsonConvert.SerializeObject
                    (new { success = false, error = "Bad request." }));
            }
            return GenerateToken(context, username, password);
        }

        public string GenerateTokenForSignup(string username, string password)
        {
            var encodedJwt = "";
            string users = objClient.GetUserByEmail(_appSettings.DbConn, username);

            if (_security.Decrypt(users).Equals(password))
            {
                lstClient = objClient.GetUserByEmailPassword(_appSettings.DbConn, username, _security.Encrypt(password));

                // return null if user not found
                if (lstClient.Count == 0)
                {
                    return null;
                }

                List<AuthModel> user = new List<AuthModel>
                {
                    new AuthModel { Id = lstClient[0].Id,
                        FirstName = lstClient[0].FirstName,
                        LastName = lstClient[0].LastName,
                        Username = lstClient[0].EmailId,
                        Password = lstClient[0].Password,
                    }
                };
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var now = DateTime.UtcNow;
                var claims = new Claim[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                };

                // Create the JWT and write it to a string
                var jwt = new JwtSecurityToken(
                    claims: claims,
                    notBefore: now,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature));

                encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            }
            return encodedJwt.ToString();
        }
        private async Task GenerateToken(HttpContext context, string username, string password)
        {
            string users = objClient.GetUserByEmail(_appSettings.DbConn, username);

            if (_security.Decrypt(users).Equals(password))
            {
                lstClient = objClient.GetUserByEmailPassword(_appSettings.DbConn, username, _security.Encrypt(password));

                // return null if user not found
                if (lstClient.Count == 0)
                {
                    var responsefail = new
                    {
                        success = false,
                        message = "Invalid User Name or Password",
                        //access_token = "",
                    };
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(responsefail, _serializerSettings));
                    return;
                }

                List<AuthModel> user = new List<AuthModel>
                {
                    new AuthModel { Id = lstClient[0].Id,
                        FirstName = lstClient[0].FirstName,
                        LastName = lstClient[0].LastName,
                        Username = lstClient[0].EmailId,
                        Password = lstClient[0].Password
                    }
                };
                // return null if user not found
                if (user == null)
                {
                    var responsefail = new
                    {
                        success = false,
                        message = "User not found.",
                        //   access_token = "",
                    };
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(responsefail, _serializerSettings));
                    return;
                }

                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var now = DateTime.UtcNow;
                var claims = new Claim[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                };

                // Create the JWT and write it to a string
                var jwt = new JwtSecurityToken(
                    claims: claims,
                    notBefore: now,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature));

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    success = true,
                    access_token = encodedJwt,
                };

                // Serialize and return the response
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonConvert.SerializeObject(response, _serializerSettings));
                return;
            }
            else
            {
                var responsefail = new
                {
                    success = false,
                    error = "Incorrect Password entered.",
                    //access_token = "",
                };
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(responsefail, _serializerSettings));
                return;
            }
        }

        #endregion
    }
}
