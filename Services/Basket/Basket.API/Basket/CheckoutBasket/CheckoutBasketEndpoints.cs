namespace Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketRequest(BasketCheckoutDto BasketCheckoutDto);
public record CheckoutBasketResponse(bool IsSuccess);

public class CheckoutBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket/checkout", async (CheckoutBasketRequest request, ISender sender, HttpContext context) =>
        {
            // Validate user can only checkout their own basket
            var currentUser = context.User?.Identity?.Name;
            if (string.IsNullOrEmpty(currentUser) || currentUser != request.BasketCheckoutDto.UserName)
            {
                return Results.Forbid();
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
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .WithSummary("Checkout User Basket")
        .WithDescription("Checkout shopping basket for the authenticated user");
    }
}
