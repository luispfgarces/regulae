public bool Main(Operand leftOperand, Operand rightOperand)
{
    bool result;

    result = (leftOperand != null && leftOperand.Value != null ? (string)leftOperand.Value : (string)null) == (string)rightOperand.Value;
    return result;

}