using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TechDevs.Accounts.WebService
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