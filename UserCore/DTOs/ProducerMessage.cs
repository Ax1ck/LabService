namespace UserCore.DTOs;

public class ProducerMessage
{
    public bool Confirmed { get; set; } = false;

    public string ObjectId { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public DateTime ConfirmationTime { get; set; }
}