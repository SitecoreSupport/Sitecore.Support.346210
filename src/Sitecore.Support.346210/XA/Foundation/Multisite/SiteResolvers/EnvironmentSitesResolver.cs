using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.XA.Foundation.Multisite;
using Sitecore.XA.Foundation.Multisite.Comparers;
using Sitecore.XA.Foundation.Multisite.SiteResolvers;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;

namespace Sitecore.Support.XA.Foundation.Multisite.SiteResolvers
{
    public class EnvironmentSitesResolver : IEnvironmentSitesResolver
    {
        public const string AnyEnvironment = "*";

        public List<Item> ResolveAllSites(Database database)
        {
            var sites = database?.GetContentItemsOfTemplate(Templates.SiteDefinition.ID).ToList() ?? new List<Item>();
            sites.Sort(new TreeOrderComparer());
            return sites;
        }

        public List<Item> ResolveEnvironmentSites(List<Item> sites, string environment)
        {
            if (string.IsNullOrEmpty(environment) ||
                string.Equals(environment, AnyEnvironment, StringComparison.OrdinalIgnoreCase))
            {
                sites.Sort(new TreeOrderComparer());
                return sites;
            }

            sites = sites.Where(site =>
            {
                var targetEnvironment = site[Templates.SiteDefinition.Fields.Environment].Trim();
                return string.IsNullOrWhiteSpace(targetEnvironment)
                    || string.Equals(targetEnvironment, environment, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(targetEnvironment, AnyEnvironment, StringComparison.OrdinalIgnoreCase);
            }).ToList();
            sites.Sort(new TreeOrderComparer());
            return sites;
        }

        public string GetActiveEnvironment()
        {
            return Settings.GetSetting("XA.Foundation.Multisite.Environment", AnyEnvironment);
        }

        public List<string> ResolveEnvironments(List<Item> sites)
        {
            return sites.Select(s => s[Templates.SiteDefinition.Fields.Environment].Trim()).Distinct().Where(i => string.Equals(i, AnyEnvironment, StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(i)).ToList();
        }
    }
}