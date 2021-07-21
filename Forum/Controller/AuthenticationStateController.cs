using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Forum.Data;
using System.Security.Claims;
using Forum.Entity;

namespace Forum.Controller
{
	public class AuthenticationStateController : AuthenticationStateProvider
	{
		private readonly IJSRuntime jsRuntime;
		private readonly IUserService userService;

		private User cachedUser;

		public AuthenticationStateController(IJSRuntime jsRuntime, IUserService userService)
		{
			this.jsRuntime = jsRuntime;
			this.userService = userService;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			if (null == cachedUser)
			{
				string token = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "SESSION_ID");

				// Not important to check if token is null or empty
				cachedUser = userService.GetUserFromSessionToken(token);

				if (null == cachedUser)
				{
					return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
				}
			}

			ClaimsIdentity identity = SetupClaims(cachedUser);

			ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
			return await Task.FromResult(new AuthenticationState(claimsPrincipal));
		}

		public async Task ValidateLogin(string identifier, string password)
		{
			User user = userService.ValidateUser(identifier, password);

			ClaimsIdentity identity = SetupClaims(user);

			string rndToken = userService.StoreSessionToken(user);
			await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "SESSION_ID", rndToken);
			
			cachedUser = user;

			NotifyAuthenticationStateChanged(
				Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
		}

		public async Task Logout()
		{
			userService.RemoveSession(cachedUser);
			cachedUser = null;
			await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "SESSION_ID", "");
			var user = new ClaimsPrincipal(new ClaimsIdentity());
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
		}

		private ClaimsIdentity SetupClaims(User user)
		{
			List<Claim> claims = new List<Claim>()
			{
				new Claim(ClaimTypes.Name, user.AccountName),
				new Claim(ClaimTypes.Role, "user"),
				new Claim("Id", user.Id.ToString()),
				new Claim("admin", "true"),
				new Claim("poster", "true"),
			};

			// "Authentication Type": If you don't pass a string here, authentication using the <AuthorizeView>-tags
			// won't work.
			return new ClaimsIdentity(claims, "Authentication Type");
		}
	}
}
