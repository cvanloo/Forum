using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Forum.Data;
using System.Security.Claims;
using Forum.Entity;
using System.Text.Json;

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
			var identity = new ClaimsIdentity();

			if (null == cachedUser)
			{
				string userAsJson = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "currentUser");

				if (!string.IsNullOrEmpty(userAsJson))
				{
					User user = JsonSerializer.Deserialize<User>(userAsJson);
					cachedUser = user;
				}
				else
				{
					return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
				}
			}

			identity = SetupClaims(cachedUser);

			ClaimsPrincipal cachedClaimsPrincipal = new ClaimsPrincipal(identity);
			return await Task.FromResult(new AuthenticationState(cachedClaimsPrincipal));
		}

		public void ValidateLogin(string username, string password)
		{
			User user = userService.ValidateUser(username, password);

			ClaimsIdentity identity = SetupClaims(user);

			string userAsJson = JsonSerializer.Serialize(user);
			jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", userAsJson);
			
			cachedUser = user;

			NotifyAuthenticationStateChanged(
				Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
		}

		public void Logout()
		{
			cachedUser = null;
			jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", "");
			var user = new ClaimsPrincipal(new ClaimsIdentity());
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
		}

		private ClaimsIdentity SetupClaims(User user)
		{
			List<Claim> claims = new List<Claim>()
			{
				new Claim(ClaimTypes.Name, user.AccountName),
				new Claim(ClaimTypes.Role, "user")
			};

			// "Authentication Type": Apparently, if you don't pass a string here, authentication using the <AuthorizeView>-tags
			// won't work.
			return new ClaimsIdentity(claims, "Authentication Type");
		}
	}
}
