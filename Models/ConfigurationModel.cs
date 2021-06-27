using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.ExternalAuth.AzureAD.Models
{
    /// <summary>
    /// Represents plugin configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.ExternalAuth.AzureAD.Instance")]
        public string Instance { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.AzureAD.Authority")]
        public string Authority { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.AzureAD.Domain")]
        public string Domain { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.AzureAD.ClientId")]
        public string ClientId { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.AzureAD.TenantId")]
        public string TenantId { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.AzureAD.CallbackPath")]
        public string CallbackPath { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.AzureAD.ClientSecret")]
        public string ClientSecret { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.AzureAD.Oauth2AllowIdTokenImplicitFlow")]
        public bool Oauth2AllowIdTokenImplicitFlow { get; set; }
        public bool Oauth2AllowIdTokenImplicitFlow_OverrideForStore { get; set; }
    }
}