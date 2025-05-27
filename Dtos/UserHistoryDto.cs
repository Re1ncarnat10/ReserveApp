public class UserHistoryDto
{
    public int UserHistoryId { get; set; }
    public string UserId { get; set; }
    public int ResourceId { get; set; }
    public string ResourceName { get; set; }
    public string ResourceDescription { get; set; }
    public string ResourceType { get; set; }
    public string ResourceImage { get; set; }
    public DateTime ApprovedAt { get; set; }
    public DateTime RentalStartTime { get; set; }
    public DateTime RentalEndTime { get; set; }
}