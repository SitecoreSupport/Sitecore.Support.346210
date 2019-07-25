using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.XA.Foundation.Multisite;
using Sitecore.XA.Foundation.Multisite.SiteResolvers;
using Sitecore.XA.Foundation.SitecoreExtensions.Comparers;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;

namespace Sitecore.Support.XA.Foundation.Multisite.SiteResolvers
{
    public class EnvironmentSitesResolver : IEnvironmentSitesResolver
    {
        public const string AnyEnvironment = "*";
        private static readonly ID SitesManagementId = new ID(Sitecore.XA.Foundation.Multisite.Constants.SitesManagementId);

        public List<Item> ResolveAllSites(Database database)
        {
            if (database == null)
            {
                return new List<Item>();
            }
            var sitesManagementItem = database.GetItem(SitesManagementId);
            MultilistField sitesOrderField = sitesManagementItem?.Fields[Templates.SiteManagement.Fields.Order];
            var sites = database.GetContentItemsOfTemplate(Templates.SiteDefinition.ID).ToList();
            if (sitesOrderField == null)
            {
                sites.Sort(new TreeComparer());
                return sites;
            }
            else
            {
                var targetIds = sitesOrderField.TargetIDs.ToList();
                var orderedSites = new Item[targetIds.Count];
                var disorderedSites = new List<Item>();
                
                foreach (var site in sites)
                {
                    var index = targetIds.IndexOf(site.ID);
                    if (index >= 0)
                    {
                        orderedSites[index] = site;
                    }
                    else
                    {
                        disorderedSites.Add(site);
                    }
                }

                disorderedSites.Sort(new TreeComparer());
                var allSites = orderedSites.Where(site => site != default(Item)).ToList();
                allSites.AddRange(disorderedSites);
                return allSites;
            }
        }

        public List<Item> ResolveEnvironmentSites(List<Item> sites, string environment)
        {
            if (string.IsNullOrEmpty(environment) ||
                string.Equals(environment, AnyEnvironment, StringComparison.OrdinalIgnoreCase))
            {
                return sites;
            }

            sites = sites.Where(site =>
            {
                var targetEnvironment = site[Templates.SiteDefinition.Fields.Environment].Trim();
                return string.IsNullOrWhiteSpace(targetEnvironment)
                    || string.Equals(targetEnvironment, environment, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(targetEnvironment, AnyEnvironment, StringComparison.OrdinalIgnoreCase);
            }).ToList();
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