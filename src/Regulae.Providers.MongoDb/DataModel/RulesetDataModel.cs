namespace Regulae.Providers.MongoDb.DataModel
{
    using System;
    using MongoDB.Bson.Serialization.Attributes;

    internal class RulesetDataModel
    {
        [BsonElement("creation", Order = 3)]
        public DateTime Creation { get; set; }

        [BsonElement("id", Order = 1)]
        public Guid Id { get; set; }

        [BsonElement("name", Order = 2)]
        public string Name { get; set; }

        public override bool Equals(object obj)
            => obj is RulesetDataModel model
                && this.Creation == model.Creation
            && this.Id.Equals(model.Id)
            && string.Equals(this.Name, model.Name, StringComparison.Ordinal);

        public override int GetHashCode() => HashCode.Combine(this.Creation, this.Id, this.Name);
    }
}