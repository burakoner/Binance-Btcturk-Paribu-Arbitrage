using Gizza.Data.Database;

namespace Arbitrage.CoreApi
{
    public class AppConnections
    {
        /* Database Connections */
        public PocoDatabase dbConn { get; set; }

        public DatabaseEngine Engine { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public bool MultipleActiveResultSets { get; set; }
        public string ConnectionString => $"Server={Server}; Database={Database}; User Id={UserId}; Password={Password}; MultipleActiveResultSets={MultipleActiveResultSets.ToString().ToLowerInvariant()}";

        public AppConnections()
        {
            Engine = DatabaseEngine.SqlServer;
            Server = ".";
            Database = "DB-CATALOG";
            UserId = "DB-USER";
            Password = "DB-PASSWORD";
            MultipleActiveResultSets = true;

            PocoStatic.SetDefaultOptions(Engine, ConnectionString);

            // Set Properties
            dbConn = new PocoDatabase(Engine, ConnectionString);

            // Open Connections
            dbConn.OpenConnection();
        }
    }
}
