namespace Basket.API.Basket.GetBasket;

//public record GetBasketRequest(string UserName);
public record GetBasketResponse(ShoppingCart Cart);

public class GetBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/basket/{userName}", async (string userName, ISender sender, HttpContext context) =>
        {
            // Validate user can only access their own basket
            var currentUser = context.User?.Identity?.Name;
            if (string.IsNullOrEmpty(currentUser) || currentUser != userName)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new GetBasketQuery(userName));
            var response = result.Adapt<GetBasketResponse>();
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithName("GetUserBasket")
        .Produces<GetBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .WithSummary("Get User Basket")
        .WithDescription("Get shopping basket for the authenticated user");
    }
}
