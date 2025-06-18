namespace Basket.API.Basket.StoreBasket;

public record StoreBasketRequest(ShoppingCart Cart);
public record StoreBasketResponse(string UserName);

public class StoreBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket", async (StoreBasketRequest request, ISender sender, HttpContext context) =>
        {
            // Get username from JWT claims
            var username = context.User?.Claims?.FirstOrDefault(x => x.Type == "username")?.Value
                        ?? context.User?.Claims?.FirstOrDefault(x => x.Type == "preferred_username")?.Value
                        ?? context.User?.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return Results.Problem("User identity not found in token", statusCode: 400);
            }

            // Set the username from JWT claims (ignore what frontend sends)
            request.Cart.UserName = username;

            var command = request.Adapt<StoreBasketCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<StoreBasketResponse>();

            return Results.Created($"/basket", response);
        })
        .RequireAuthorization()
        .WithName("StoreUserBasket")
        .Produces<StoreBasketResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Store Current User's Basket")
        .WithDescription("Create or update shopping basket for the authenticated user using JWT claims");
    }
}
