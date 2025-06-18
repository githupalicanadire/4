namespace Basket.API.Basket.DeleteBasket;

//public record DeleteBasketRequest(string UserName);
public record DeleteBasketResponse(bool IsSuccess);

public class DeleteBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/basket/{userName}", async (string userName, ISender sender, HttpContext context) =>
        {
            // Validate user can only delete their own basket
            var currentUser = context.User?.Identity?.Name;
            if (string.IsNullOrEmpty(currentUser) || currentUser != userName)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new DeleteBasketCommand(userName));
            var response = result.Adapt<DeleteBasketResponse>();

            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithName("DeleteUserBasket")
        .Produces<DeleteBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Delete User Basket")
        .WithDescription("Delete shopping basket for the authenticated user");
    }
}
