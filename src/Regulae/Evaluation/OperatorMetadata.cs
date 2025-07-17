namespace Regulae.Evaluation
{
    using System.Collections.Frozen;
    using System.Collections.Generic;
    using System.Linq;
    using Regulae;

    internal sealed class OperatorMetadata
    {
        private bool? leftSupportForOneMultiplicity = null;

        public OperatorMetadata(Operators @operator, params Multiplicities[] supportedMultiplicities)
        {
            this.Operator = @operator;
            this.SupportedMultiplicities = supportedMultiplicities.ToFrozenSet();
        }

        public bool HasSupportForOneMultiplicityAtLeft
        {
            get
            {
                if (this.leftSupportForOneMultiplicity is null)
                {
                    this.leftSupportForOneMultiplicity = this.SupportedMultiplicities.Any(m => (((int)m >> 1) | 0x0) == 0x0);
                }

                return this.leftSupportForOneMultiplicity.GetValueOrDefault();
            }
        }

        public Operators Operator { get; }

        public ISet<Multiplicities> SupportedMultiplicities { get; }
    }
}