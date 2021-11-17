using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using ProductCatalogue.Application.Queries;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace MultipleLambdas
{
    public class GetProductsFunction : FunctionBase
    {
        private readonly Guid _tenantId;

        public GetProductsFunction()
        {
            // JUST FOR TESTING, forces the tenant ID to be a known one so the user doesn't have to remember it
            this._tenantId = Guid.Parse("743872ea-7e68-421b-9f98-e09f35d76117");
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            // performs a query for all products for a tenant (tenant id would be from a parameter in real life).
            this.Logger.SetLoggerContext(context.Logger);
            this.Logger.LogInfo($"Fetching all products for tenant: {this._tenantId}");

            try
            {
                var query = new GetProductsQuery(this._tenantId);
                var queryResponse = await this.Mediator.Send(query);

                this.Logger.LogInfo($"Returning {queryResponse.Count()} records");

                // return result
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Body = JsonConvert.SerializeObject(queryResponse)
                };
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"exception; {ex.Message}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
