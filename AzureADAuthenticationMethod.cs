﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.ExternalAuth.AzureAD
{
    /// <summary>
    /// Represents method for the authentication with AzureAD account
    /// </summary>
    public class AzureADAuthenticationMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public AzureADAuthenticationMethod(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/AzureADAuthentication/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return AzureADAuthenticationDefaults.VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new AzureADExternalAuthSettings());

            //locales
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.ExternalAuth.AzureAD.ClientKeyIdentifier"] = "App ID/API Key",
                ["Plugins.ExternalAuth.AzureAD.ClientKeyIdentifier.Hint"] = "Enter your app ID/API key here. You can find it on your AzureAD application page.",
                ["Plugins.ExternalAuth.AzureAD.ClientSecret"] = "App Secret",
                ["Plugins.ExternalAuth.AzureAD.ClientSecret.Hint"] = "Enter your app secret here. You can find it on your AzureAD application page.",
                ["Plugins.ExternalAuth.AzureAD.Instructions"] = "<p>To configure authentication with AzureAD, please follow these steps:<br/><br/><ol><li>Navigate to the <a href=\"https://developers.AzureAD.com/apps\" target =\"_blank\" > AzureAD for Developers</a> page and sign in. If you don't already have a AzureAD account, use the <b>Sign up for AzureAD</b> link on the login page to create one.</li><li>Tap the <b>+ Add a New App button</b> in the upper right corner to create a new App ID. (If this is your first app with AzureAD, the text of the button will be <b>Create a New App</b>.)</li><li>Fill out the form and tap the <b>Create App ID button</b>.</li><li>The <b>Product Setup</b> page is displayed, letting you select the features for your new app. Click <b>Get Started</b> on <b>AzureAD Login</b>.</li><li>Click the <b>Settings</b> link in the menu at the left, you are presented with the <b>Client OAuth Settings</b> page with some defaults already set.</li><li>Enter \"{0:s}signin-AzureAD\" into the <b>Valid OAuth Redirect URIs</b> field.</li><li>Click <b>Save Changes</b>.</li><li>Click the <b>Dashboard</b> link in the left navigation.</li><li>Copy your App ID and App secret below.</li></ol><br/><br/></p>"
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<AzureADExternalAuthSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.ExternalAuth.AzureAD");

            await base.UninstallAsync();
        }

        #endregion
    }
}