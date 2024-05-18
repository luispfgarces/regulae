namespace Regulae.Rql.Pipeline.Scan
{
    internal interface ITokenScanner
    {
        ScanResult ScanTokens(string source);
    }
}