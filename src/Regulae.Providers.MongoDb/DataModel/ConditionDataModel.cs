namespace Regulae.Providers.MongoDb.DataModel
{
    using System;
    using MongoDB.Bson.Serialization.Attributes;

    internal class ConditionDataModel
    {
        [BsonElement("creation", Order = 3)]
        public DateTime Creation { get; set; }

        [BsonElement("dataType", Order = 4)]
        public string DataType { get; set; }

        [BsonElement("id", Order = 1)]
        public Guid Id { get; set; }

        [BsonElement("name", Order = 2)]
        public string Name { get; set; }

        public override bool Equals(object obj)
            => obj is ConditionDataModel model
                && this.Creation == model.Creation
                && string.Equals(this.DataType, model.DataType, StringComparison.Ordinal)
                && this.Id.Equals(model.Id)
                && string.Equals(this.Name, model.Name, StringComparison.Ordinal);

        public override int GetHashCode() => HashCode.Combine(this.Creation, this.DataType, this.Id, this.Name);
    }
}