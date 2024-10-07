using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.WebApi;
using System.Web.Http;
using System.Reflection;
using System.Data.Entity;
using Ecuafact.WebAPI.Dal.Core;
using Ecuafact.WebAPI.Dal.Repository;
using Ecuafact.WebAPI.Dal.Services;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Domain.Repository;

namespace Ecuafact.WebAPI
{
    public class AutofacConfig
    {
        public static void Initialize(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // EF DbContext
            builder.Register(c => new EcuafactExpressContext()).As<DbContext>().InstancePerRequest();            

            // Register repositories by using Autofac's OpenGenerics feature
            // More info: http://code.google.com/p/autofac/wiki/OpenGenerics
            builder.RegisterGeneric(typeof(EntityRepository<>)).As(typeof(IEntityRepository<>)).InstancePerRequest();

            builder.RegisterType<DocumentRepository>().As<IDocumentRepository>().InstancePerRequest();
            // Services
            builder.RegisterType<CatalogsService>().As<ICatalogsService>().InstancePerRequest();
            builder.RegisterType<ProductsService>().As<IProductsService>().InstancePerRequest();
            builder.RegisterType<ContributorsService>().As<IContributorsService>().InstancePerRequest();
            builder.RegisterType<IssuersService>().As<IIssuersService>().InstancePerRequest();
            builder.RegisterType<RequestSessionsService>().As<IRequestSessionsService>().InstancePerRequest();
            builder.RegisterType<DocumentsService>().As<IDocumentsService>().InstancePerRequest();
            builder.RegisterType<TaxesService>().As<ITaxesService>().InstancePerRequest();
            builder.RegisterType<StatisticsService>().As<IStatisticsService>().InstancePerRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<ReportService>().As<IReportService>().InstancePerRequest();
            builder.RegisterType<PurchaseOrderService>().As<IPurchaseOrderService>().InstancePerRequest();
            builder.RegisterType<EngineService>().As<IEngineService>().InstancePerRequest();
            builder.RegisterType<AppService>().As<IAppService>().InstancePerRequest();
            builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().InstancePerRequest();
            builder.RegisterType<AgreementService>().As<IAgreementService>().InstancePerRequest();
            builder.RegisterType<DocumentReceivedRepository>().As<IDocumentReceivedRepository>().InstancePerRequest();
            builder.RegisterType<AtsService>().As<IAtsService>().InstancePerRequest();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }


        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // EF DbContext
            //builder.RegisterType<EcuafactExpressContext>().As<DbContext>().InstancePerApiRequest();
            builder.Register(c => new EcuafactExpressContext()).As<DbContext>().InstancePerRequest();
             
            // Register repositories by using Autofac's OpenGenerics feature
            // More info: http://code.google.com/p/autofac/wiki/OpenGenerics
            //builder.RegisterGeneric(typeof(EntityRepository<>)).As(typeof(IEntityRepository<>)).InstancePerApiRequest();
            builder.RegisterGeneric(typeof(EntityRepository<>)).As(typeof(IEntityRepository<>)).InstancePerRequest();

            // Services
            //builder.RegisterType<CryptoService>()
            //    .As<ICryptoService>()
            //    .InstancePerApiRequest();

            //builder.RegisterType<MembershipService>()
            //    .As<IMembershipService>()
            //    .InstancePerApiRequest();

            //builder.RegisterType<ShipmentService>()
            //    .As<IShipmentService>()
            //    .InstancePerApiRequest();

            return builder.Build();
        }
    }
}