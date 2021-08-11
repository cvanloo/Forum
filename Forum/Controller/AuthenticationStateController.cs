using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;
using Forum.Data;
using System.Security.Claims;
using Forum.Entity;

namespace Forum.Controller
{
	public class AuthenticationStateController : AuthenticationStateProvider
	{
		private readonly IJSRuntime _jsRuntime;
		private readonly IUserService _userService;

		private User _cachedUser;

		public AuthenticationStateController(IJSRuntime jsRuntime, IUserService userService)
		{
			_jsRuntime = jsRuntime;
			_userService = userService;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			if (_cachedUser is null)
			{
				var token = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "SESSION_ID");

				// Not important to check if token is null or empty
				_cachedUser = _userService.GetUserFromSessionToken(token);

				if (_cachedUser is null)
				{
					return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
				}
			}

			var identity = SetupClaims(_cachedUser);

			var claimsPrincipal = new ClaimsPrincipal(identity);
			return await Task.FromResult(new AuthenticationState(claimsPrincipal));
		}

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

		public async Task Logout()
		{
			_userService.RemoveSession(_cachedUser);
			_cachedUser = null;
			await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "SESSION_ID", "");
			var user = new ClaimsPrincipal(new ClaimsIdentity());
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
		}

		private ClaimsIdentity SetupClaims(User user)
		{
			var claims = new List<Claim>()
			{
				new Claim(ClaimTypes.Name, user.AccountName),
				new Claim(ClaimTypes.Role, "user"),
				new Claim("admin", "true"),
				new Claim("poster", "true")
			};

			// "Authentication Type": If you don't pass a string here, authentication using the 
			// <AuthorizeView>-tags won't work.
			return new ClaimsIdentity(claims, "Authentication Type");
		}
	}
}
