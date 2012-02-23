using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SimpleScriptRunner.MsSql;

namespace SimpleScriptRunner
{
    public class Program
    {
        #region Category enum

        public enum Category
        {
            error,
            warning
        }

        #endregion

        public static int Main(string[] argArray)
        {
            try
            {
                IEnumerable<string> optional = argArray.Where(a => a.StartsWith("-"));
                var required = argArray.Where(a => !a.StartsWith("-")).ToArray();
                var requireRollback = optional.Any(a => a == "-requirerollback");
                var useTransactions = optional.Any(a => a == "-usetransactions");
                Execute(required[0], required[1], required.Last(), useTransactions, requireRollback, required[2], required[3]);
                return 0;
            }
            catch (Exception ex)
            {
                WriteMessage(Assembly.GetExecutingAssembly().FullName, string.Empty, Category.error, "1", ex.ToString());
                return 1;
            }
        }

        public static void Execute(string serverName, string databaseName, string path = ".", bool useTransactions = false, bool requireRollback = false, string username = null, string password = null)
        {
            var scriptTarget = (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                                   ? new SqlDatabase(serverName, databaseName, username, password)
                                   : new SqlDatabase(serverName, databaseName);
            foreach (var releaseDirectoryPath in Directory.GetDirectories(path, "Release *"))
            {
                var scriptSource = new FolderContainingNumberedSqlScripts(releaseDirectoryPath, "*.sql");
                var updater = new Updater<ITextScriptTarget>(scriptSource, scriptTarget, requireRollback, useTransactions);
                updater.ApplyScripts();
            }
        }

        private static void WriteMessage(string origin, string subcategory, Category category, string code, string text)
        {
            Console.WriteLine("{0} : {1} {2} {3} : {4}", origin, subcategory, category, code, text);
        }
    }
}