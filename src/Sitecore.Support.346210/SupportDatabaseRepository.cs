using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.DependencyInjection;
using Sitecore.XA.Foundation.Abstractions;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;

namespace Sitecore.Support.XA.Foundation.SitecoreExtensions.Repositories
{
    public class SupportDatabaseRepository : Sitecore.XA.Foundation.SitecoreExtensions.Repositories.IDatabaseRepository
    {
        protected IContext Context { get; } = ServiceLocator.ServiceProvider.GetService<IContext>();

        protected BaseFactory Factory { get; } = ServiceLocator.ServiceProvider.GetService<BaseFactory>();

        public Database GetContentDatabase()
        {
            return (from db in GetDatabases()
                    where db != null
                    select db).FirstOrDefault((Database db) => db.Name != "core");
        }

        protected IEnumerable<Database> GetDatabases()
        {
            yield return Context.ContentDatabase;
            yield return Context.Database;
            foreach (var database in Factory.GetDatabases())
            {
                if (database.ReadOnly)
                {
                    continue;
                }
                yield return database;
            }
        }

        public Database GetDatabase(string databaseName)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                return null;
            }
            return Factory.GetDatabase(databaseName);
        }

        public string[] GetDatabaseNames()
        {
            return Factory.GetDatabaseNames();
        }
    }
}