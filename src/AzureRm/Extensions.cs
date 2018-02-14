using System.Text.RegularExpressions;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace LINQPadHelpers.AzureRm
{
    public static class Extensions
    {
        public static string GetResourceGroupName(this Resource resource)
        {
            return Regex.Match(resource.Id, "^.*?/resourcegroups/([^/]*)", RegexOptions.IgnoreCase).Groups[1].Value;
        }
    }
}
