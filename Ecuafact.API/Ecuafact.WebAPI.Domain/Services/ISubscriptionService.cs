using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Ecuafact.WebAPI.Domain.Entities;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface ISubscriptionService
    {
        Subscription GetSubscription(long id);
        Subscription GetSubscription(string issuerRuc);
        IQueryable<Subscription> GetISubscriptions(object issuerRuc);
        IQueryable<Subscription> GetISubscriptions();
        bool Exists(object id);
        OperationResult<Subscription> AddSubscription(Subscription model);
        OperationResult<Subscription> UpdateSubscription(Subscription model);
        OperationResult<Subscription> ActiveSubscription(string ruc, long issuerId);
        OperationResult<PurchaseSubscription> GetIPurchaseSubscription(long id);
        OperationResult<PurchaseSubscription> BuySubscription(PurchaseSubscription model);
        OperationResult<PurchaseSubscription> AddPurchaseSubscription(PurchaseSubscription model);
        OperationResult<PurchaseSubscription> UpdatePurchaseSubscription(PurchaseSubscription model);
        OperationResult<PurchaseSubscription> PurchaseSubscriptionInvoiceProcessed(long purchaseSubscriptionId);
        OperationResult<PurchaseSubscription> GetSubscriptionByPurchase(long purchaseOrderId);
        OperationResult<PurchaseSubscription> GetPurchaseBySubscriptionbyRuc(string ruc);
        OperationResult<PurchaseOrder> GetPurchaseOrderById(long id);
        OperationResult<PurchaseSubscription> GetPurchaseSubscriptionActive(string ruc);
        OperationResult<PurchaseSubscription> GetPurchaseBySubscriptionValidatePayment(string ruc);
        OperationResult<PurchaseSubscription> GetIPurchaseSubscriptionBySubscriptionId(long id);
        SubscriptionLog GetSubscriptionLog(long id);
        LicenceType GetLicenceTypeById(long id);
    }
}
