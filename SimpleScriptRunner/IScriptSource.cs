using System.Collections.Generic;
using System.Text;

namespace SimpleScriptRunner
{
	interface IScriptSource<T> where T : IScriptTarget
	{
		IEnumerable<IScript<T>> Scripts { get; }
	}
}
