﻿using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Orbit.Core.DataAccess;
using Orbit.Models.DTOs;
using Orbit.Models.OrbitDB;
using Orbit.Models.Settings;
using System.Collections;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OrbitAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly GoogleSetting googleSetting;
		private readonly JwtSetting jwtSetting;
		private readonly IUserService userService;

		public AuthController(IOptions<GoogleSetting> googleSettingOption, IOptions<JwtSetting> jwtSettingOption, IUserService userService)
		{
			this.googleSetting = googleSettingOption.Value;
			this.jwtSetting = jwtSettingOption.Value;
			this.userService = userService;
		}

		[HttpPost("LoginWithGoogle")]
		public async Task<IActionResult> LoginWithGoogle(RegisterRequest credential)
		{
			User? user = null;
			if (!string.IsNullOrEmpty(credential.GoogleToken) || !Debugger.IsAttached)
			{
				var settings = new GoogleJsonWebSignature.ValidationSettings()
				{
					Audience = new List<string> { this.googleSetting.GoogleClientId }
				};

				var payload = await GoogleJsonWebSignature.ValidateAsync(credential.GoogleToken, settings);

				user = await userService.GetUserByEmail(payload.Email);
				user.Picture = payload.Picture;
			}
			else
			{
				user = await userService.GetUserByEmail(credential.Email);
			}

			if (user != null)
			{
				return Ok(JWTGenerator(user));
			}
			else
			{
				return BadRequest();
			}
		}

		[HttpPost("RegisterWithGoogle")]
		public async Task RegisterWithGoogle(RegisterRequest credential)
		{
			var settings = new GoogleJsonWebSignature.ValidationSettings()
			{
				Audience = new List<string> { this.googleSetting.GoogleClientId }
			};

			var payload = await GoogleJsonWebSignature.ValidateAsync(credential.GoogleToken, settings);

			var user = new User()
			{
				Created = DateTime.Now,
				CreatedBy = 1,
				Updated = DateTime.Now,
				UpdatedBy = 1,
				Email = payload.Email,
				FirstName = payload.GivenName,
				LastName = payload.FamilyName,
				IsActive = true,
			};

			await this.userService.AddUser(user);
		}

		private dynamic JWTGenerator(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(this.jwtSetting.Key);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.Name, user.ID.ToString()),

					new Claim("id", user.ID.ToString()),
					new Claim(ClaimTypes.Role, "Admin")
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

			var token = tokenHandler.CreateToken(tokenDescriptor);
			var encrypterToken = tokenHandler.WriteToken(token);

			SetJWT(encrypterToken);

			var refreshToken = GenerateRefreshToken();

			SetRefreshToken(refreshToken, user);

			return new { token = encrypterToken, refreshToken = refreshToken, username = user.ID };
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
