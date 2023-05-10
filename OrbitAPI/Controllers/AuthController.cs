using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Orbit.Core;
using Orbit.Core.DataAccess;
using Orbit.Core.Exceptions;
using Orbit.Models.DTOs;
using Orbit.Models.OrbitDB;
using Orbit.Models.Settings;
using System.Collections;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace OrbitAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly GoogleSetting googleSetting;
		private readonly JwtSetting jwtSetting;
		private readonly IUserService userService;
		private readonly IUserRoleService userRoleService;
		private readonly IUserSession userSession;

		public AuthController(
			IOptions<GoogleSetting> googleSettingOption,
			IOptions<JwtSetting> jwtSettingOption,
			IUserService userService,
			IUserRoleService userRoleService,
			IUserSession userSession)
		{
			this.googleSetting = googleSettingOption.Value;
			this.jwtSetting = jwtSettingOption.Value;
			this.userService = userService;
			this.userRoleService = userRoleService;
			this.userSession = userSession;
		}

		[HttpPost("LoginWithGoogle")]
		public async Task<dynamic> LoginWithGoogle(RegisterRequest credential)
		{
			User? user = null;
			Payload? payload = null;
			if (!string.IsNullOrEmpty(credential.GoogleToken) || !Debugger.IsAttached)
			{
				var settings = new GoogleJsonWebSignature.ValidationSettings()
				{
					Audience = new List<string> { this.googleSetting.GoogleClientId }
				};

				payload = await GoogleJsonWebSignature.ValidateAsync(credential.GoogleToken, settings);

				if (payload == null)
				{
					throw new BadRequestException("Your security token was not verified by Google, please try again.");
				}

				user = await userService.GetUserByEmail(payload.Email);
				if (user != null)
				{
					user.Picture = payload.Picture;
				}
			}
			else
			{
				user = await userService.GetUserByEmail(credential.Email);
			}

			if (user == null)
			{
				user = new User()
				{
					Email = payload.Email,
					FirstName = payload.GivenName,
					LastName = payload.FamilyName,
					IsActive = true,
				};
				this.userSession.SetCreatedAuditColumns(user);
				this.userSession.SetUpdatedAuditColumns(user);

				await this.userService.AddUser(user);
			}

			user.Permissions = await this.userRoleService.GetUserPermissions(user.ID);
			return JWTGenerator(user);
		}

		[HttpPost("LoginForOrg")]
		[Authorize]
		public async Task<dynamic> LoginWithGoogleAndOrgnaization(Organization org)
		{
			if (!this.userSession.UserID.HasValue)
			{
				throw new Exception("You are not authorized");
			}

			var user = await userService.GetUserByID(this.userSession.UserID.Value);

			if (user == null)
			{
				throw new Exception($"Your account doesn't exists, please reach out to Admin");
			}

			user.Permissions = await this.userRoleService.GetUserPermissions(user.ID, org.ID);
			return JWTGenerator(user, org);
		}

		private dynamic JWTGenerator(User user, Organization org = null)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(this.jwtSetting.Key);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.Name, user.ID.ToString()),
					new Claim("id", user.ID.ToString()),
				}),
				Expires = DateTime.Now.AddMinutes(jwtSetting.DurationInMinutes),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature),
				Audience = jwtSetting.Audience,
				Issuer = jwtSetting.Issuer,
			};

			if (!string.IsNullOrEmpty(user.Picture))
			{
				tokenDescriptor.Subject.AddClaim(new Claim("AvatarUrl", user.Picture));
			}

			if (org != null)
			{
				tokenDescriptor.Subject.AddClaim(new Claim("Organization", System.Text.Json.JsonSerializer.Serialize(org)));
			}

			if (user.Permissions != null && user.Permissions.Count > 0)
			{
				foreach (var perm in user.Permissions)
				{
					tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, perm));
				}
			}

			var token = tokenHandler.CreateToken(tokenDescriptor);
			var encrypterToken = tokenHandler.WriteToken(token);

			SetJWT(encrypterToken);

			var refreshToken = GenerateRefreshToken();

			SetRefreshToken(refreshToken, user);

			return new { token = encrypterToken, refreshToken = refreshToken, userID = user.ID };
		}

		private void SetJWT(string encrypterToken)
		{

			HttpContext.Response.Cookies.Append("X-Access-Token", encrypterToken,
				  new CookieOptions
				  {
					  Expires = DateTime.Now.AddMinutes(15),
					  HttpOnly = true,
					  Secure = true,
					  IsEssential = true,
					  SameSite = SameSiteMode.None
				  });
		}

		private RefreshToken GenerateRefreshToken()
		{
			var refreshToken = new RefreshToken()
			{
				Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
				Expires = DateTime.Now.AddDays(7),
				Created = DateTime.Now
			};

			return refreshToken;

		}

		private void SetRefreshToken(RefreshToken refreshToken, User user)
		{

			HttpContext.Response.Cookies.Append("X-Refresh-Token", refreshToken.Token,
				 new CookieOptions
				 {
					 Expires = refreshToken.Expires,
					 HttpOnly = true,
					 Secure = true,
					 IsEssential = true,
					 SameSite = SameSiteMode.None
				 });
		}
	}
}
