public bool Main(IDictionary<string, Operand> conditions)
{
    Operand leftOperand;
    bool cnd0Result;
    bool cnd1Result;

    conditions.TryGetValue("NumberOfSales", out leftOperand);

    if (leftOperand == null || leftOperand.Value == null)
    {
        cnd0Result = true;
        goto cnd0LabelEndValueConditionNode;
    }
    cnd0Result = true;
cnd0LabelEndValueConditionNode:

    conditions.TryGetValue("IsoCountryCode", out leftOperand);

    if (leftOperand == null || leftOperand.Value == null)
    {
        cnd1Result = true;
        goto cnd1LabelEndValueConditionNode;
    }
    cnd1Result = true;
cnd1LabelEndValueConditionNode:
    bool Result = cnd0Result || cnd1Result;
    return Result;

}