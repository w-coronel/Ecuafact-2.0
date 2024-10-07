using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using Ecuafact.WebAPI.Dal.Repository.Extensions;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;

namespace Ecuafact.WebAPI.Dal.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IEntityRepository<Product> _productRepository;

        public ProductsService(IEntityRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public Product GetIssuerProduct(long productId, long issuerId)
        {
            return _productRepository.One(productId, issuerId);
        }

        public IQueryable<Product> GetIssuerProducts(long issuerId, bool export = false)
        {
            if (export)
            {
                return _productRepository.GetProductsByIssuer(issuerId).Include(m => m.ProductType).Include(i=> i.IvaRate).Include(e => e.IceRate);
            }
            return _productRepository.GetProductsByIssuer(issuerId);
        }

        public OperationResult<Product> AddProduct(Product newProduct)
        {
            var product = _productRepository.FindBy(pr => pr.MainCode.Equals(newProduct.MainCode) && pr.IssuerId == newProduct.IssuerId && pr.IsEnabled).FirstOrDefault();

            if (product != null)
                return new OperationResult<Product>(false, HttpStatusCode.Conflict) { DevMessage = "El código de producto ya existe", UserMessage = "El código del producto que desea crear, ya existe." };

            try
            {
                newProduct.CreatedOn = DateTime.Now;
                _productRepository.Add(newProduct);
                _productRepository.Save();
                return new OperationResult<Product>(true, HttpStatusCode.OK)
                {
                    Entity = newProduct
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<Product>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el producto" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Product>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el producto"};
            }
            catch (Exception e)
            {
                return new OperationResult<Product>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el producto" };
            }
        }

        public OperationResult<Product> UpdateProduct(Product productToUpdate)
        {
            try
            {
                //_productRepository.Edit(productToUpdate);
                _productRepository.Save();
                return new OperationResult<Product>(true, HttpStatusCode.OK)
                {
                    Entity = productToUpdate
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<Product>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el producto" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Product>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el producto" };
            }
            catch (Exception e)
            {
                return new OperationResult<Product>(false, HttpStatusCode.InternalServerError) { DevMessage = e.Message, UserMessage = "Ocurrio un error guardar el producto" };
            }
        }

        public IEnumerable<Product> SearchProducts(string searchTerm, long issuerId)
        {
            return _productRepository.All.Where(x =>
            x.IssuerId == issuerId &&
            (
                x.MainCode.Contains(searchTerm)
                || ((x.AuxCode ?? "").Contains(searchTerm))
                || (x.MainCode + "." + (x.AuxCode ?? "")).Contains(searchTerm)
                || x.Name.Contains(searchTerm)
                )
            )?.ToList();
            
            //return _productRepository.Search(searchTerm, issuerId);
        }

        public Product GetProductByCode(string code, long issuerId)
        {
            return _productRepository.All.FirstOrDefault(x => x.IssuerId == issuerId && (x.MainCode == code || (x.MainCode + "." + (x.AuxCode ?? "")) == code));
        }


        public OperationResult<Product> AddImportProduct(Product newProduct)
        {         

            try
            {
                newProduct.CreatedOn = DateTime.Now;
                _productRepository.Add(newProduct);
                _productRepository.Save();
                return new OperationResult<Product>(true, HttpStatusCode.OK)
                {
                    Entity = newProduct
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<Product>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el producto" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Product>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el producto" };
            }
            catch (Exception e)
            {
                return new OperationResult<Product>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el producto" };
            }
        }

        public OperationResult<List<Product>> ImportProduct(List<Product> newProduct)
        {            

            try
            {               
                if (newProduct.Count() > 0)
                {                   
                    _productRepository.AddRange(newProduct);
                    _productRepository.Save();
                    return new OperationResult<List<Product>>(true, HttpStatusCode.OK)
                    {
                        Entity = newProduct,                        
                        DevMessage = $"Se registraron los productos exitosamente.",
                        UserMessage = $"Se registraron los productos exitosamente."
                    };                   
                }

                return new OperationResult<List<Product>>(false, HttpStatusCode.BadRequest) {UserMessage = "Ocurrio un error guardar el producto" };

            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<List<Product>>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el producto" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<List<Product>>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el producto" };
            }
            catch (Exception e)
            {
                return new OperationResult<List<Product>>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el producto" };
            }
        }
    }
}
