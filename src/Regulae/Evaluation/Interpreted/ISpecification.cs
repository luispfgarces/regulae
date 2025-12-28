namespace Regulae.Evaluation.Interpreted
{
    internal interface ISpecification<T>
    {
        ISpecification<T> And(ISpecification<T> otherSpecification);

        bool IsSatisfiedBy(T input);

        ISpecification<T> Or(ISpecification<T> otherSpecification);

        ISpecification<T> Xor(ISpecification<T> otherSpecification);
    }
}