using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalAuth.AzureAD
{
    /// <summary>
    /// Represents settings of the AzureAD authentication method
    /// </summary>
    public class AzureADExternalAuthSettings : ISettings
    {
        /// <summary>
        /// Gets or sets Azure AD Instance
        /// https://login.microsoftonline.com/
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// Gets or sets Azure AD Authority
        /// https://login.microsoftonline.com/{TenantId}/v2.0/
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Gets or sets Azure AD Domain (mycompanyname.onmicrosoft.com)
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets Azure AD TenantId
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets Azure AD ClientId
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets Azure AD client secret
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets Azure AD CallbackPath (/signin-oidc)
        /// </summary>
        public string CallbackPath { get; set; }

        /// <summary>
        /// Gets or sets Azure AD Oauth2 AllowId Token Implicit Flow 
        /// </summary>
        public bool Oauth2AllowIdTokenImplicitFlow { get; set; }
    }
}