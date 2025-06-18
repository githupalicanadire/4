namespace Basket.API.Basket.StoreBasket;

public record StoreBasketRequest(ShoppingCart Cart);
public record StoreBasketResponse(string UserName);

public class StoreBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket", async (StoreBasketRequest request, ISender sender, HttpContext context) =>
        {
            // Validate user can only modify their own basket
            var currentUser = context.User?.Identity?.Name;
            if (string.IsNullOrEmpty(currentUser) || currentUser != request.Cart.UserName)
            {
                return Results.Forbid();
            }

            var command = request.Adapt<StoreBasketCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<StoreBasketResponse>();

            return Results.Created($"/basket/{response.UserName}", response);
        })
        .RequireAuthorization()
        .WithName("StoreUserBasket")
        .Produces<StoreBasketResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .WithSummary("Store User Basket")
        .WithDescription("Create or update shopping basket for the authenticated user");
    }
}
