public bool Main(IDictionary<string, Operand> conditions)
{
    Operand leftOperand;
    bool cnd0Result;
    bool cnd1Result;

    conditions.TryGetValue("NumberOfSales", out leftOperand);
    cnd0Result = true;

    conditions.TryGetValue("IsoCountryCode", out leftOperand);
    cnd1Result = true;
    bool Result = cnd0Result && cnd1Result;
    return Result;

}