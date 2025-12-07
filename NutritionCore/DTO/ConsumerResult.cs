namespace NutritionCore.DTO;

public class ConsumerResult
{
    public bool Confirmed { get; set; } = false;

    public string ObjectId { get; set; }

    public Guid UserId { get; set; }

    public DateTime ConfirmationTime { get; set; }
}