﻿using Bookstore.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Bookstore.Customer
{
    public class LocalAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string UserId = "FB6135C7-1464-4A72-B74E-4B63D343DD09";

        public LocalAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(ClaimTypes.Name, "bookstoreuser"));
            identity.AddClaim(new Claim("sub", UserId));
            identity.AddClaim(new Claim("given_name", "Bookstore"));
            identity.AddClaim(new Claim("family_name", "User"));

            await Context.SignInAsync(new ClaimsPrincipal(identity));

            await SaveCustomerDetails(Context, identity);

            Context.Response.Redirect(properties.RedirectUri);
        }

        private async Task SaveCustomerDetails(HttpContext context, ClaimsIdentity identity)
        {
            var customerService = context.RequestServices.GetService<ICustomerService>();
            var customer = new Domain.Customers.Customer
            {
                Sub = identity.FindFirst("Sub").Value,
                Username = identity.Name,
                FirstName = identity.FindFirst("given_name").Value,
                LastName = identity.FindFirst("family_name").Value
            };

            await customerService.SaveCustomerAsync(customer);
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
}