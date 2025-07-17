namespace Regulae.Core
{
    internal static class ConditionNodeProperties
    {
        internal static class CompilationProperties
        {
            public static string CompiledMatchDelegateKey => $"{Prefix}_compiledMatchDelegate";
            public static string CompiledSearchDelegateKey => $"{Prefix}_compiledSearchDelegate";
            public static string IsCompiledKey => $"{Prefix}_isCompiled";
            public static string Prefix => "_compilation";
        }
    }
}