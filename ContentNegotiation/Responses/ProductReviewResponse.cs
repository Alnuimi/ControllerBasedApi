using System;
using ContentNegotiation.Models;

namespace ContentNegotiation.Responses;

public class ProductReviewResponse
{
     public Guid ReviewId { get; set; }
    public Guid ProductId { get; set; }
    public string? Reviewer { get; set; }
    public int Rating { get; set; }

    private ProductReviewResponse()
    {
    }

    public static ProductReviewResponse FromModel(ProductReview review)
    {
        if (review is null)
            throw new ArgumentNullException(nameof(review), "Cannot create a response from a null review");

        return new ProductReviewResponse
        {
            ReviewId = review.Id,
            ProductId = review.ProductId,
            Reviewer = review.Reviewer,
            Rating = review.Rating
        };
    }

    public static IEnumerable<ProductReviewResponse> FromModels(IEnumerable<ProductReview> reviews)
    {
        if (reviews is null)
            throw new ArgumentNullException(nameof(reviews), "Cannot create a response from a null review list");

        return reviews.Select(FromModel);
    }
}
