namespace Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketRequest(BasketCheckoutDto BasketCheckoutDto);
public record CheckoutBasketResponse(bool IsSuccess);

public class CheckoutBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket/checkout", async (CheckoutBasketRequest request, ISender sender, HttpContext context) =>
        {
            // Get user information from JWT claims
            var username = context.User?.Claims?.FirstOrDefault(x => x.Type == "username")?.Value
                        ?? context.User?.Claims?.FirstOrDefault(x => x.Type == "preferred_username")?.Value
                        ?? context.User?.Identity?.Name;

            var customerId = context.User?.Claims?.FirstOrDefault(x => x.Type == "sub")?.Value
                          ?? context.User?.Claims?.FirstOrDefault(x => x.Type == "user_id")?.Value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(customerId))
            {
                return Results.Problem("User identity not found in token", statusCode: 400);
            }

            // Set user information from JWT claims (ignore what frontend sends)
            request.BasketCheckoutDto.UserName = username;
            if (Guid.TryParse(customerId, out var customerGuid))
            {
                request.BasketCheckoutDto.CustomerId = customerGuid;
            }

            var command = request.Adapt<CheckoutBasketCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CheckoutBasketResponse>();

            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithName("CheckoutUserBasket")
        .Produces<CheckoutBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Checkout Current User's Basket")
        .WithDescription("Checkout shopping basket for the authenticated user using JWT claims");
    }
}
