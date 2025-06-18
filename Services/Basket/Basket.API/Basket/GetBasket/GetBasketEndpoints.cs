namespace Basket.API.Basket.GetBasket;

//public record GetBasketRequest(string UserName);
public record GetBasketResponse(ShoppingCart Cart);

public class GetBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/basket", async (ISender sender, HttpContext context) =>
        {
            // Get username from JWT claims
            var username = context.User?.Claims?.FirstOrDefault(x => x.Type == "username")?.Value
                        ?? context.User?.Claims?.FirstOrDefault(x => x.Type == "preferred_username")?.Value
                        ?? context.User?.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return Results.Problem("User identity not found in token", statusCode: 400);
            }

            var result = await sender.Send(new GetBasketQuery(username));
            var response = result.Adapt<GetBasketResponse>();
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithName("GetUserBasket")
        .Produces<GetBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Get Current User's Basket")
        .WithDescription("Get shopping basket for the authenticated user using JWT claims");
    }
}
