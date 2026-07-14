namespace CommerceFlow.WebApi;

public interface IEndpoint
{
    IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app);
}