namespace Regulae.Evaluation
{
    internal enum Multiplicities : byte
    {
        ManyToMany = 0b11,
        ManyToOne = 0b10,
        OneToMany = 0b01,
        OneToOne = 0b00,
    }
}