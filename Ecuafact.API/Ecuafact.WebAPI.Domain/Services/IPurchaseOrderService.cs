using Ecuafact.WebAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface IPurchaseOrderService
    {
        OperationResult<PurchaseOrder> Add(PurchaseOrder purchaseOrder);
        OperationResult<PurchaseOrder> Update(PurchaseOrder purchaseOrder);
        OperationResult<ElectronicSign> BuyElectronicSign(ElectronicSign eSign);
        OperationResult<ElectronicSign> GetElectronicSignByPurchase(long id);
        OperationResult<ElectronicSign> GetElectronicSignById(long id);
        OperationResult<PurchaseOrder> GetPurchaseOrderById(long id);
        OperationResult<List<PurchasePayment>> GetValidateOrderPayment(long purchaseOrderId);
        OperationResult<PurchasePayment> SavePurchasePayment(PurchasePayment purchasePayment);
        OperationResult<PurchasePayment> UpdatePurchasePayment(PurchasePayment purchasePayment);
        OperationResult<ElectronicSign> GetElectronicSignByRUC(string ruc, bool pending = false);
        OperationResult<ElectronicSign> UpdateElectronicSign(ElectronicSign esign);
        OperationResult<List<PurchasePayment>> GetPayments(long purchaseOrderId);
        OperationResult<PurchasePayment> GetPaymentById(long id, bool purchaseOrder = false);
        OperationResult<PurchaseLog> SavePurchaseLog(PurchaseLog log);
        OperationResult<ElectronicSign> GetHasElectronicSignByRUC(string ruc);
        OperationResult<ElectronicSign> GetElectronicSignByRUC(string ruc);
        OperationResult<BeneficiaryReferenceCode> GetBeneficiaryReferenceCode(string RUC);
        OperationResult<ReferenceCodes> GetReferenceCodes(string cod, string ruc = "");
        OperationResult<BeneficiaryReferenceCode>AddBeneficiaryReferenceCode(BeneficiaryReferenceCode model);
        OperationResult<BeneficiaryReferenceCode> UpdateBeneficiaryReferenceCode(BeneficiaryReferenceCode beniReferCode);
        OperationResult<List<ECommerce>> GeteCommerces();
        OperationResult<ReferenceCodes> GetReferenceCodesById(long id);
        OperationResult<ECommerce> GeteCommerceByCode(string code);
        OperationResult<List<ElectronicSign>> GetElectronicSignByApproved();
        long GetElectronicSignByEmail(string ruc, string email);
        OperationResult<ElectronicSign> GetElectronicSignByToken(string token);
    }

}
