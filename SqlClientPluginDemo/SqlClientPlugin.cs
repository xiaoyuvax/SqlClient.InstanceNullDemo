using DemoPlugin;
using System.Data;
using System.Data.Common;

namespace SqlClientPluginDemo
{
    public class SqlClientPlugin : IPlugin
    {
        static SqlClientPlugin()
        {
            DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", "Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient");
        }


        public void Start()
        {
            // Store factory and connection string
            var providerName = "Microsoft.Data.SqlClient";
            var factory = DbProviderFactories.GetFactory(providerName);
        }
    }
}