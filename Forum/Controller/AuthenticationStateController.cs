using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
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
				try
				{
					string userAsJson = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "currentUser");

					if (!string.IsNullOrEmpty(userAsJson))
					{
						User user = JsonSerializer.Deserialize<User>(userAsJson);
						ValidateLogin(user.AccountName, user.PwHash);
					}
				}
				catch (Exception ex) { }
			}
			else
			{
				identity = SetupClaims(cachedUser);
			}

			ClaimsPrincipal cachedClaimsPrincipal = new ClaimsPrincipal(identity);
			return await Task.FromResult(new AuthenticationState(cachedClaimsPrincipal));
		}

		public void ValidateLogin(string username, string password)
		{
			ClaimsIdentity identity = new ClaimsIdentity();

			User user = userService.ValidateUser(username, password);

			identity = SetupClaims(user);

			string userAsJson = JsonSerializer.Serialize(user);
			jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", userAsJson);
			
			cachedUser = user;

			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
		}

		public void Logout()
		{
			cachedUser = null;
			var user = new ClaimsPrincipal(new ClaimsIdentity());
			jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", "");
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
		}

		private ClaimsIdentity SetupClaims(User user)
		{
			List<Claim> claims = new List<Claim>()
			{
				new Claim(ClaimTypes.Name, user.AccountName),
			};

			return new ClaimsIdentity(claims);
		}
	}
}
