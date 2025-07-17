namespace Regulae
{
    using System;

    public class Condition
    {
        internal Condition(string name, DateTime creation, DataTypes dataType)
        {
            this.Name = name;
            this.Creation = creation;
            this.DataType = dataType;
        }

        public DateTime Creation { get; }

        public DataTypes DataType { get; }

        public string Name { get; }
    }
}