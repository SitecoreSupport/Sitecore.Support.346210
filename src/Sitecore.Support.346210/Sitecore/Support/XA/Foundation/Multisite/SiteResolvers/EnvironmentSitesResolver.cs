
using Microsoft.Extensions.DependencyInjection;
using Sitecore.XA.Foundation.SitecoreExtensions.Repositories;

namespace Sitecore.Support.XA.Foundation.Multisite.SiteResolvers
{
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.DependencyInjection;
    using Sitecore.XA.Foundation.Multisite;
    using Sitecore.XA.Foundation.Multisite.Comparers;
    using Sitecore.XA.Foundation.Multisite.SiteResolvers;
    using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EnvironmentSitesResolver : IEnvironmentSitesResolver
    {
        public const string AnyEnvironment = "*";

        public IList<Item> ResolveAllSites(Database database)
        {
            List<Item> obj =
                ((database != null)
                    ? database.GetContentItemsOfTemplate(Templates.SiteDefinition.ID).ToList()
                    : null) ?? new List<Item>();
            return SortSites(obj);
        }

        public IList<Item> ResolveEnvironmentSites(List<Item> sites, string environment)
        {
            if (string.IsNullOrEmpty(environment) ||
                string.Equals(environment, "*", StringComparison.OrdinalIgnoreCase))
            {
                return SortSites(sites);
            }

            sites = sites.Where(delegate (Item site)
            {
                string text = site[Templates.SiteDefinition.Fields.Environment].Trim();
                return string.IsNullOrWhiteSpace(text) ||
                       string.Equals(text, environment, StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(text, "*", StringComparison.OrdinalIgnoreCase);
            }).ToList();
            return SortSites(sites);
        }

        public string GetActiveEnvironment()
        {
            return Settings.GetSetting("XA.Foundation.Multisite.Environment", "*");
        }

        public IList<string> ResolveEnvironments(IEnumerable<Item> sites)
        {
            return (from i in (from s in sites
                               select s[Templates.SiteDefinition.Fields.Environment].Trim()).Distinct()
                    where string.Equals(i, "*", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(i)
                    select i).ToList();
        }
        private IList<Item> SortSites(IList<Item> sites)
        {
            Item sitesManagementItem = ServiceLocator.ServiceProvider.GetService<IContentRepository>().GetItem(new ID(Constants.SitesManagementId));
            MultilistField sitesOrderField = sitesManagementItem?.Fields[Templates.SiteManagement.Fields.Order];
            var siteIds = sitesOrderField.TargetIDs.ToList();
            var orderedSites = sites.Where(s => siteIds.Contains(s.ID)).ToList();
            var disorderedSites = sites.Except(orderedSites);
            orderedSites = orderedSites.OrderBy(s => siteIds.IndexOf(s.ID)).ToList();
            orderedSites.AddRange(disorderedSites);
            return orderedSites;
        }
    }
}