
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
            obj.Sort(new TreeOrderComparer());
            return obj;
        }

        public IList<Item> ResolveEnvironmentSites(List<Item> sites, string environment)
        {
            if (string.IsNullOrEmpty(environment) ||
                string.Equals(environment, "*", StringComparison.OrdinalIgnoreCase))
            {
                sites.Sort(new TreeOrderComparer());
                return sites;
            }

            sites = sites.Where(delegate (Item site)
            {
                string text = site[Templates.SiteDefinition.Fields.Environment].Trim();
                return string.IsNullOrWhiteSpace(text) ||
                       string.Equals(text, environment, StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(text, "*", StringComparison.OrdinalIgnoreCase);
            }).ToList();
            sites.Sort(new TreeOrderComparer());
            return sites;
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
    }
}