using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.AzureAD.Infrastructure
{
    /// <summary>
    /// Represents registrar of AzureAD authentication service
    /// </summary>
    public class AzureADAuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        public void Configure(AuthenticationBuilder builder)
        {
            builder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                //set credentials
                var settings = EngineContext.Current.Resolve<AzureADExternalAuthSettings>();
                options.ClientId = settings.ClientId;
                options.ClientSecret = settings.ClientSecret; // for code flow
                options.Authority = settings.Authority;
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = false };
                //store access and refresh tokens for the further usage
                options.SaveTokens = true;
                // "login", "none", "consent", "select_account"
                options.Prompt = "select_account";
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme, options => {
                options.LoginPath = AzureADAuthenticationDefaults.LoginPath;
                options.Cookie.HttpOnly = true;
            });
        }
    }
}