using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;

namespace LINQPadHelpers.AzureRm
{
    public class KvTokenProvider : ITokenProvider
    {
        /// <summary>
        /// Uri parameters used in the credential prompt.  Allows recalling previous 
        /// logins in the login dialog.
        /// </summary>
        private readonly string tokenAudience;
        private readonly AuthenticationContext authenticationContext;
        private readonly string clientId;
        private readonly UserIdentifier userid;

        /// <summary>
        /// Create a token provider which can provide user tokens in the given context.  The user must have previously authenticated in the given context. 
        /// Tokens are retrieved from the token cache.
        /// </summary>
        /// <param name="context">The active directory authentication context to use for retrieving tokens.</param>
        /// <param name="clientId">The active directory client Id to match when retrieving tokens.</param>
        /// <param name="tokenAudience">The audience to match when retrieving tokens.</param>
        /// <param name="userId">The user id to match when retrieving tokens.</param>
        public KvTokenProvider(AuthenticationContext context, string clientId, Uri tokenAudience, UserIdentifier userId)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            if (tokenAudience == null)
            {
                throw new ArgumentNullException(nameof(tokenAudience));
            }
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            this.authenticationContext = context;
            this.clientId = clientId;
            this.tokenAudience = tokenAudience.OriginalString;
            this.userid = userId;
        }

        /// <summary>
        /// Log in to Azure active directory with both user account and authentication credentials provided by the user.  This call may display a 
        /// credentials dialog, depending on the supplied client settings and the state of the token cache and user cookies.
        /// </summary>
        /// <param name="domain">The domain to authenticate against.</param>
        /// <param name="clientSettings">The client settings to use for authentication. These determine when a dialog will be displayed.</param>
        /// <param name="serviceSettings">The settings for ad service, including endpoint and token audience</param>
        /// <param name="cache">The token cache to target during authentication.</param>
        /// <returns>A ServiceClientCredentials object that can be used to authenticate http requests using the given credentials.</returns>
        public static async Task<ServiceClientCredentials> LoginWithPromptAsync(string domain,
            ActiveDirectoryClientSettings clientSettings,
            ActiveDirectoryServiceSettings serviceSettings, TokenCache cache)
        {
            return await LoginWithPromptAsync(domain, clientSettings, serviceSettings,
                UserIdentifier.AnyUser, cache);
        }

        /// <summary>
        /// Log in to Azure active directory with credentials provided by the user.  This call may display a credentials 
        /// dialog, depending on the supplied client settings and the state of the token cache and user cookies.
        /// </summary>
        /// <param name="domain">The domain to authenticate against.</param>
        /// <param name="clientSettings">The client settings to use for authentication. These determine when a dialog will be displayed.</param>
        /// <param name="serviceSettings">The settings for ad service, including endpoint and token audience</param>
        /// <param name="userId">The userid of the desired credentials</param>
        /// <param name="cache">The token cache to target during authentication.</param>
        /// <returns>A ServiceClientCredentials object that can be used to authenticate http requests using the given credentials.</returns>
        public static async Task<ServiceClientCredentials> LoginWithPromptAsync(string domain, ActiveDirectoryClientSettings clientSettings,
            ActiveDirectoryServiceSettings serviceSettings, UserIdentifier userId, TokenCache cache)
        {
            return await LoginWithPromptAsync(domain, clientSettings, serviceSettings, userId, cache, () => { return TaskScheduler.FromCurrentSynchronizationContext(); });
        }

        /// <summary>
        /// Log in to Azure active directory with credentials provided by the user.  This call may display a credentials 
        /// dialog, depending on the supplied client settings and the state of the token cache and user cookies.
        /// </summary>
        /// <param name="domain">The domain to authenticate against.</param>
        /// <param name="clientSettings">The client settings to use for authentication. These determine when a dialog will be displayed.</param>
        /// <param name="serviceSettings">The settings for ad service, including endpoint and token audience</param>
        /// <param name="userId">The userid of the desired credentials</param>
        /// <param name="cache">The token cache to target during authentication.</param>
        /// <param name="taskScheduler">Scheduler needed to run the task</param>
        /// <returns></returns>
        public static async Task<ServiceClientCredentials> LoginWithPromptAsync(string domain, ActiveDirectoryClientSettings clientSettings,
            ActiveDirectoryServiceSettings serviceSettings, UserIdentifier userId, TokenCache cache, Func<TaskScheduler> taskScheduler)
        {
            var authenticationContext = GetAuthenticationContext(domain, serviceSettings, cache);

            AuthenticationResult authResult;

            try
            {
                authResult = await authenticationContext.AcquireTokenAsync(
                    serviceSettings.TokenAudience.OriginalString,
                    clientSettings.ClientId,
                    clientSettings.ClientRedirectUri,
                    new PlatformParameters(clientSettings.PromptBehavior),
                    userId,
                    clientSettings.AdditionalQueryParameters);

            }
            catch (Exception e)
            {
                throw new AuthenticationException(
                    string.Format(CultureInfo.CurrentCulture, "Authentication error while acquiring token: '{0}'", e.Message), e);
            }

            var newUserId = new UserIdentifier(authResult.UserInfo.DisplayableId, UserIdentifierType.RequiredDisplayableId);

            return new TokenCredentials(
                new KvTokenProvider(authenticationContext, clientSettings.ClientId, serviceSettings.TokenAudience, newUserId),
                authResult.TenantId,
                authResult.UserInfo.DisplayableId);
        }

        /// <summary>
        /// Gets an access token from the token cache or from AD authentication endpoint.  Will attempt to 
        /// refresh the access token if it has expired.
        /// </summary>
        public virtual async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var result = await this.authenticationContext.AcquireTokenSilentAsync(this.tokenAudience,
                    this.clientId, this.userid).ConfigureAwait(false);
                return new AuthenticationHeaderValue(result.AccessTokenType, result.AccessToken);
            }
            catch (AdalException authenticationException)
            {
                throw new AuthenticationException("Error renewing Token", authenticationException);
            }
        }

        private static AuthenticationContext GetAuthenticationContext(string domain, ActiveDirectoryServiceSettings serviceSettings, TokenCache cache)
        {
            var context = (cache == null
                ? new AuthenticationContext(serviceSettings.AuthenticationEndpoint + domain,
                    serviceSettings.ValidateAuthority)
                : new AuthenticationContext(serviceSettings.AuthenticationEndpoint + domain,
                    serviceSettings.ValidateAuthority, cache));
            return context;
        }
    }
}