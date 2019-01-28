using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TechDevs.Gibson.Api
{
    public class TechDevsClientKeyHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)

        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "TechDevs-ClientKey",
                In = "header",
                Type = "string",
                Required = false
            });
        }
    }
}