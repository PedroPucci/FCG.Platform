using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FCG.Platform.Extensions.SwaggerDocumentation
{
    public class CustomOperationDescriptions : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context?.ApiDescription?.HttpMethod is null || context.ApiDescription.RelativePath is null)
                return;

            var path = context.ApiDescription.RelativePath.ToLowerInvariant();

            var routeHandlers = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
            {
                { "user",               () => HandleUserOperations(operation, context) }
            };

            foreach (var kv in routeHandlers
                     .OrderByDescending(k => k.Key.Length))
            {
                if (path.Contains(kv.Key))
                {
                    kv.Value.Invoke();
                    return;
                }
            }
        }

        private void HandleUserOperations(OpenApiOperation operation, OperationFilterContext context)
        {
            var method = context.ApiDescription.HttpMethod;
            var path = context.ApiDescription.RelativePath?.ToLower() ?? string.Empty;

            if (method == "POST")
            {
                operation.Summary = "Create a new User.";
                operation.Description = "This endpoint allows you to create a new User by providing the necessary details.";
                AddResponses(operation, "200", "The User was successfully created.");
            }
            else if (method == "PUT")
            {
                operation.Summary = "Update an existing User.";
                operation.Description = "This endpoint allows you to update an existing User by providing the necessary details.";
                AddResponses(operation, "200", "The User was successfully updated.");
            }
            else if (method == "DELETE")
            {
                operation.Summary = "Delete an existing User.";
                operation.Description = "This endpoint allows you to delete an existing User by providing the ID.";
                AddResponses(operation, "200", "The User was successfully deleted.");
                AddResponses(operation, "404", "User not found. Please verify the ID.");
            }
            else if (method == "GET")
            {
                operation.Summary = "Retrieve all Users.";
                operation.Description = "This endpoint allows you to retrieve details of all existing Users.";
                AddResponses(operation, "200", "All Users details were successfully retrieved.");
            }
        }

        private void AddResponses(OpenApiOperation operation, string statusCode, string description)
        {
            if (!operation.Responses.ContainsKey(statusCode))
            {
                operation.Responses.Add(statusCode, new OpenApiResponse { Description = description });
            }
        }
    }
}