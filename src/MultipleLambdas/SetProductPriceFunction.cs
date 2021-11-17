using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Ardalis.GuardClauses;
using Newtonsoft.Json;
using ProductCatalogue.Application.Commands;

namespace MultipleLambdas
{
    // example payloads { "tenantId": "743872ea-7e68-421b-9f98-e09f35d76117", "sku": "HOU/IN/82", "newPrice": 3.99 }
    // { \"tenantId\": \"743872ea-7e68-421b-9f98-e09f35d76117\", \"sku\": \"HOU/IN/82\", \"newPrice\": 3.99 }

    public class SetProductPriceFunction : FunctionBase
    {
        private readonly Guid _tenantId;

        public SetProductPriceFunction()
        {
            // JUST FOR TESTING, forces the tenant ID to be a known one so the user doesn't have to remember it
            this._tenantId = Guid.Parse("743872ea-7e68-421b-9f98-e09f35d76117");
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            this.Logger.SetLoggerContext(context.Logger);
            this.Logger.LogInfo($"Fetching product by SKU");

            try
            {
                Guard.Against.Null(request, nameof(request));
                var command = JsonConvert.DeserializeObject<ChangeProductPriceCommand>(request.Body);

                // fire command (tenantId should come from the data passed but this is for testing)
                command.TenantId = this._tenantId;
                var cmdResult = await this.Mediator.Send(command);

                // return result
                return new APIGatewayProxyResponse
                {
                    StatusCode = cmdResult ? (int) HttpStatusCode.OK : (int) HttpStatusCode.BadRequest
                };
            }
            catch (NotFoundException nex)
            {
                this.Logger.LogError($"exception; {nex.Message}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.NotFound
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
