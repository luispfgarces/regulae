namespace Regulae.Management
{
    using System.Threading.Tasks;

    internal interface IRuleSanitizer
    {
        ValueTask<OperationResult> SanitizeAsync(Rule rule);
    }
}