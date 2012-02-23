using System;
using System.Diagnostics;

namespace SimpleScriptRunner
{
	class Updater<T> where T:IScriptTarget
	{
		private readonly IScriptSource<T> scriptSource;
		private readonly T scriptTarget;
	    private readonly bool _requireRollback;
	    private readonly bool _useTransactions;

	    public Updater(IScriptSource<T> scriptSource, T scriptTarget, bool requireRollback, bool useTransactions)
		{
			this.scriptSource = scriptSource;
			this.scriptTarget = scriptTarget;
		    _requireRollback = requireRollback;
	        _useTransactions = useTransactions;
		}


		internal void ApplyScripts()
		{
			var currentVersion = scriptTarget.CurrentVersion;
			foreach (var script in scriptSource.Scripts)
			{
				if (script.Version.CompareTo(currentVersion) > 0)
				{
					Console.WriteLine("Running: " + script.ToString());
                    script.Apply(scriptTarget, _requireRollback, _useTransactions);
					currentVersion = scriptTarget.CurrentVersion;
				}else
				{
					Console.WriteLine("Skipping: " + script.ToString());
					
				}
			}
		}
	}
}