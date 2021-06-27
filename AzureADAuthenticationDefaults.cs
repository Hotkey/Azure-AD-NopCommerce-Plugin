namespace Nop.Plugin.ExternalAuth.AzureAD
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public static class AzureADAuthenticationDefaults
    {
        /// <summary>
        /// Gets a name of the view component to display login button
        /// </summary>
        public const string VIEW_COMPONENT_NAME = "AzureADAuthentication";

        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public const string SystemName = "ExternalAuth.AzureAD";

        /// <summary>
        /// Gets a name of error callback method
        /// </summary>
        public const string ErrorCallback = "/ErrorCallback";

        /// <summary>
        /// Gets a name of login path
        /// </summary>
        public const string LoginPath = "/Login";
    }
}