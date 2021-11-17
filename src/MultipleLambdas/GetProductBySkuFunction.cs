using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Ardalis.GuardClauses;
using Newtonsoft.Json;
using ProductCatalogue.Application.Queries;

namespace MultipleLambdas
{
    public class GetProductBySkuFunction : FunctionBase
    {
        private readonly Guid _tenantId;

        // example payload { "tenantId": "743872ea-7e68-421b-9f98-e09f35d76117", "sku": "HOU/IN/82" }
        public GetProductBySkuFunction()
        {
            // JUST FOR TESTING, forces the tenant ID to be a known one so the user doesn't have to remember it
            this._tenantId = Guid.Parse("743872ea-7e68-421b-9f98-e09f35d76117");
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            this.Logger.SetLoggerContext(context.Logger);
            this.Logger.LogInfo($"Fetching product by SKU");

            try
            {
                // just for testing, the tenant id would be a guid in a real app
                Guard.Against.Null(request, nameof(request));
                var query = JsonConvert.DeserializeObject<GetProductBySkuQuery>(request.Body);

                // fire command (tenantId should come from the data passed but this is for testing)
                query.TenantId = this._tenantId;

                this.Logger.LogInfo($"Fetching product by SKU, tenant: {this._tenantId}, sku: {query.Sku}");
                var queryResult = await this.Mediator.Send(new GetProductBySkuQuery(this._tenantId, query.Sku));

                // return result
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Body = JsonConvert.SerializeObject(queryResult)
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
