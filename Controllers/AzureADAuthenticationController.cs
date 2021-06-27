using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nop.Core;
using Nop.Plugin.ExternalAuth.AzureAD.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.ExternalAuth.AzureAD.Controllers
{
    public class AzureADAuthenticationController : BasePluginController
    {
        #region Fields

        private readonly AzureADExternalAuthSettings _azureADExternalAuthSettings;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IOptionsMonitorCache<OpenIdConnectOptions> _optionsCache;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AzureADAuthenticationController(AzureADExternalAuthSettings azureADExternalAuthSettings,
            IAuthenticationPluginManager authenticationPluginManager,
            IExternalAuthenticationService externalAuthenticationService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IOptionsMonitorCache<OpenIdConnectOptions> optionsCache,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _azureADExternalAuthSettings = azureADExternalAuthSettings;
            _authenticationPluginManager = authenticationPluginManager;
            _externalAuthenticationService = externalAuthenticationService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _optionsCache = optionsCache;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                Instance = _azureADExternalAuthSettings.Instance,
                Authority = _azureADExternalAuthSettings.Authority,
                Domain = _azureADExternalAuthSettings.Domain,
                TenantId = _azureADExternalAuthSettings.TenantId,
                ClientId = _azureADExternalAuthSettings.ClientId,
                ClientSecret = _azureADExternalAuthSettings.ClientSecret,
                CallbackPath = _azureADExternalAuthSettings.CallbackPath,
                Oauth2AllowIdTokenImplicitFlow = _azureADExternalAuthSettings.Oauth2AllowIdTokenImplicitFlow
            };

            return View("~/Plugins/ExternalAuth.AzureAD/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //save settings
            _azureADExternalAuthSettings.Instance = model.Instance;
            _azureADExternalAuthSettings.Authority = model.Authority;
            _azureADExternalAuthSettings.Domain = model.Domain;
            _azureADExternalAuthSettings.TenantId = model.TenantId;
            _azureADExternalAuthSettings.ClientId = model.ClientId;
            _azureADExternalAuthSettings.ClientSecret = model.ClientSecret;
            _azureADExternalAuthSettings.CallbackPath = model.CallbackPath;
            _azureADExternalAuthSettings.Oauth2AllowIdTokenImplicitFlow = model.Oauth2AllowIdTokenImplicitFlow;
            await _settingService.SaveSettingAsync(_azureADExternalAuthSettings);

            //clear AzureAD authentication options cache
            _optionsCache.TryRemove(OpenIdConnectDefaults.AuthenticationScheme);
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
            return await Configure();
        }

        public async Task<IActionResult> Login(string returnUrl)
        {
            var methodIsAvailable = await _authenticationPluginManager
                .IsPluginActiveAsync(AzureADAuthenticationDefaults.SystemName, await _workContext.GetCurrentCustomerAsync(), (await _storeContext.GetCurrentStoreAsync()).Id);
            if (!methodIsAvailable)
                throw new NopException("AzureAD authentication module cannot be loaded");

            if (string.IsNullOrEmpty(_azureADExternalAuthSettings.TenantId) ||
                string.IsNullOrEmpty(_azureADExternalAuthSettings.Instance) ||
                string.IsNullOrEmpty(_azureADExternalAuthSettings.Authority) ||
                string.IsNullOrEmpty(_azureADExternalAuthSettings.Domain) ||
                string.IsNullOrEmpty(_azureADExternalAuthSettings.ClientId))
            {
                throw new NopException("AzureAD authentication module not configured");
            }

            //configure login callback action
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginCallback", "AzureADAuthentication", new { returnUrl = returnUrl })
            };
            authenticationProperties.SetString(AzureADAuthenticationDefaults.ErrorCallback, Url.RouteUrl("Login", new { returnUrl }));

            return Challenge(authenticationProperties, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            //authenticate AzureAD user
            var authenticateResult = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded || !authenticateResult.Principal.Claims.Any())
                return RedirectToRoute("Login");

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = AzureADAuthenticationDefaults.SystemName,
                AccessToken = await HttpContext.GetTokenAsync(OpenIdConnectDefaults.AuthenticationScheme, "access_token"),
                Email = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email)?.Value,
                ExternalIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                ExternalDisplayIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value,
                Claims = authenticateResult.Principal.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList()
            };

            //authenticate Nop user
            return await _externalAuthenticationService.AuthenticateAsync(authenticationParameters, returnUrl);
        }
        #endregion
    }
}