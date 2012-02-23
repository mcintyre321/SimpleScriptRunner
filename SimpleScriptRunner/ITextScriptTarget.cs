namespace SimpleScriptRunner
{
	interface ITextScriptTarget : IScriptTarget
	{
		void Apply(string content, ScriptVersion version);
	}
}