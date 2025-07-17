namespace Regulae.Providers.MongoDb.DataModel
{
    using MongoDB.Bson.Serialization.Attributes;
    using Regulae;

    [BsonDiscriminator("value")]
    internal sealed class ValueConditionNodeDataModel : ConditionNodeDataModel
    {
        [BsonElement(Order = 1)]
        public string Condition { get; set; }

        [BsonElement(Order = 2)]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Operators Operator { get; set; }

        [BsonElement(Order = 3)]
        public OperandDataModel RightOperand { get; set; }
    }
}