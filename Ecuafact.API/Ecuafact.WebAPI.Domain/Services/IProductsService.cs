using System.Collections.Generic;
using System.Linq;
using Ecuafact.WebAPI.Domain.Entities;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface IProductsService
    {
        Product GetIssuerProduct(long productId, long issuerId);

        IQueryable<Product> GetIssuerProducts(long issuerId, bool export = false);
        
        OperationResult<Product> AddProduct(Product newProduct);

        OperationResult<Product> UpdateProduct(Product productToUpdate);

        IEnumerable<Product> SearchProducts(string searchTerm, long issuerId);

        Product GetProductByCode(string code, long issuerId);

        OperationResult<List<Product>> ImportProduct(List<Product> newProduct);
    }
}
