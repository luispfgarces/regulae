namespace Regulae.Analyzers
{
    using System;
    using Microsoft.CodeAnalysis;

    internal static class RulesFrameworkHelpers
    {
        public static bool IsRulesFrameworkSymbol(INamespaceOrTypeSymbol symbol)
        {
            if (symbol == null)
            {
                return false;
            }

            var ns = symbol.ContainingNamespace;
            if (ns == null)
            {
                return false;
            }

            var nsString = ns.ToDisplayString();
            return nsString.StartsWith("Rules.Framework", StringComparison.Ordinal);
        }
    }
}
