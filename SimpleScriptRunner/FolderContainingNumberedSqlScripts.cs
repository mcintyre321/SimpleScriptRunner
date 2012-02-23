using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleScriptRunner
{
    internal class FolderContainingNumberedSqlScripts : IScriptSource<ITextScriptTarget>
    {
        private readonly string path;
        private readonly string searchPattern;

        public FolderContainingNumberedSqlScripts(string path, string searchPattern)
        {
            this.path = path;
            this.searchPattern = searchPattern;
        }

        #region IScriptSource<ITextScriptTarget> Members

        public IEnumerable<IScript<ITextScriptTarget>> Scripts
        {
            get
            {
                return new DirectoryInfo(path)
                    .GetFiles(searchPattern)
                    .Where(fi => !fi.FullName.EndsWith("rollback.sql"))
                    .Select(fi => new NumberedTextScript(fi.FullName))
                    .OrderBy(s => s.Version)
                    .Cast<IScript<ITextScriptTarget>>();
            }
        }

        #endregion
    }
}