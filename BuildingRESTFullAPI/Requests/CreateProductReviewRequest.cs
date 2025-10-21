namespace BuildingRESTFullAPI.Requests;

public class CreateProductReviewRequest
{
    public string? Reviewer { get; set; }
    public int Rating { get; set; }
}