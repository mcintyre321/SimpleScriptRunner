using System;
using System.Transactions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace SimpleScriptRunner.MsSql
{
    internal class SqlDatabase : ITextScriptTarget
    {
        private readonly Database database;

        public SqlDatabase(string serverName, string databaseName, string username, string password)
            : this(new Server(new ServerConnection(serverName, username, password)), databaseName)
        {
        }

        public SqlDatabase(string serverName, string databaseName)
            : this(new Server(serverName), databaseName)
        {
        }

        public SqlDatabase(Server server, string databaseName)
        {
            if (!server.Databases.Contains(databaseName))
            {
                database = new Database(server, databaseName);
                database.Create();
            }
            else
            {
                database = server.Databases[databaseName];
            }

            Console.WriteLine("CurrentVersion: " + CurrentVersion.ToString());
        }

        #region ITextScriptTarget Members

        public ScriptVersion CurrentVersion
        {
            get
            {
                return new ScriptVersion(
                    int.Parse(database.GetExtendedPropertyValue("ReleaseNumber", "0")),
                    int.Parse(database.GetExtendedPropertyValue("ScriptNumber", "0")),
                    DateTime.Parse(database.GetExtendedPropertyValue("ScriptModifiedDate", DateTime.MinValue.ToString()))
                    );
            }

            private set
            {
                database.SetExtendedPropertyValue("ReleaseNumber", value.ReleaseNumber.ToString());
                database.SetExtendedPropertyValue("ScriptNumber", value.ScriptNumber.ToString());
                database.SetExtendedPropertyValue("ScriptModifiedDate", value.Modified.ToString());
            }
        }

        public void Apply(string content, ScriptVersion version)
        {
            database.ExecuteNonQuery(content);
            CurrentVersion = version;
        }

        #endregion
    }

    internal static class DatabaseExtensions
    {
        public static string GetExtendedPropertyValue(this Database db, string epName, string defaultValue)
        {
            var ep = db.ExtendedProperties[epName];
            if (ep == null)
            {
                return defaultValue;
            }
            else
            {
                return ep.Value as string ?? defaultValue;
            }
        }

        public static void SetExtendedPropertyValue(this Database db, string epName, string value)
        {
            var ep = db.ExtendedProperties[epName];
            if (ep == null)
            {
                ep = new ExtendedProperty(db, epName);
                ep.Value = value;
                ep.Create();
            }
            else
            {
                ep.Value = value;
                ep.Alter();
            }
        }
    }
}