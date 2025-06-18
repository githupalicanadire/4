namespace Basket.API.Basket.DeleteBasket;

//public record DeleteBasketRequest(string UserName);
public record DeleteBasketResponse(bool IsSuccess);

public class DeleteBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/basket", async (ISender sender, HttpContext context) =>
        {
            // Get username from JWT claims
            var username = context.User?.Claims?.FirstOrDefault(x => x.Type == "username")?.Value
                        ?? context.User?.Claims?.FirstOrDefault(x => x.Type == "preferred_username")?.Value
                        ?? context.User?.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return Results.Problem("User identity not found in token", statusCode: 400);
            }

            var result = await sender.Send(new DeleteBasketCommand(username));
            var response = result.Adapt<DeleteBasketResponse>();

            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithName("DeleteUserBasket")
        .Produces<DeleteBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Delete Current User's Basket")
        .WithDescription("Delete shopping basket for the authenticated user using JWT claims");
    }
}
