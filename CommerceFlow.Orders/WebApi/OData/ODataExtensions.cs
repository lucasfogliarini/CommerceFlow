using CommerceFlow;
using CommerceFlow.WebApi;
using CommerceFlow.WebApi.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Reflection;

namespace Microsoft.AspNetCore.OData
{
    public static class ODataExtensions
    {
        public static void AddRouteComponentsUsingODataControllers(this ODataOptions oDataOptions, string routePrefix = "odata")
        {
            oDataOptions.AddRouteComponents(routePrefix, GetEdmModelODataControllers());
        }

        static IEdmModel GetEdmModelODataControllers()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EnableLowerCamelCase();
            var odataControllers = Assembly.GetExecutingAssembly().GetTypes().Where(e => e.BaseType?.Name == typeof(ODataController<>).Name);
            foreach (var odataController in odataControllers)
            {
                var responseType = odataController.BaseType?.GenericTypeArguments.FirstOrDefault();
                var entityType = builder.AddEntityType(responseType);
                var entitySetName = odataController.Name.Replace("Controller", "");
                builder.AddEntitySet(entitySetName, entityType);
            }
            var edmModel = builder.GetEdmModel();
            return edmModel;
        }
    }
}