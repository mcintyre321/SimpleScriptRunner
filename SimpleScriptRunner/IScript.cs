namespace SimpleScriptRunner
{
	internal interface IScript<T> where T : IScriptTarget
	{
		ScriptVersion Version { get; }
		void Apply(T scriptTarget, bool requireRollback, bool useTransactions);
	}
}