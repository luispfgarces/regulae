namespace Regulae.Providers.MongoDb.DataModel
{
    using MongoDB.Bson.Serialization.Attributes;

    internal sealed class OperandDataModel
    {
        [BsonElement(Order = 2)]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Cardinalities Cardinality { get; set; }

        [BsonElement(Order = 1)]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public DataTypes DataType { get; set; }

        [BsonElement(Order = 3)]
        public object Value { get; set; }
    }
}