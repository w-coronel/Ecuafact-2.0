using Ecuafact.WebAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface IAgreementService
    {
        #region Convenios
        OperationResult<List<Agreement>> GetAgreement();
        OperationResult<Agreement> GetAgreementById(long Id);
        OperationResult<Agreement> GetAgreementByCode(string code);
        OperationResult<ReferenceCodes> GetReferenceCodeByAgreementCode(string code);

        #endregion

        #region Benificiarios

        OperationResult<Beneficiary> AddBeneficiary(Beneficiary model);
        OperationResult<Beneficiary> UpdateBeneficiary(Beneficiary model);
        OperationResult<Beneficiary> GetBeneficiary(long id);
        OperationResult<List<Beneficiary>> GetBeneficiaryByAgreementId(long id);
       
        #endregion


    }
}
