using System.IO;
using System.Transactions;

namespace SimpleScriptRunner
{
    internal class NumberedTextScript : IScript<ITextScriptTarget>
    {
        private readonly string path;

        public NumberedTextScript(string path)
        {
            this.path = path;
            var fi = new FileInfo(path);
            Version = new ScriptVersion(
                                        int.Parse(fi.Directory.Name.Substring(fi.Directory.Name.IndexOf(' ')).Trim()),
                                        int.Parse(fi.Name.Substring(0, fi.Name.IndexOf(' ')).Trim()),
                                        fi.LastWriteTime);
        }

        #region IScript<ITextScriptTarget> Members

        public ScriptVersion Version { get; private set; }

        public void Apply(ITextScriptTarget scriptTarget, bool requireRollback, bool useTransaction)
        {
            var prevVersion = scriptTarget.CurrentVersion;
            var sql = File.ReadAllText(path);
            TransactionScope ts = null;
            if (useTransaction)
            {
                ts = new TransactionScope(TransactionScopeOption.RequiresNew);
            }
            var success = false;
            try
            {
                
                scriptTarget.Apply(sql, Version);
                if (requireRollback)
                {
                    var rollbackSql = File.ReadAllText(path.Replace(".sql", ".rollback.sql"));

                    scriptTarget.Apply(sql, Version);
                    scriptTarget.Apply(rollbackSql, prevVersion);
                    scriptTarget.Apply(rollbackSql, prevVersion);
                    scriptTarget.Apply(sql, Version);
                }
                success = true;
            }
            finally
            {
                if (ts != null)
                {
                    if (success)
                    {
                        ts.Complete();
                    }
                    ts.Dispose();
                }
            }
        }

        #endregion

        public override string ToString()
        {
            return path;
        }
    }
}