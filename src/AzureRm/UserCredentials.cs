using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;

namespace LINQPadHelpers.AzureRm
{
    public class UserCredentials : AzureCredentials
    {
        private readonly PromptBehavior promptBehavior;
        private readonly IDictionary<Uri, ServiceClientCredentials> credentialsCache;
        private static readonly Regex VaultHostRegex = new Regex(@"([^\.]+\.)?(vault\..+)", RegexOptions.IgnoreCase);

        public const string WellKnownClientId = "1950a258-227b-4e31-a9cf-717495945fc2";

        public UserCredentials(string tenantId, AzureEnvironment environment, PromptBehavior promptBehavior) : base((UserLoginInformation)null, tenantId, environment)
        {
            this.promptBehavior = promptBehavior;
            this.credentialsCache = new Dictionary<Uri, ServiceClientCredentials>();
        }

        static UserCredentials()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tokenAudience = new Uri(this.Environment.ManagementEndpoint);

            if (request.RequestUri.AbsoluteUri.StartsWith(this.Environment.GraphEndpoint,
                StringComparison.OrdinalIgnoreCase))
            {
                tokenAudience = new Uri(this.Environment.GraphEndpoint);
            }
            else
            {
                var hostMatch = VaultHostRegex.Match(request.RequestUri.Authority);
                if (hostMatch.Success)
                {
                    tokenAudience = new Uri($"{request.RequestUri.Scheme}://{hostMatch.Groups[2].Value}");
                }
            }

            if (!this.credentialsCache.ContainsKey(tokenAudience))
            {
                var clientSettings = new ActiveDirectoryClientSettings
                {
                    ClientId = WellKnownClientId,
                    ClientRedirectUri = new Uri("urn:ietf:wg:oauth:2.0:oob"),
                    PromptBehavior = this.promptBehavior
                };

                var serviceSettings = ActiveDirectoryServiceSettings.Azure;
                serviceSettings.TokenAudience = tokenAudience;

                this.credentialsCache[tokenAudience] = await KvTokenProvider.LoginWithPromptAsync(this.TenantId, clientSettings, serviceSettings, TokenCache.DefaultShared);
            }

            await this.credentialsCache[tokenAudience].ProcessHttpRequestAsync(request, cancellationToken);
        }

        public async Task<IAzure> BuildAzureClientAsync(string subscriptionName)
        {
            Trace.WriteLine($"Getting subscription info for {subscriptionName}");

            var subscription = await Azure.Authenticate(this)
                .Subscriptions
                .ListAsync()
                .Then(x => x.FirstOrDefault(y => y.DisplayName.Equals(subscriptionName, StringComparison.InvariantCultureIgnoreCase)));

            return Azure.Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(this)
                .WithSubscription(subscription.SubscriptionId);
        }
    }
}