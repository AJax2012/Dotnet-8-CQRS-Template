using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using ExampleProject.Api.Endpoints.Common.Contracts;
using ExampleProject.Api.Endpoints.Common.Extensions;
using ExampleProject.Application.Profiles.Queries.GetById;
using ExampleProject.Contracts.Profiles.Responses;

namespace ExampleProject.Api.Endpoints.Profiles;

/// <summary>
/// Endpoint handler for getting a Profile by id.
/// </summary>
public class GetProfileById : IEndpoint
{
    /// <inheritdoc />
    public static RouteHandlerBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("{id:guid}", HandleAsync)
        .Produces<GetProfileByIdResponse>(StatusCodes.Status200OK, "application/json")
        .ProducesProblem(StatusCodes.Status404NotFound, "application/json")
        .WithName("GetProfileById");
            
    /// <summary>
    /// API request for getting a Profile by id.
    /// </summary>
    /// <param name="id">Unique id for Profile.</param>
    /// <param name="user"><see cref="ClaimsPrincipal"/>. Logged in user.</param>
    /// <param name="mediator"><see cref="ISender"/>.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="IResult"/>.</returns>
    public async static Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        ClaimsPrincipal user,
        [FromServices] ISender mediator,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) && userId != Guid.Empty)
        {
            return Results.Unauthorized();
        }
        
        if (id == Guid.Empty)
        {
            var error = Error.Validation(code: "GetProfileById.IdRequired", description: "Id is required.");
            return new List<Error> { error }.ToProblemDetailsResult();
        }

        var query = new GetProfileByIdQuery(id, userId);
        var result = await mediator.Send(query, cancellationToken);
        
        return result.Match(
            profile => Results.Ok(profile.ToResponse()),
            _ => Results.NotFound(new { id = id }));
    }
}