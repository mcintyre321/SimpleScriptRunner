using System;
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
                var required = argArray.Where(a => !a.StartsWith("-")).ToArray();
                var optional = argArray.Where(a => a.StartsWith("-"));
                bool requireRollback = optional.Any(a => a == "-requirerollback");
                bool useTransactions = optional.Any(a => a == "-usetransactions");

                var scriptTarget = required.Length > 3 ? new SqlDatabase(required[0], required[1], required[2], required[3]) : new SqlDatabase(required[0], required[1]);
                var path = required.Last();
                foreach (var releaseDirectoryPath in Directory.GetDirectories(path, "Release *"))
                {
                    var scriptSource = new FolderContainingNumberedSqlScripts(releaseDirectoryPath, "*.sql");
                    var updater = new Updater<ITextScriptTarget>(scriptSource, scriptTarget, requireRollback, useTransactions);
                    updater.ApplyScripts();
                }

                return 0;
            }
            catch (Exception ex)
            {
                WriteMessage(Assembly.GetExecutingAssembly().FullName, string.Empty, Category.error, "1", ex.ToString());
                return 1;
            }
        }

        private static void WriteMessage(string origin, string subcategory, Category category, string code, string text)
        {
            Console.WriteLine("{0} : {1} {2} {3} : {4}", origin, subcategory, category, code, text);
        }
    }
}