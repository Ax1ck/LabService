using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NutritionCore.Entities;

public class Meal
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } // Óáåäèòåñü, ÷òî ýòî ñâîéñòâî ìîæåò èçìåíÿòüñÿ
    [BsonElement("name"), BsonRepresentation(BsonType.String)]
    public string Name { get; set; }
    [BsonElement("ingredients"), BsonRepresentation(BsonType.String)]
    public string Ingredients { get; set; }
    [BsonElement("calories"), BsonRepresentation(BsonType.String)]
    public string Calories { get; set; }
    [BsonElement("user_id"), BsonRepresentation(BsonType.String)]
    public Guid? UserId { get; set; }
    [BsonElement("confirmation_time"), BsonRepresentation(BsonType.DateTime)]
    public DateTime? ConfirmationTime { get; set; }
}