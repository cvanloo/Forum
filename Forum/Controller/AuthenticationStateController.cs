﻿using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;
using Forum.Data;
using System.Security.Claims;
using Forum.Entity;
// using Microsoft.Extensions.Hosting;

namespace Forum.Controller
{
	public class AuthenticationStateController : AuthenticationStateProvider
	{
		private readonly IJSRuntime _jsRuntime;
		private readonly IUserService _userService;
		// private readonly IHostEnvironment _hostEnvironment;

		private User _cachedUser;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="jsRuntime">The javascript runtime</param>
		/// <param name="userService">The user service</param>
		// /// <param name="hostEnvironment">The host environment</param>
		public AuthenticationStateController(IJSRuntime jsRuntime, IUserService userService /*, IHostEnvironment hostEnvironment */)
		{
			_jsRuntime = jsRuntime;
			_userService = userService;
			// _hostEnvironment = hostEnvironment;
		}

		/// <summary>
		/// Get the current authentication state.
		/// </summary>
		/// <returns>The authentication state.</returns>
		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			//// Automatically log the user in when in development environment
			//if (_hostEnvironment.IsDevelopment())
			//{
			//	await ValidateLogin("Testikus", "t");
			//}
			
			if (_cachedUser is null)
			{
				var token = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "SESSION_ID");

				// Not important to check if token is null or empty
				var user = _userService.GetUserFromSessionToken(token);

				if (user is null)
				{
					// Empty authentication-state equals "not logged in".
					return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
				}

				_cachedUser = user;
			}

			var identity = SetupClaims(_cachedUser);

			var claimsPrincipal = new ClaimsPrincipal(identity);
			return await Task.FromResult(new AuthenticationState(claimsPrincipal));
		}

		/// <summary>
		/// Check login credentials. If successful, the user will be logged in
		/// and a session cookie is stored, to keep the user logged in.
		/// </summary>
		/// <param name="identifier">Accountname or Email address</param>
		/// <param name="password">The users password</param>
		public async Task ValidateLogin(string identifier, string password)
		{
			var user = _userService.ValidateUser(identifier, password);

			var identity = SetupClaims(user);

			var rndToken = _userService.StoreSessionToken(user);
			await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "SESSION_ID", rndToken);
			
			_cachedUser = user;

			NotifyAuthenticationStateChanged(
				Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
		}

		/// <summary>
		/// Log out. Destroys the session cookie.
		/// </summary>
		public async Task Logout()
		{
			_userService.RemoveSession(_cachedUser);
			_cachedUser = null;
			await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "SESSION_ID", "");
			// Empty authentication-state equals "not logged in".
			var user = new ClaimsPrincipal(new ClaimsIdentity());
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
		}

		/// <summary>
		/// Set up user settings.
		/// </summary>
		/// <param name="user">User to set up</param>
		/// <returns>Users claims-identity</returns>
		private static ClaimsIdentity SetupClaims(User user)
		{
			var claims = new List<Claim>
			{
				new (ClaimTypes.Role, "user"),
				new (ClaimTypes.Name, user.AccountName),
			};
			
			foreach (var setting in user.Settings)
			{
				claims.Add(new Claim(setting.SettingKey, setting.Value));
			}

			// "Authentication Type": If you don't pass a string here, authentication using the 
			// <AuthorizeView>-tags won't work.
			return new ClaimsIdentity(claims, "Authentication Type");
		}
	}
}
