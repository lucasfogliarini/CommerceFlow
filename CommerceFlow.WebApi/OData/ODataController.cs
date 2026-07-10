using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace CommerceFlow.WebApi.OData;

public abstract class ODataController<TEntity> : ODataController where TEntity : Entity
{
}