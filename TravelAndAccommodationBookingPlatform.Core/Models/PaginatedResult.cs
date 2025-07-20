namespace TravelAndAccommodationBookingPlatform.Core.Models;

public class PaginatedResult<T> where T : class
{
    public List<T> Items { get; set; } 
    public PaginationMetadata? PaginationMetadata { get; set; }

	public PaginatedResult(List<T> Items, PaginationMetadata pagination)
	{
		this.Items = Items;
		PaginationMetadata = new PaginationMetadata
		{
			PageNumber = pagination.PageNumber,
			PageSize=pagination.PageSize,
			TotalCount=pagination.TotalCount
		};
	}

}
