public bool Main(Operand leftOperand, Operand rightOperand)
{
    bool result;

    result = (leftOperand != null && leftOperand.Value != null ? (IEnumerable<string>)leftOperand.Value : Enumerable.Empty<string>()) == (string)rightOperand.Value;
    return result;

}