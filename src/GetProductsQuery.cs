using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediatr;

namespace Queries
{
    public class GetProductsQuery : IRequest<IEnumerable<Product>>
    {
        public GetProductsQuery(Guid tenantId)
        {

        }
    }
}
