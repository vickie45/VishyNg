using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Script.Serialization;
using ECS.Model.Core;
using ECS.Model.CustomEntity;
using ECS.Model.CustomEntity.ProviderPortal.AuthorizationLetter;
using ECS.Model.Models.AuthorizationLetter;
using ECS.Model.Models.Master;
using ECS.Model.Models.ProviderPortalAL;
using ECS.Service.Contract.Contracts;
using ECS.Service.Contract.Request.AuthorizationLetter;
using ECS.Service.Contract.Request.ClaimsCommon;
using ECS.Service.Contract.Request.Master;
using ECS.Service.Contract.Request.ProviderPortalAL;
using ECS.Service.Contract.Response.AuthorizationLetter;
using ECS.Service.Contract.Response.ClaimsCommon;
using ECS.Service.Contract.Response.Master;
using ECS.Service.Contract.Response.ProviderPortalAL;
using Newtonsoft.Json;
using RGICL.ProviderPortal.Common.Constants;
using RGICL.ProviderPortal.Common.Util;
using RGICL.ProviderPortal.Core.Service;
using RGICL.ProviderPortal.Domain.Models.AuthorizationLetter;
using RGICL.ProviderPortal.Domain.Models.Common;
using RGICL.ProviderPortal.Domain.Models.Master;
using WebAPI.Controllers;
using RGICL.ProviderPortal.Common.Enums;
using System.Globalization;
using ECS.Service.Contract.Response.ProviderPortalCommon;
using ECS.Service.Contract.Request.ProviderPortalCommon;
using ECS.Model.Models.ClaimCommon;
using ECS.Model.CustomEntity.ProviderPortal.Common;

namespace RGICL.ProviderPortal.WebApi2.Controllers.AuthorizationLetter
{
    public class ALDetailsApiController : BaseApiController
    {

        //// GET api/<controller>/5
        public ALCaseDetails GetCaseDetails(int alDetailId)
        {
            ALCaseDetailResponse alCaseDetailsResponse = new ALCaseDetailResponse();
            GetALCaseDetailRequest aLCaseDetailsRequest = new GetALCaseDetailRequest();
            aLCaseDetailsRequest.alDetailId = alDetailId;

            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                alCaseDetailsResponse = proxy.Channel.GetALCaseDetail(aLCaseDetailsRequest);
            }

            ALCaseDetail alCaseDetail = alCaseDetailsResponse.ALCaseDetail;
            List<RGISLICDMasterViewModel> alICDDiagnosisCodes = new List<RGISLICDMasterViewModel>();
            foreach (var item in alCaseDetail.ALICDDiagnosisCodes)
            {
                alICDDiagnosisCodes.Add(new RGISLICDMasterViewModel { Code = item.ICDMaster.ICDCode, Description = item.ICDMaster.Description, ICDMasterId = item.ICDCodeId, IsDeleted = item.IsDeleted, ALICDDiagnosisCodeId = item.ALICDDiagnosisCodeId });
            }

            RGICLDiseaseProcedureMasterViewModel cptMedicalList = new RGICLDiseaseProcedureMasterViewModel();
            cptMedicalList.Code = alCaseDetailsResponse.CPTMedicalList.RGICLCode;
            cptMedicalList.Description = alCaseDetailsResponse.CPTMedicalList.DiagnosisName;
            cptMedicalList.RGICLDiseaseProcedureID = alCaseDetailsResponse.CPTMedicalList.RGICLDiseaseProcedureID;
            //---rsannake---To remove default CPT code for QR
            if (alCaseDetailsResponse != null && alCaseDetailsResponse.ALCaseDetail!=null && alCaseDetailsResponse.ALCaseDetail.ALDetail != null )
            {
                if (alCaseDetailsResponse.ALCaseDetail.ALDetail.InwardClassificationId == 318 || alCaseDetailsResponse.ALCaseDetail.ALDetail.InwardClassificationId == 319)
                {
                    cptMedicalList = null;
                }
            }
            //---
            List<RGICLDiseaseProcedureMasterViewModel> alDiseaseCodes = new List<RGICLDiseaseProcedureMasterViewModel>();
            foreach (var item in alCaseDetail.ALDiseaseProcedureDetails)
            {
                alDiseaseCodes.Add(new RGICLDiseaseProcedureMasterViewModel { Code = item.RGICLDiseaseProcedureMaster.RGICLCode, Description = item.RGICLDiseaseProcedureMaster.DiagnosisName, RGICLDiseaseProcedureID = item.DiseaseProcedureCodeId, IsDeleted = item.IsDeleted, ALDiseaseProcedureDetailId = item.ALDiseaseProcedureDetailId, IsPrimary = item.IsPrimary, Proportion = item.Proportion, NetworkProportion = item.NetworkProportion, Treatment = item.TreatmentType });
            }
            List<InvestigationType> alInvestigationType = new List<InvestigationType>();
            foreach (var item in alCaseDetail.ALDiagnosisInvestigations)
            {
                alInvestigationType.Add(new InvestigationType { Code = item.InvestigationTypeMaster.Description, Description = item.InvestigationFindings, MasterId = item.InvestigationTypeId, IsDeleted = item.IsDeleted, ALDiagnosisInvestigationId = item.ALDiagnosisInvestigationId });
            }
            List<PastMedicalHistory> alPastMedicalHistoryDetails = new List<PastMedicalHistory>();
            foreach (var item in alCaseDetail.ALMedicalHistoryDetails)
            {
                alPastMedicalHistoryDetails.Add(new PastMedicalHistory
                {
                    ALMedicalHistoryDetailId = item.ALMedicalHistoryDetailId,
                    PastDiseaseId = item.PastDiseaseId,
                    Details = item.Details,
                    DurationFrequencyId = item.DurationFrequencyId,
                    DurationDays = Convert.ToInt32(item.DurationDays.GetValueOrDefault()),
                    IsDeleted = item.IsDeleted
                });
            }
            List<RGICL.ProviderPortal.Domain.Models.AuthorizationLetter.ALMillimanConditions> alMillimanConditions = new List<RGICL.ProviderPortal.Domain.Models.AuthorizationLetter.ALMillimanConditions>();
            foreach (var item in alCaseDetail.ALMillimanConditions)
            {
                alMillimanConditions.Add(new RGICL.ProviderPortal.Domain.Models.AuthorizationLetter.ALMillimanConditions
                    {
                        ALMillimanConditionId = item.ALMillimanConditionId,
                        ConditionId = item.ConditionId,
                        MaxLOS = item.MaxLOS,
                        MinLOS = item.MinLOS,
                        SeverityId = item.SeverityId == null ? 0 : item.SeverityId,
                        SignAndSymptomsId = item.SignAndSymptomsId == null ? 0 : item.SignAndSymptomsId,
                        IsEnabled = item.IsEnabled,
                        IsPrimary = item.IsPrimary
                    });
            }
            string inputDOA = string.Empty;
            string dateofAdmission = string.Empty;
            if (alCaseDetail.DOA != null)
            {
                inputDOA = Convert.ToString(alCaseDetail.DOA);
                DateTime dtDateofAdmission = DateTime.Parse(inputDOA, new CultureInfo("en-US"));
                dateofAdmission = dtDateofAdmission.ToString("dd/MMM/yyyy");
            }

            ALCaseDetails alCaseDetailsViewModel = new ALCaseDetails()
            {
                ALCaseDetailId = alCaseDetail.ALCaseDetailId,
                ALDetailId = alCaseDetail.ALDetailId,
                Diagnosis = alCaseDetail.Diagnosis,
                DurationofAilment = alCaseDetail.DurationofAilment,
                FrequencyId = alCaseDetail.FrequencyId,
                PastIllnessPresentComplaint = alCaseDetail.PastIllnessPresentComplaint,
                RelevantClinicalFindings = alCaseDetail.RelevantClinicalFindings,
                TreatmentPlanId = alCaseDetail.TreatmentPlanId,
                TreatmentDetails = alCaseDetail.TreatmentDetails,
                FirstOnsetConsultationDate = alCaseDetail.FirstOnsetConsultationDate,
                IsMaternity = alCaseDetail.IsMaternity == null ? false : alCaseDetail.IsMaternity.Value,
                ObstetricGVal = alCaseDetail.ObstetricGVal,
                ObstetricPVal = alCaseDetail.ObstetricPVal,
                ObstetricLVal = alCaseDetail.ObstetricLVal,
                ObstetricAVal = alCaseDetail.ObstetricAVal,
                LMPDate = alCaseDetail.LMPDate,
                EDD = alCaseDetail.EDD,
                IsInjuryRTASelfIinjury = alCaseDetail.IsInjuryRTASelfIinjury == null ? false : alCaseDetail.IsInjuryRTASelfIinjury.Value,
                IsInjury = alCaseDetail.IsInjuryRTASelfIinjury == null ? false : alCaseDetail.IsInjuryRTASelfIinjury.Value,
                IsInfluenceOfAlcoholDrugAbuse = alCaseDetail.IsInfluenceOfAlcoholDrugAbuse,
                IsTestConducted = alCaseDetail.IsTestConducted,
                MLCFIRPINumber = alCaseDetail.MLCFIRPINumber,
                Place = alCaseDetail.Place,
                IsPastMedicalHistoryAvailable = alCaseDetail.IsPastMedicalHistoryAvailable,
                ALICDDiagnosisCodes = alICDDiagnosisCodes,
                ALDiseaseCodes = alDiseaseCodes,
                ALInvestigationType = alInvestigationType,
                ALMedicalHistoryDetails = alPastMedicalHistoryDetails,
                PatientIPDNo = alCaseDetail.PatientIPDNo,
                PatientRegistrationNo = alCaseDetail.PatientRegistrationNo,
                ALMillimanConditions = alMillimanConditions,
                RGICLSurgeryGradeId = alCaseDetail.RGICLSurgeryGradeId,
                DateofAdmission = dateofAdmission,
                CPTMedicalList = cptMedicalList
            };

            List<PastMedicalRecord> PastMedicalData = new List<PastMedicalRecord>();
            foreach (var PMRecords in alCaseDetail.ALMedicalHistoryDetails)
            {
                if (PMRecords.IsDeleted == false)
                {
                    string FreqDesc = "";
                    if (alCaseDetailsViewModel.Frequency != null)
                    {
                        var FrequencyDesc = (from FreqSrc in alCaseDetailsViewModel.Frequency
                                             where FreqSrc.CodeMasterId == int.Parse(PMRecords.DurationFrequencyId.ToString()) && FreqSrc.IsActive == true
                                             select FreqSrc.Description);

                        foreach (string strDesc in FrequencyDesc)
                        {
                            FreqDesc = strDesc;
                        }
                    }
                    PastMedicalData.Add(new PastMedicalRecord
                    {
                        PastMedicalId = PMRecords.ALMedicalHistoryDetailId,
                        PastDiseaseId = PMRecords.PastDiseaseId,
                        DurationFrequencyId = PMRecords.DurationFrequencyId,
                        Code = PMRecords.RGICLDiseaseProcedureMaster.RGICLCode,
                        Desc = PMRecords.RGICLDiseaseProcedureMaster.DiagnosisName,
                        Duration = PMRecords.DurationDays,
                        Frequency = FreqDesc,
                        FreeText = PMRecords.Details
                    });
                }
            }
            alCaseDetailsViewModel.PMRecords = PastMedicalData;
            return alCaseDetailsViewModel;
        }

        public CostMaster GetCostMasters(int alDetailId, int alCostDetailID, int providerBasicDetailId, bool isReadOnly)
        {
            CostMaster costMaster = new CostMaster();
            List<GenericKeyValue> lstRoomNameAndRent = new List<GenericKeyValue>();
            List<GenericKeyValue> lstRoomName = new List<GenericKeyValue>();
            List<GenericKeyValue> lstRoomTypeName = new List<GenericKeyValue>();
            List<ProviderRoomAndRGCILRoomDetail> lstProviderRoomAndRGCILRoomDetail = new List<ProviderRoomAndRGCILRoomDetail>();
            List<RGICL.ProviderPortal.Domain.Models.Master.BillTypeMaster> lstPackageType = new List<RGICL.ProviderPortal.Domain.Models.Master.BillTypeMaster>();
            if (alDetailId != 0 && alCostDetailID != 0)
            {
                ALCostDetailRequest ALCostDetailsRequest = new ALCostDetailRequest();
                PreAuthDetailsRequest reqStatusID = new PreAuthDetailsRequest();

                ALCostDetailsRequest.ALDetailId = alDetailId;
                ALCostDetailsRequest.ALCostDetailId = alCostDetailID;
                ALCostDetailsRequest.ProviderBasicDetailId = providerBasicDetailId;
                ALCostDetailsRequest.IsReadOnly = isReadOnly;
                reqStatusID.ALDetailId = alDetailId;
                ALCostDetailResponse respRoomNameAndRent = new ALCostDetailResponse();
                BillTypeMasterResponse respBillType = new BillTypeMasterResponse();
                using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
                {
                    respRoomNameAndRent = proxy.Channel.GetHospitalRoomNames(ALCostDetailsRequest);
                    respBillType = proxy.Channel.GetBillTypeMasters();
                }

                foreach (var item in respRoomNameAndRent.ProviderRoomAndRGCILRoomDetails)
                {
                    lstRoomNameAndRent.Add(new RGICL.ProviderPortal.Domain.Models.Master.GenericKeyValue { ID = item.ProviderRoomAndRgiclRoomDetailId, Description = item.RoomMaster.RoomDescription + "    " + item.RoomRate });
                    lstRoomName.Add(new RGICL.ProviderPortal.Domain.Models.Master.GenericKeyValue { ID = item.ProviderRoomAndRgiclRoomDetailId, Description = item.RoomMaster.RoomDescription });
                    lstRoomTypeName.Add(new RGICL.ProviderPortal.Domain.Models.Master.GenericKeyValue { ID = item.ProviderRoomAndRgiclRoomDetailId, Description = item.RoomTypeClassificationMaster.RoomTypeDescription });
                    lstProviderRoomAndRGCILRoomDetail.Add(new ProviderRoomAndRGCILRoomDetail
                    {
                        ProviderRoomAndRgiclRoomDetailId = item.ProviderRoomAndRgiclRoomDetailId,
                        HospitalRoomNameId = item.HospitalRoomNameId,
                        RgiclRoomNameId = item.RgiclRoomNameId,
                        ProviderBasicDetailId = item.ProviderBasicDetailId,
                        ProviderPackageDetailId = item.ProviderPackageDetailId,
                        EffectiveStartDate = item.ProviderPackageDetails == null ? null : item.ProviderPackageDetails.EffectiveStartDate,
                        EffectiveEndDate = item.ProviderPackageDetails == null ? null : item.ProviderPackageDetails.EffectiveEndDate,
                        IsStatusVoid = item.ProviderPackageDetails == null ? null : item.ProviderPackageDetails.IsStatusVoid,
                    });
                }

                List<RGICL.ProviderPortal.Domain.Models.Master.BillTypeMaster> lstMatBillType = new List<RGICL.ProviderPortal.Domain.Models.Master.BillTypeMaster>();

                foreach (var item in respBillType.BillTypeMaster)
                {
                    if (item.IsActive == true)
                    {
                        lstPackageType.Add(new RGICL.ProviderPortal.Domain.Models.Master.BillTypeMaster { BillType = item.BillType, BillTypeMasterId = item.BillTypeMasterId, IsActive = item.IsActive, Priority = item.Priority });
                    }
                }
            }
            costMaster.RoomNameandRent = lstRoomNameAndRent;
            costMaster.RoomName = lstRoomName;
            costMaster.BillType = lstPackageType;
            costMaster.RoomTypeName = lstRoomTypeName;
            costMaster.ProviderRoomAndRGCILRoomDetail = lstProviderRoomAndRGCILRoomDetail;
            return costMaster;
        }
        public List<RGICL.ProviderPortal.Domain.Models.Master.BillTypeMaster> GetMaternityBill(string policyClassification, string policyNo, int? productId, string dOA, bool isDataFound)
        {
            List<RGICL.ProviderPortal.Domain.Models.Master.BillTypeMaster> lstMatBillType = new List<RGICL.ProviderPortal.Domain.Models.Master.BillTypeMaster>();

            ALCostDetailResponse MatBillType = new ALCostDetailResponse();

            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                if (isDataFound)
                {
                    MatBillType = proxy.Channel.GetMaternityBenefits(policyClassification, policyNo, Util.ConvertToInt(productId), String.IsNullOrEmpty(dOA) ? DateTime.Now : Convert.ToDateTime(dOA));
                }
                else
                {
                    MatBillType = proxy.Channel.GetBenefitsMaster(1);
                    MatBillType.BenefitsMasterList = MatBillType.BenefitsMaster.ToList();

                    if (MatBillType.BenefitsMasterList.Where(benefit => benefit.BenefitsSubCoverage == "Pre-Post Natal").Any())
                    {
                        MatBillType.BenefitsMasterList.Remove(MatBillType.BenefitsMasterList.Find(benefit => benefit.BenefitsSubCoverage == "Pre-Post Natal"));
                    }

                    if (MatBillType.BenefitsMasterList.Where(benefit => benefit.BenefitsSubCoverage == "Baby Day1").Any())
                    {
                        MatBillType.BenefitsMasterList.Remove(MatBillType.BenefitsMasterList.Find(benefit => benefit.BenefitsSubCoverage == "Baby Day1"));
                    }
                }
            }

            foreach (var item in MatBillType.BenefitsMasterList)
            {
                if (item.IsActive == true)
                {
                    string strBillType = item.ParentID == null ? item.BenefitsMainCoverage : item.BenefitsSubCoverage;
                    lstMatBillType.Add(new RGICL.ProviderPortal.Domain.Models.Master.BillTypeMaster { BillType = strBillType, BillTypeMasterId = item.BenefitsID, IsActive = item.IsActive });
                }
            }
            return lstMatBillType;
        }

        public ALCostDetail GetCostDetails(int alDetailId, int alCostDetailID, int providerBasicDetailId, string ViewMode)
        {
            /// <Summary>
            /// Code for AL Cost
            /// <Summary>
            ALCostDetailResponse ALCostDetailsResponse = new ALCostDetailResponse();
            //ALCostDetailResponse MatBillType = new ALCostDetailResponse();
            //ALCostDetailResponse respRoomNameAndRent = new ALCostDetailResponse();
            ALCostDetailResponse respBenefits = new ALCostDetailResponse();
            PreAuthDetailsResponse respClaimStatusID = new PreAuthDetailsResponse();
            //BillTypeMasterResponse respBillType = new BillTypeMasterResponse();
            ALCostDetailRequest ALCostDetailsRequest = new ALCostDetailRequest();
            PreAuthDetailsRequest reqStatusID = new PreAuthDetailsRequest();
            ALCostDetailsRequest.ALDetailId = alDetailId;
            ALCostDetailsRequest.ALCostDetailId = alCostDetailID;
            ALCostDetailsRequest.ProviderBasicDetailId = providerBasicDetailId;
            reqStatusID.ALDetailId = alDetailId;

            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                ALCostDetailsResponse = proxy.Channel.GetALCostDetail(ALCostDetailsRequest);
                respBenefits = proxy.Channel.GetBenefitsMaster(null);
                respClaimStatusID = proxy.Channel.GetClaimStatusId(reqStatusID);
            }

            ALCostDetails CostDetail = ALCostDetailsResponse.ALCostDetails;
            // MasterViewModel masters = GetALMasters();
            ALCostDetail CostDetailViewModel = new ALCostDetail();
            CostDetailViewModel.ALDetailId = CostDetail.ALDetailId.Value;
            CostDetailViewModel.ALCostDetailId = CostDetail.ALCostDetailId;

            if (CostDetail.RequestedRoomAndRGICLRoomDetailId.HasValue)
                CostDetailViewModel.RequestedRoomAndRGICLRoomDetailId = CostDetail.RequestedRoomAndRGICLRoomDetailId.Value;
            else
                CostDetailViewModel.RequestedRoomAndRGICLRoomDetailId = null;

            //  CostDetailViewModel.lstAdmissionType = masters.TypeOfAdmission;
            if (CostDetail.AdmissionTypeId.HasValue)
                CostDetailViewModel.AdmissionTypeId = CostDetail.AdmissionTypeId.Value;
            else
                CostDetailViewModel.AdmissionTypeId = null;
            //CostDetailViewModel.AdmissionTypeId = masters.TypeOfAdmission.Where(de => de.Description == "Planned").Select(did => did.CodeMasterId).FirstOrDefault();

            DateTime ExpDOAdate;
            if (CostDetail.DOA != null)
            {
                ExpDOAdate = DateTime.Parse(CostDetail.DOA.ToString());
                ExpDOAdate = ExpDOAdate.Date;
                CostDetailViewModel.DateofAdmission = ExpDOAdate.ToString("dd/MMM/yyyy");
            }
            else
            {
                CostDetailViewModel.DateofAdmission = "";
            }

            CostDetailViewModel.DOA = CostDetail.DOA;

            if (CostDetail.IsReAdmission.HasValue)
                CostDetailViewModel.IsReadmission = CostDetail.IsReAdmission;
            else
                CostDetailViewModel.IsReadmission = false;

            if (CostDetail.LengthOfStay.HasValue)
                CostDetailViewModel.LengthOfStay = (int)CostDetail.LengthOfStay;
            else
                CostDetailViewModel.LengthOfStay = null;

            // CostDetailViewModel.lstFrequency = masters.Frequency;
            if (CostDetail.FrequencyId.HasValue)
                CostDetailViewModel.FrequencyId = (int)CostDetail.FrequencyId;
            else
                CostDetailViewModel.FrequencyId = null;

            if (CostDetail.PatientPaidAmount.HasValue)
                CostDetailViewModel.PatientPaidAmount = CostDetail.PatientPaidAmount.Value;
            else
                CostDetailViewModel.PatientPaidAmount = null;

            if (CostDetail.IsPackageRate.HasValue)
                CostDetailViewModel.IsPackageRate = CostDetail.IsPackageRate;
            else
                CostDetailViewModel.IsPackageRate = false;

            List<CostBillRecord> CostBillData = new List<CostBillRecord>();
            foreach (var item in ALCostDetailsResponse.ALCostDetails.ALCostBillDetails)
            {
                if (item.IsDeleted == false)
                {
                    if (item.ALCostDetailId == ALCostDetailsRequest.ALCostDetailId)
                    {
                        string strBillType;
                        int BillID;
                        if (item.BenefitsMaster != null)
                        {
                            if (item.BenefitsMaster.BenefitsMainCoverage == ApplicationConstant.AuthorizationLetter.Maternity_BabyDay)
                            {
                                strBillType = item.BenefitsMaster.BenefitsMainCoverage;
                            }
                            else
                            {
                                strBillType = item.BenefitsMaster.BenefitsSubCoverage;
                            }
                            BillID = item.BenefitsMaster.BenefitsID;
                        }
                        else
                        {
                            strBillType = "";
                            BillID = 0;
                        }
                        string strPackageType = item.BillTypeMaster.BillType;
                        int PackageID = item.BillTypeMaster.BillTypeMasterId;
                        string strSubBillType = item.BillSubTypeMaster.SubBillType;
                        int SubBillID = item.BillSubTypeId.Value;

                        DateTime billdate;
                        String displayBillDate;
                        if (item.BillDate != null)
                        {
                            billdate = DateTime.Parse(item.BillDate.ToString());
                            displayBillDate = billdate.ToString("dd/MMM/yyyy");
                        }
                        else
                        {
                            displayBillDate = "";
                        }
                        CostBillData.Add(new CostBillRecord
                                                {
                                                    ALCostBillID = item.ALCostBillDetailId,
                                                    MatBillType = strBillType,
                                                    PackageType = strPackageType,
                                                    SubBillType = strSubBillType.ToString(),
                                                    BillNumber = item.BillNo,
                                                    BillDate = displayBillDate,
                                                    NoOfUnit = item.NoOfUnit,
                                                    Amount = item.ClaimAmount.GetValueOrDefault(),
                                                    MatBillTypeID = BillID,
                                                    PackageTypeID = PackageID,
                                                    SubBillTypeID = SubBillID,
                                                    IsForAllowance = item.BillSubTypeMaster.IsForAllowance
                                                });
                    }
                }
            }

            //The below code is made to get execute only for view mode to improve performance.
            if (ViewMode == "True")
            {
                ViewCostCummulative CostCummulativeData = new ViewCostCummulative();
                if (ALCostDetailsResponse.ALCostDetails.ALCostBillDetails != null)
                {
                    ICollection<ALCostBillDetails> bills = ALCostDetailsResponse.ALCostDetails.ALCostBillDetails.Where(x => x.IsDeleted == false).ToList();

                    ALCummulativeCopayDiscDetails cumulativeCopayDiscountDetail = new ALCummulativeCopayDiscDetails();
                    if (ALCostDetailsResponse.ALCostDetails.ALCumulativeCopayDiscDetails != null)
                    {
                        cumulativeCopayDiscountDetail = ALCostDetailsResponse.ALCostDetails.ALCumulativeCopayDiscDetails.Where(x => x.IsDeleted == false).FirstOrDefault();
                    }

                    if (respClaimStatusID.ClaimStatusId == 110 || respClaimStatusID.ClaimStatusId == 111 || respClaimStatusID.ClaimStatusId == 120 || respClaimStatusID.ClaimStatusId == 140)
                    {
                        decimal copay = (decimal)bills.Sum(x => x.ApprovedCopay);
                        decimal discount = (decimal)bills.Sum(x => x.ApprovedDiscount);
                        decimal approvedNSA = (decimal)bills.Sum(x => x.ApprovedNSA);
                        decimal approvedGSA = (decimal)bills.Sum(x => x.GSA);
                        decimal requestedAmount = (decimal)bills.Sum(x => x.ClaimAmount);
                        decimal totalDisallowedAmount = default(decimal);
                        foreach (var bill in bills)
                        {
                            totalDisallowedAmount += (bill.ClaimAmount != null && bill.GSA != null) ? (decimal)bill.ClaimAmount - (decimal)bill.GSA : 0;
                        }

                        if (cumulativeCopayDiscountDetail != null)
                        {
                            if (cumulativeCopayDiscountDetail.Copay > 0 && copay == 0)
                            {
                                approvedNSA = approvedNSA - cumulativeCopayDiscountDetail.Copay;
                                copay = cumulativeCopayDiscountDetail.Copay;
                            }
                            if (cumulativeCopayDiscountDetail.Discount > 0 && discount == 0)
                            {
                                approvedNSA = approvedNSA - cumulativeCopayDiscountDetail.Discount;
                                discount = cumulativeCopayDiscountDetail.Discount;
                            }
                        }
                        ////Get Approved GSA & NSA
                        //foreach (var item in ALCostDetailsResponse.ALCostDetails.ALCostBillDetails)
                        //{
                        CostCummulativeData.cumulativeTotal = requestedAmount;
                        CostCummulativeData.cumulativeReqAmt = requestedAmount;
                        CostCummulativeData.cumulativeGSA = approvedGSA;
                        CostCummulativeData.cumulativeDisallowedAmt = totalDisallowedAmount;
                        CostCummulativeData.cumulativeCoPay = copay;
                        CostCummulativeData.cumulativeDiscount = discount;
                        CostCummulativeData.cumulativeNSA = approvedNSA;
                        //}
                    }
                    else
                    {
                        ////Get Executive GSA & NSA
                        CostCummulativeData.cumulativeTotal = CostBillData.Sum(BillSum => BillSum.Amount);
                        CostCummulativeData.cumulativeReqAmt = CostBillData.Sum(BillSum => BillSum.Amount);
                        CostCummulativeData.cumulativeGSA = 0;
                        CostCummulativeData.cumulativeDisallowedAmt = 0;
                        CostCummulativeData.cumulativeCoPay = 0;
                        CostCummulativeData.cumulativeDiscount = 0;
                        CostCummulativeData.cumulativeNSA = 0;
                    }
                }

                if (respClaimStatusID.ClaimStatusId != 110 && respClaimStatusID.ClaimStatusId != 111 && respClaimStatusID.ClaimStatusId != 120 && respClaimStatusID.ClaimStatusId != 140)
                {
                    if (ALCostDetailsResponse.ALCostDetails.ALBenefitsCostSummaries != null)
                    {
                        ALCostDetailsResponse.ALCostDetails.ALBenefitsCostSummaries = null;
                    }
                }
                List<ViewCostBenefit> BenefitData = new List<ViewCostBenefit>();
                if (ALCostDetailsResponse.ALCostDetails.ALBenefitsCostSummaries != null)
                {
                    foreach (var BenefitSummary in ALCostDetailsResponse.ALCostDetails.ALBenefitsCostSummaries)
                    {
                        decimal varReqAmt = BenefitSummary.RequestedAmount.HasValue ? BenefitSummary.RequestedAmount.Value : default(decimal);
                        decimal varGSA = BenefitSummary.GSA.HasValue ? BenefitSummary.GSA.Value : default(decimal);
                        if (!BenefitSummary.IsDeleted)
                        {
                            BenefitData.Add(new ViewCostBenefit
                                                {
                                                    Benefit = this.GetBenefitMasterDescription(BenefitSummary.BenefitId, BenefitSummary.SubBenefitId, ALCostDetailsResponse.ValidDiseaseProcedureCodes, respBenefits.BenefitsMaster),
                                                    ReqAmt = varReqAmt,
                                                    GSA = varGSA,
                                                    DisallowedAmount = varReqAmt - varGSA,
                                                    CoPay = BenefitSummary.CoPay.HasValue ? BenefitSummary.CoPay.Value : default(decimal),
                                                    Discount = BenefitSummary.Discount.HasValue ? BenefitSummary.Discount.Value : default(decimal),
                                                    NSA = BenefitSummary.NSA.HasValue ? BenefitSummary.NSA.Value : default(decimal)
                                                });
                        }
                    }
                }
                CostDetailViewModel.ViewCostCummulativeDetails = CostCummulativeData;
                CostDetailViewModel.ViewCostBenefitDetails = BenefitData;
            }

            CostDetailViewModel.CostBillRecords = CostBillData;
            CostDetailViewModel.CostRecords = CostBillData;

            List<CostRoomRecord> CostRoomData = new List<CostRoomRecord>();
            foreach (var item in CostDetail.ALCostRoomSelectionDetails)
            {

                string strHospitalRoom = item.HospitalRoomName.RoomDescription;
                //string strRoomType = item.RequestedRoomType.RoomTypeDescription;

                DateTime fromdate;
                String displayFromDate;

                DateTime todate;
                String displayToDate;
                if (item.FromDate != null)
                {
                    fromdate = DateTime.Parse(item.FromDate.ToString());
                    displayFromDate = fromdate.ToString("dd/MMM/yyyy");
                }
                else
                {
                    displayFromDate = "";
                }
                if (item.ToDate != null)
                {
                    todate = DateTime.Parse(item.ToDate.ToString());
                    displayToDate = todate.ToString("dd/MMM/yyyy");
                }
                else
                {
                    displayToDate = "";
                }
                CostRoomData.Add(new CostRoomRecord
                {
                    ALCostRoomSelectionDetailId = item.ALCostRoomSelectionDetailId,
                    HospitalRoomNameId = item.HospitalRoomNameId,
                    HospitalRoomName = strHospitalRoom,
                    RequestedRoomTypeId = item.RequestedRoomTypeId,
                    RequestedRoomType = null,
                    RequestedRoomAndRGICLRoomDetailId = item.RequestedRoomAndRGICLRoomDetailId,
                    FromDate = displayFromDate,
                    ToDate = displayToDate,
                    NoOfUnit = item.NoOfUnits
                });
            }
            List<CostRoomHistory> lstRoomHistory = new List<CostRoomHistory>();
            foreach (var item in CostDetail.ALCostRoomSelectionDetails.Where(x => x.IsDeleted == false).ToList())
            {
                lstRoomHistory.Add(new CostRoomHistory
                {
                    ALCostRoomSelectionDetailId = item.ALCostRoomSelectionDetailId,
                    HospitalRoomNameId = item.HospitalRoomNameId,
                    RequestedRoomTypeId = item.RequestedRoomTypeId,
                    FromDate = item.FromDate,
                    ToDate = item.ToDate,
                    RequestedRoomAndRGICLRoomDetailId = item.SanctionedRoomAndRGICLRoomDetailId,
                    NoOfUnit = item.NoOfUnits,
                    IsDeleted = item.IsDeleted
                });
            }

            CostDetailViewModel.CostRoomRecord = CostRoomData;
            CostDetailViewModel.ALCostRoomHistoryDetails = lstRoomHistory;
            return CostDetailViewModel;
        }

        /// <summary>
        /// Returns the Benefits Description
        /// </summary>
        /// <param name="BenefitId">BenefitId</param>
        /// <returns></returns>
        /// Added for ACDM - Disease Procedure changes :: Get BenefitMaster Description based on benefits id and sub benefit id
        private string GetBenefitMasterDescription(int? BenefitId, int? subBenefitsId, List<RGICLDiseaseProcedureMaster> validDiseaseProcedureCodes, IList<BenefitsMaster> benefitsMasterList)
        {
            string benefitsdescription = string.Empty;
            if (BenefitId != null)
            {
                if (BenefitId == (int)ProductBenefitsMaster.DiseaseProcedure && subBenefitsId.HasValue)
                {
                    if (validDiseaseProcedureCodes != null && validDiseaseProcedureCodes.Any())
                    {
                        benefitsdescription = validDiseaseProcedureCodes.Find(x => x.RGICLDiseaseProcedureID == subBenefitsId.Value) != null
                                            ? validDiseaseProcedureCodes.Find(x => x.RGICLDiseaseProcedureID == subBenefitsId.Value).RGICLCode
                                            : string.Empty;
                    }
                }
                if (string.IsNullOrEmpty(benefitsdescription) && benefitsMasterList != null && benefitsMasterList.Any())
                {
                    BenefitsMaster benefits = benefitsMasterList.ToList().Find(x => BenefitId == x.BenefitsID);
                    if (benefits != null)
                    {
                        benefitsdescription = benefits.ParentID == null ? benefits.BenefitsMainCoverage : benefits.BenefitsSubCoverage;
                    }
                }
            }
            return benefitsdescription;
        }

        //public MasterViewModel GetALMasters()
        //{
        //    GetCodeMasterTypesRequest getCodeMasterTypesRequest = new GetCodeMasterTypesRequest();
        //    GetCodeMasterTypesResponse getCodeMasterTypesResponse = new GetCodeMasterTypesResponse();
        //    getCodeMasterTypesRequest.MasterTypes = new string[] { "TreatmentPlan", "ExpectedLengthofStay", "Physician Specialization", "TypeofAdmission", "Gender" };
        //    using (var proxy = new ServiceProxy<IMasterService>())
        //    {
        //        getCodeMasterTypesResponse = proxy.Channel.GetMasterTypeDetails(getCodeMasterTypesRequest);
        //    }

        //    List<RGICL.ProviderPortal.Domain.Models.Master.CodeMaster> frequency = new List<RGICL.ProviderPortal.Domain.Models.Master.CodeMaster>();
        //    List<RGICL.ProviderPortal.Domain.Models.Master.CodeMaster> treatmentPlan = new List<RGICL.ProviderPortal.Domain.Models.Master.CodeMaster>();
        //    List<RGICL.ProviderPortal.Domain.Models.Master.CodeMaster> specialization = new List<RGICL.ProviderPortal.Domain.Models.Master.CodeMaster>();
        //    List<RGICL.ProviderPortal.Domain.Models.Master.CodeMaster> CostAdmType = new List<RGICL.ProviderPortal.Domain.Models.Master.CodeMaster>();
        //    List<RGICL.ProviderPortal.Domain.Models.Master.CodeMaster> gender = new List<RGICL.ProviderPortal.Domain.Models.Master.CodeMaster>();
        //    List<RGICL.ProviderPortal.Domain.Models.Master.CodeMaster> relation = new List<RGICL.ProviderPortal.Domain.Models.Master.CodeMaster>();
        //    foreach (var item in getCodeMasterTypesResponse.MasterTypes)
        //    {
        //        if (item.Key == "ExpectedLengthofStay")
        //        {
        //            foreach (var codeItem in item.Value)
        //            {
        //                frequency.Add(new RGICL.ProviderPortal.Domain.Models.Master.CodeMaster { CodeMasterId = codeItem.CodeMasterId, Description = codeItem.Description, IsActive = codeItem.IsActive, Type = codeItem.Type });
        //            }
        //        }
        //        else if (item.Key == "TreatmentPlan")
        //        {
        //            foreach (var codeItem in item.Value)
        //            {
        //                treatmentPlan.Add(new RGICL.ProviderPortal.Domain.Models.Master.CodeMaster { CodeMasterId = codeItem.CodeMasterId, Description = codeItem.Description, IsActive = codeItem.IsActive, Type = codeItem.Type });
        //            }
        //        }
        //        else if (item.Key == "Physician Specialization")
        //        {
        //            foreach (var codeItem in item.Value)
        //            {
        //                specialization.Add(new RGICL.ProviderPortal.Domain.Models.Master.CodeMaster { CodeMasterId = codeItem.CodeMasterId, Description = codeItem.Description, IsActive = codeItem.IsActive, Type = codeItem.Type });
        //            }
        //        }
        //        else if (item.Key == "TypeofAdmission")
        //        {
        //            foreach (var codeItem in item.Value)
        //            {
        //                CostAdmType.Add(new RGICL.ProviderPortal.Domain.Models.Master.CodeMaster { CodeMasterId = codeItem.CodeMasterId, Description = codeItem.Description, IsActive = codeItem.IsActive, Type = codeItem.Type });
        //            }
        //        }
        //        else if (item.Key == "Gender")
        //        {
        //            foreach (var codeItem in item.Value)
        //            {
        //                gender.Add(new RGICL.ProviderPortal.Domain.Models.Master.CodeMaster { CodeMasterId = codeItem.CodeMasterId, Description = codeItem.Description, IsActive = codeItem.IsActive, Type = codeItem.Type });
        //            }
        //        }
        //    }

        //    List<QualificationMaster> qualificationResponse = new List<QualificationMaster>();
        //    List<Qualification> qualification = new List<Qualification>();
        //    using (var proxy = new ServiceProxy<IMasterService>())
        //    {
        //        qualificationResponse = proxy.Channel.GetQualificationMaster();
        //    }
        //    foreach (var item in qualificationResponse)
        //    {
        //        qualification.Add(new Qualification { Description = item.Description, QualificationId = item.QualificationId });
        //    }

        //    PolicyClassificationResponse policyClassificationResponse = new PolicyClassificationResponse();
        //    using (var proxy = new ServiceProxy<IMasterService>())
        //    {
        //        policyClassificationResponse = proxy.Channel.GetPolicyClassification();
        //    }
        //    string policy = JsonConvert.SerializeObject(policyClassificationResponse.policyClassificationMaster);
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    List<PolicyClassification> policyClassification = serializer.Deserialize<List<PolicyClassification>>(policy);
        //    policyClassification.Remove(policyClassification.FirstOrDefault(x => x.PolicyClassificationDescription == "Mass"));
        //    List<ProductMaster> productMaster = new List<ProductMaster>();
        //    using (var proxy = new ServiceProxy<IMasterService>())
        //    {
        //        productMaster = proxy.Channel.GetProductMaster();
        //    }
        //    string product = JsonConvert.SerializeObject(productMaster);
        //    serializer = new JavaScriptSerializer();
        //    List<RGICLProductMaster> ProductMasterRGICL = serializer.Deserialize<List<RGICLProductMaster>>(product);
        //    PolicyRelationshipMasterResponse relationMaster = new PolicyRelationshipMasterResponse();
        //    using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
        //    {
        //        relationMaster = proxy.Channel.GetPolicyRelationMaster();
        //    }
        //    foreach (var codeItem in relationMaster.PolicyRelationshipMaster)
        //    {
        //        relation.Add(new RGICL.ProviderPortal.Domain.Models.Master.CodeMaster { CodeMasterId = codeItem.RelationCode, Description = codeItem.Description, IsActive = codeItem.IsActive });
        //    }
        //    ////Get Document master
        //    List<DocumentMaster> documentMasterResponse = new List<DocumentMaster>();
        //    List<DocumentMasterPortal> documentMaster = new List<DocumentMasterPortal>();
        //    using (var proxy = new ServiceProxy<IMasterService>())
        //    {
        //        documentMasterResponse = proxy.Channel.GetDocumentMasterByDocumentType(ApplicationConstant.AuthorizationLetter.AL_DOCUMENT_TYPE_MAIN_DOCUMENT).Where(x=>x.Active==true).ToList();
        //    }

        //    foreach (DocumentMaster item in documentMasterResponse)
        //    {
        //        documentMaster.Add(new DocumentMasterPortal
        //        {
        //            DocumentId = item.DocumentId,
        //            LoBId = item.LoBId,
        //            Description = item.Description,
        //            DocumentType = item.DocumentType,
        //            Productid = item.Productid,
        //            Active = item.Active,
        //            TypeOfLossID = item.TypeOfLossID,
        //            IsDefaultSet = item.IsDefaultSet,
        //            IsMandatory = item.IsMandatory,
        //            ProcessOrder = item.ProcessOrder,
        //            DisplayOrder = item.DisplayOrder,
        //            IsProviderPortalMandatory = item.IsProviderPortalMandatory
        //        });
        //    }

        //    MasterViewModel masters = new MasterViewModel
        //    {
        //        TreatmentPlan = treatmentPlan,
        //        Frequency = frequency,
        //        Specialization = specialization,
        //        QualificationMaster = qualification,
        //        TypeOfAdmission = CostAdmType,
        //        DocumentMaster = documentMaster,
        //        Gender = gender,
        //        PolicyClassification = policyClassification,
        //        ProductMaster = ProductMasterRGICL,
        //        RelationShip = relation,
        //        InvestigationTypeMaster = GetAllInvestigationDetailsMaster()
        //    };
        //    return masters;
        //}

        ////Added by Venkatesh
        public List<SubBillTypeMaster> GetSubBillType(int billTypeId, bool? isActive)
        {
            SubBillTypeMaster resSubBillType = new SubBillTypeMaster();
            BillSubTypeMasterResponse respSubBillType = new BillSubTypeMasterResponse();


            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                respSubBillType = proxy.Channel.GetBillSubTypeMasters(billTypeId, isActive);
            }

            List<ECS.Model.Models.Master.BillSubTypeMaster> NewRecord = respSubBillType.BillSubTypeMaster;
            List<SubBillTypeMaster> retBillType = new List<SubBillTypeMaster>();
            foreach (var NR in NewRecord)
            {
                SubBillTypeMaster retBill = new SubBillTypeMaster()
                {
                    BillSubTypeMasterId = NR.BillSubTypeMasterId,
                    BillCode = NR.BillCode,
                    BillTypeId = NR.BillTypeId,
                    SubBillType = NR.SubBillType,
                    IsActive = NR.IsActive,
                    IsForAllowance = NR.IsForAllowance

                };
                retBillType.Add(retBill);
            }
            return retBillType;
        }

        [HttpGet]
        public List<RGICLDiseaseProcedureMasterViewModel> RGICLDiseaseProcedureMasterDetails(string rgiclCode, string treatmentPlan, string hospitalCode, DateTime? doa, int providerID)
        {
            RGICLDiseaseProcedureMasterRequest rgiclDiseaseProcedureMasterRequest = new RGICLDiseaseProcedureMasterRequest();
            RGICLDiseaseProcedureMasterResponse rgiclDiseaseProcedureMasterResponse = new RGICLDiseaseProcedureMasterResponse();
            rgiclDiseaseProcedureMasterRequest.RGICLCode = rgiclCode;
            rgiclDiseaseProcedureMasterRequest.Doa = doa;
            rgiclDiseaseProcedureMasterRequest.ProviderBasicDetailId = providerID;
            rgiclDiseaseProcedureMasterRequest.HospitalCode = hospitalCode;
            rgiclDiseaseProcedureMasterRequest.PageIndex = 0;
            rgiclDiseaseProcedureMasterRequest.PageSize = 10000;

            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                rgiclDiseaseProcedureMasterResponse = proxy.Channel.GetRGICLDiseaseProcedureMasterDetails(rgiclDiseaseProcedureMasterRequest);
            }

            List<RgiclAndHospitalCodeMapping> rgiclAndHospitalCodeMapping = new List<RgiclAndHospitalCodeMapping>();
            if (CodeMasterTypes.TreatmentPlan.Medical.ToString().Equals(treatmentPlan.Trim()))
                rgiclAndHospitalCodeMapping = rgiclDiseaseProcedureMasterResponse.RgiclDiseaseProcedureSearchResponse.Where(x => x.Treatment == CodeMasterTypes.TreatmentPlan.Medical.ToString() ||  x.Treatment.ToLower() =="both").ToList();
            else if (CodeMasterTypes.TreatmentPlan.Surgical.ToString().Equals(treatmentPlan.Trim()))
                rgiclAndHospitalCodeMapping = rgiclDiseaseProcedureMasterResponse.RgiclDiseaseProcedureSearchResponse.Where(x => x.Treatment == CodeMasterTypes.TreatmentPlan.Surgical.ToString() || x.Treatment.ToLower() == "both").ToList();
            else
                rgiclAndHospitalCodeMapping = rgiclDiseaseProcedureMasterResponse.RgiclDiseaseProcedureSearchResponse.ToList();
            List<RGICLDiseaseProcedureMasterViewModel> rgiclDiseaseProcedureMasterList = new List<RGICLDiseaseProcedureMasterViewModel>();
            foreach (var diseaseProcedureMaster in rgiclAndHospitalCodeMapping)
            {
                RGICLDiseaseProcedureMasterViewModel rgiclDiseaseProcedureMasterViewModel = new RGICLDiseaseProcedureMasterViewModel()
                {
                    Department_ID = diseaseProcedureMaster.DepartmentId,
                    RGICLDiseaseProcedureID = diseaseProcedureMaster.RgiclCodeId,
                    Description = diseaseProcedureMaster.RgiclCodeDescription,
                    ProcedureName = diseaseProcedureMaster.ProcedureName,
                    GroupName = diseaseProcedureMaster.GroupName,
                    Code = diseaseProcedureMaster.RgiclCode,
                    Treatment = diseaseProcedureMaster.Treatment,
                    HospitalCode = diseaseProcedureMaster.NetworkCode == null ? " " : diseaseProcedureMaster.NetworkCode,
                    HospitalDescription = diseaseProcedureMaster.NetworkCodeDescription == null ? " " : diseaseProcedureMaster.NetworkCodeDescription
                };

                rgiclDiseaseProcedureMasterList.Add(rgiclDiseaseProcedureMasterViewModel);
            }

            return rgiclDiseaseProcedureMasterList;
        }


        [HttpGet]
        public IcdCptHospitalSpecializationViewModel GetHospitalSpeciality(string DoctorSpecialityId, string SearchType, int? ProviderBasicdetailsId, string TreatmentPlan)
        {
            List<int?> DoctorSpecialitList = new List<int?>();
            if (DoctorSpecialityId != null)
            {
                List<int> DoctorSpecialityIdlist = DoctorSpecialityId.Split(',').Select(int.Parse).ToList();
                DoctorSpecialitList = DoctorSpecialityIdlist.Cast<int?>().ToList();
            }
            ProviderPortalCommonRequest getHospitalSpecialityRequest = new ProviderPortalCommonRequest();
            ProviderPortalCommonResponseServices getHospitalSpecialityResponse = new ProviderPortalCommonResponseServices();
            IcdCptHospitalSpecializationViewModel icdCptHospitalSpecializationViewModel = new IcdCptHospitalSpecializationViewModel();
            getHospitalSpecialityRequest.DoctorSpecialityIds = DoctorSpecialitList;
            getHospitalSpecialityRequest.ClaimAcknowledgmentDetails = null;
            getHospitalSpecialityRequest.IcdCptSearchType = SearchType.Contains("Icd") == true ? "Icd" : "Cpt";
            getHospitalSpecialityRequest.ProviderBasicdetailsId = ProviderBasicdetailsId;
            getHospitalSpecialityRequest.TreatmentPlan = TreatmentPlan;
            using (var proxy = new ServiceProxy<IProviderPortalCommonService>())
            {
                getHospitalSpecialityResponse = proxy.Channel.GetIcdAndCptCodesForHospitalAndSpeciality(getHospitalSpecialityRequest);

            }
            List<ProviderIcdAndCptCodesHistory> getHospitalIcdandCptCode = getHospitalSpecialityResponse.RecentIcdAndCptCodes.RecentIcdCptCodesForHospital;
            List<ProviderIcdAndCptCodesHistory> getSpecializationIcdandCptCode = getHospitalSpecialityResponse.RecentIcdAndCptCodes.RecentIcdCptCodesForSpecialization;
            icdCptHospitalSpecializationViewModel.IcdCptHospital = getHospitalIcdandCptCode;
            icdCptHospitalSpecializationViewModel.IcdCptSpecialization = getSpecializationIcdandCptCode;
            return icdCptHospitalSpecializationViewModel;
        }

        [HttpGet]
        public List<RGISLICDMasterViewModel> GetRGICLICDMasterDetails(string Code)
        {
            ICDMasterRequest rgiclICDMasterRequest = new ICDMasterRequest();
            ICDMasterResponse rgiclICDMasterResponse = new ICDMasterResponse();
            rgiclICDMasterRequest.ICDCode = Code;
            rgiclICDMasterRequest.PageIndex = 0;
            rgiclICDMasterRequest.PageSize = 10000;

            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                rgiclICDMasterResponse = proxy.Channel.GetICDMasterDetails(rgiclICDMasterRequest);

            }

            List<ICDMaster> rgiclICDMasters = rgiclICDMasterResponse.GetICDMasterDetails;
            List<RGISLICDMasterViewModel> rgiclICDMasterList = new List<RGISLICDMasterViewModel>();
            foreach (var ICDMaster in rgiclICDMasters)
            {
                rgiclICDMasterList.Add(new RGISLICDMasterViewModel { Code = ICDMaster.ICDCode, Description = ICDMaster.Description, ICDMasterId = ICDMaster.ICDMasterId });
            }
            return rgiclICDMasterList;
        }

        //public List<InvestigationType> GetAllInvestigationDetailsMaster()
        //{
        //    InvestigationTypeResponse response = new InvestigationTypeResponse();
        //    using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
        //    {
        //        response = proxy.Channel.GetAllInvestigationDetailsMaster();

        //    }
        //    List<InvestigationTypeMaster> rgiclInvestigationType = response.InvestigationTypeMaster;
        //    List<InvestigationType> modelInvestigationType = new List<InvestigationType>();

        //    foreach (var item in rgiclInvestigationType)
        //    {
        //        modelInvestigationType.Add(new InvestigationType { Code = Convert.ToString(item.InvestigationTypeId), Description = item.Description });
        //    }
        //    return modelInvestigationType;
        //}

        [HttpPost]
        public ALViewModel SavePreAuthInward(ALViewModel alModel)
        {
            PreAuthDetailsRequest request = new PreAuthDetailsRequest();
            request.ALPolicyPatientInsuredDetails = new ALPolicyPatientInsuredDetails();
            request.ALPolicyPatientInsuredDetails.PolicyNumber = alModel.PolicyNumber;
            request.ALPolicyPatientInsuredDetails.PatientUHID = alModel.PatientUHID;
            request.ALPolicyPatientInsuredDetails.PolicyClassificationId = alModel.PolicyClassificationId;
            request.ALDetail = new ALDetail();
            request.ALDetail.InwardClassificationId = alModel.InwardClassificationId;
            request.ALDetail.IsDataFound = alModel.IsDataFound;
            request.ALDetail.PreAuthType = alModel.PreAuthType;
            request.ALDetail.ProviderBasicDetailId = alModel.ProviderBasicDetailId;
            request.ABHAID = alModel.ABHAID;
            request.ABHA_Adress = alModel.ABHA_Adress;
            PreAuthDetailsResponse response = new PreAuthDetailsResponse();
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.SavePortalPreAuthIPD(request);
            }

            alModel.ALInwardDetailId = response.PreAuthIPDDetails.ALInwardDetailID;
            alModel.ALDetailId = response.PreAuthIPDDetails.ALDetailID;
            alModel.TaskDetailId = response.PreAuthIPDDetails.TaskDetailID;
            alModel.MaxFileUploadPerDocumnetType = response.PreAuthIPDDetails.MaxFileUploadPerDocumnetType;
            alModel.CaseDetail_Data = new ALCaseDetails();
            alModel.CostDetail_Data = new ALCostDetail();
            alModel.CaseDetail_Data.ALCaseDetailId = response.PreAuthIPDDetails.ALCaseDetailID;
            alModel.CaseDetail_Data.ALDetailId = response.PreAuthIPDDetails.ALDetailID;
            alModel.CostDetail_Data.ALCostDetailId = response.PreAuthIPDDetails.ALCostDetailID;
            alModel.CostDetail_Data.ALDetailId = response.PreAuthIPDDetails.ALDetailID;
            return alModel;
        }

        [HttpPost]
        public ALViewModel CopyPortalPreAuth(ALViewModel alModel)
        {
            PreAuthDetailsRequest request = new PreAuthDetailsRequest();
            request.ALDetailId = (int)alModel.ALDetailId;
            request.ALInwardDetailId = (int)alModel.ALInwardDetailId;
            request.InwardClassificationId = alModel.InwardClassificationId;

            PreAuthDetailsResponse response = new PreAuthDetailsResponse();
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.CopyPortalPreAuth(request);
            }
            alModel.ALInwardDetailId = response.PreAuthIPDDetails.ALInwardDetailID;
            alModel.ALDetailId = response.PreAuthIPDDetails.ALDetailID;
            alModel.TaskDetailId = response.PreAuthIPDDetails.TaskDetailID;
            alModel.CaseDetail_Data = new ALCaseDetails();
            alModel.CostDetail_Data = new ALCostDetail();
            alModel.CaseDetail_Data.ALCaseDetailId = response.PreAuthIPDDetails.ALCaseDetailID;
            alModel.CaseDetail_Data.ALDetailId = response.PreAuthIPDDetails.ALDetailID;
            alModel.CostDetail_Data.ALCostDetailId = response.PreAuthIPDDetails.ALCostDetailID;
            alModel.CostDetail_Data.ALDetailId = response.PreAuthIPDDetails.ALDetailID;
            alModel.PolicyNumber = response.PreAuthIPDDetails.PolicyNo;
            alModel.PatientUHID = response.PreAuthIPDDetails.PatientUHID;
            alModel.DOA = response.PreAuthIPDDetails.DOA;
            alModel.TotalAmount = response.PreAuthIPDDetails.TotalAmount;
            alModel.MaxFileUploadPerDocumnetType = response.PreAuthIPDDetails.MaxFileUploadPerDocumnetType;
            alModel.IsDataFound = response.PreAuthIPDDetails.IsDataFound;
            alModel.ALNumber = response.PreAuthIPDDetails.ALNumber;
            alModel.PatientName = response.PreAuthIPDDetails.PatientName;
            alModel.MemberName = response.PreAuthIPDDetails.PatientName;
            alModel.ProductId = response.PreAuthIPDDetails.ProductId;
            alModel.PolicyClassificationId = response.PreAuthIPDDetails.PolicyClassificationId;
            alModel.CaseDetail_Data.IsMaternity = response.PreAuthIPDDetails.IsMaternity;
            alModel.CaseDetail_Data.IsInjuryRTASelfIinjury = response.PreAuthIPDDetails.IsInjury;
            alModel.CaseDetail_Data.IsInjury = response.PreAuthIPDDetails.IsInjury;
            return alModel;
        }

        [HttpPost]
        public ALCostDetail SaveCostBillAddDelete(ALCostDetail AddCostBill)
        {
            ////Request,Response to save Bill detail.
            PreAuthDetailsRequest req = new PreAuthDetailsRequest();
            PreAuthDetailsResponse resp = new PreAuthDetailsResponse();
            ALCostBillDetails ALCost = new ALCostBillDetails();

            ////Request,Response to fetch the Bill details
            ALCostDetailRequest reqALCostDetail = new ALCostDetailRequest();
            ALCostDetailResponse respALCostDetail = new ALCostDetailResponse();
            reqALCostDetail.ALDetailId = AddCostBill.ALDetailId;
            reqALCostDetail.ALCostDetailId = AddCostBill.ALCostDetailId;
            reqALCostDetail.ProviderBasicDetailId = AddCostBill.ProviderBasicDetailId;

            req.ALCostBillDetails = ALCost;
            req.ALCostDetail = new ALCostDetails(); //need to check if we get existing value
            req.ALCostDetail.ALDetailId = AddCostBill.ALDetailId;
            req.ALCostDetail.ALCostDetailId = AddCostBill.ALCostDetailId;
            req.TaskDetailId = AddCostBill.TaskDetailID; //need to remove once integrated
            string CostBill = JsonConvert.SerializeObject(AddCostBill.ALCostBillHistoryDetails);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<CostBillHistory> alCostBillDetail = serializer.Deserialize<List<CostBillHistory>>(CostBill);
            foreach (var item in alCostBillDetail)
            {
                req.ALCostBillDetails.ALCostBillDetailId = item.ALCostBillDetailId;
                req.ALCostBillDetails.MaternityBillTypeId = item.MatBillTypeID;
                req.ALCostBillDetails.BillNo = item.BillNumber.ToString();
                req.ALCostBillDetails.BillDate = item.BillDate;
                req.ALCostBillDetails.NoOfUnit = item.NoOfUnit;
                req.ALCostBillDetails.BillTypeId = item.PackageTypeID;
                req.ALCostBillDetails.BillSubTypeId = item.SubBillTypeID;
                req.ALCostBillDetails.ClaimAmount = item.Amount;
                req.ALCostBillDetails.ExecutiveGSA = item.Amount;
                req.ALCostBillDetails.ExecutiveEnteredGSA = item.Amount;
                req.ALCostBillDetails.ApproverEnteredGSA = item.Amount;
                req.ALCostBillDetails.ExecutiveNSA = item.Amount;
                req.ALCostBillDetails.ApprovedNSA = item.Amount;
                req.ALCostBillDetails.IsDeleted = item.IsDeleted;
            }
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                resp = proxy.Channel.SaveCostBillDetails(req);
                respALCostDetail = proxy.Channel.GetALCostDetail(reqALCostDetail);

            }

            ////Do Mapping to ALCostDetail from the response ALCostDetails
            List<CostBillHistory> lstBillHistory = new List<CostBillHistory>();
            foreach (var item in resp.ALCostDetails.ALCostBillDetails)
            {
                lstBillHistory.Add(new CostBillHistory
                {
                    ALCostBillDetailId = item.ALCostBillDetailId,
                    MatBillTypeID = item.MaternityBillTypeId,
                    BillNumber = item.BillNo,
                    BillDate = item.BillDate,
                    NoOfUnit = item.NoOfUnit,
                    PackageTypeID = item.BillTypeId.Value,
                    SubBillTypeID = item.BillSubTypeId.Value,
                    IsDeleted = item.IsDeleted
                });
            }

            List<CostBillRecord> CostBillData = new List<CostBillRecord>();
            foreach (var item in respALCostDetail.ALCostDetails.ALCostBillDetails)
            {
                string strBillType;
                int BillID;
                if (item.BenefitsMaster != null)
                {
                    if (item.BenefitsMaster.BenefitsMainCoverage == ApplicationConstant.AuthorizationLetter.Maternity_BabyDay)
                    {
                        strBillType = item.BenefitsMaster.BenefitsMainCoverage;
                    }
                    else
                    {
                        strBillType = item.BenefitsMaster.BenefitsSubCoverage;
                    }
                    BillID = item.BenefitsMaster.BenefitsID;
                }
                else
                {
                    strBillType = "";
                    BillID = 0;
                }
                string strPackageType = item.BillTypeMaster.BillType;
                int PackageID = item.BillTypeMaster.BillTypeMasterId;
                string strSubBillType = item.BillSubTypeMaster.SubBillType;
                int SubBillID = item.BillSubTypeId.Value;
                DateTime billdate;
                String displayBillDate;
                if (item.BillDate != null)
                {
                    billdate = DateTime.Parse(item.BillDate.ToString());
                    displayBillDate = billdate.ToString("dd/MMM/yyyy");
                }
                else
                {
                    displayBillDate = "";
                }
                CostBillData.Add(new CostBillRecord
                {
                    ALCostBillID = item.ALCostBillDetailId,
                    MatBillType = strBillType,
                    PackageType = strPackageType,
                    SubBillType = strSubBillType.ToString(),
                    BillNumber = item.BillNo,
                    BillDate = displayBillDate,
                    NoOfUnit = item.NoOfUnit,
                    Amount = item.ClaimAmount.GetValueOrDefault(),
                    MatBillTypeID = BillID,
                    PackageTypeID = PackageID,
                    SubBillTypeID = SubBillID,
                    IsForAllowance = item.BillSubTypeMaster.IsForAllowance
                });
            }

            ALCostDetail respCostBill = new ALCostDetail()
            {
                ALCostDetailId = req.ALCostDetail.ALCostDetailId,
                ALCostBillHistoryDetails = lstBillHistory,
                CostBillRecords = CostBillData
            };
            return respCostBill;
        }

        [HttpPost]
        public ALViewModel SaveALCaseDetails(ALViewModel alViewModel)
        {
            PreAuthDetailsResponse response = new PreAuthDetailsResponse();
            PreAuthDetailsRequest request = new PreAuthDetailsRequest();

            string caseDetail = JsonConvert.SerializeObject(alViewModel.CaseDetail_Data); // Serialization

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ALCaseDetail alCaseDetail = serializer.Deserialize<ALCaseDetail>(caseDetail);
            List<ALICDDiagnosisCodes> alICDDiagnosisCodes = new List<ALICDDiagnosisCodes>();
            alCaseDetail.ALDiseaseProcedureDetails = new List<ALDiseaseProcedureDetails>();
            alCaseDetail.ALMedicalHistoryDetails = new List<ALMedicalHistoryDetail>();
            alCaseDetail.ALDiagnosisInvestigations = new List<ALDiagnosisInvestigation>();
            alCaseDetail.ALMillimanConditions = new List<ECS.Model.Models.AuthorizationLetter.ALMillimanConditions>();
            alCaseDetail.TrackState = alCaseDetail.ALCaseDetailId == 0 ? TrackState.Added : TrackState.UpdateModified;
            if (alViewModel.CaseDetail_Data.DateofAdmission != null && alViewModel.CaseDetail_Data.DateofAdmission != "")
            {
                alCaseDetail.DOA = Convert.ToDateTime(alViewModel.CaseDetail_Data.DateofAdmission);
            }

            if (alViewModel.CaseDetail_Data.ALICDDiagnosisCodes != null)
            {
                foreach (var icdDiagnosisCode in alViewModel.CaseDetail_Data.ALICDDiagnosisCodes)
                {
                    ALICDDiagnosisCodes alICDDiagnosisCode = new ALICDDiagnosisCodes();
                    alICDDiagnosisCode.ALCaseDetailId = alViewModel.CaseDetail_Data.ALCaseDetailId;
                    alICDDiagnosisCode.ALICDDiagnosisCodeId = icdDiagnosisCode.ALICDDiagnosisCodeId;
                    alICDDiagnosisCode.ICDCodeId = icdDiagnosisCode.ICDMasterId;
                    if (icdDiagnosisCode.ICDMasterId.GetValueOrDefault() == 0)
                    {
                        alICDDiagnosisCode.ICDMaster = new ICDMaster();
                        alICDDiagnosisCode.ICDMaster.ICDCode = icdDiagnosisCode.Code;
                        alICDDiagnosisCode.ICDMaster.Description = icdDiagnosisCode.Description;
                    }

                    alICDDiagnosisCode.IsDeleted = icdDiagnosisCode.IsDeleted;
                    alICDDiagnosisCode.TrackState = icdDiagnosisCode.ALICDDiagnosisCodeId == 0 ? TrackState.Added : TrackState.UpdateModified;
                    alICDDiagnosisCodes.Add(alICDDiagnosisCode);
                }
            }

            if (alViewModel.CaseDetail_Data.ALInvestigationType != null)
            {
                foreach (var investigation in alViewModel.CaseDetail_Data.ALInvestigationType)
                {
                    alCaseDetail.ALDiagnosisInvestigations.Add(new ALDiagnosisInvestigation
                    {
                        ALCaseDetailId = alViewModel.CaseDetail_Data.ALCaseDetailId,
                        ALDiagnosisInvestigationId = investigation.ALDiagnosisInvestigationId,
                        InvestigationTypeId = investigation.MasterId,
                        InvestigationFindings = investigation.Description,
                        IsDeleted = investigation.IsDeleted,
                        TrackState = investigation.ALDiagnosisInvestigationId == 0 ? TrackState.Added : TrackState.UpdateModified
                    });
                }
            }

            List<ALDiseaseProcedureDetails> alDiseaseProcedureDetails = new List<ALDiseaseProcedureDetails>();
            if (alViewModel.CaseDetail_Data.ALDiseaseCodes != null)
            {
                foreach (var diseaseCode in alViewModel.CaseDetail_Data.ALDiseaseCodes)
                {
                    ALDiseaseProcedureDetails alDisease = new ALDiseaseProcedureDetails();

                    alDisease.ALCaseDetailId = alViewModel.CaseDetail_Data.ALCaseDetailId;
                    alDisease.ALDiseaseProcedureDetailId = diseaseCode.ALDiseaseProcedureDetailId;
                    alDisease.DiseaseProcedureCodeId = diseaseCode.RGICLDiseaseProcedureID;
                    if (diseaseCode.RGICLDiseaseProcedureID == 0)
                    {
                        alDisease.RGICLDiseaseProcedureMaster = new RGICLDiseaseProcedureMaster();
                        alDisease.RGICLDiseaseProcedureMaster.RGICLCode = diseaseCode.Code;
                        alDisease.RGICLDiseaseProcedureMaster.DiagnosisName = diseaseCode.Description;
                    }

                    alDisease.IsDeleted = diseaseCode.IsDeleted;
                    alDisease.IsPrimary = diseaseCode.IsPrimary;
                    alDisease.Proportion = diseaseCode.Proportion;
                    alDisease.NetworkProportion = diseaseCode.NetworkProportion;
                    alDisease.TreatmentType = diseaseCode.Treatment;
                    alDisease.TrackState = diseaseCode.ALDiseaseProcedureDetailId == 0 ? TrackState.Added : TrackState.UpdateModified;
                    alDisease.HospitalCode = diseaseCode.HospitalCode;
                    alDisease.HospitalCodeDescription = diseaseCode.HospitalDescription;
                    alDiseaseProcedureDetails.Add(alDisease);
                }
            }

            if (alViewModel.CaseDetail_Data.ALMedicalHistoryDetails != null)
            {
                foreach (var pastmedicalHistory in alViewModel.CaseDetail_Data.ALMedicalHistoryDetails)
                {
                    alCaseDetail.ALMedicalHistoryDetails.Add(new ALMedicalHistoryDetail
                    {
                        ALCaseDetailId = alViewModel.CaseDetail_Data.ALCaseDetailId,
                        ALMedicalHistoryDetailId = pastmedicalHistory.ALMedicalHistoryDetailId,
                        PastDiseaseId = pastmedicalHistory.PastDiseaseId,
                        DurationDays = pastmedicalHistory.DurationDays,
                        DurationFrequencyId = pastmedicalHistory.DurationFrequencyId,
                        Details = pastmedicalHistory.Details,
                        IsDeleted = pastmedicalHistory.IsDeleted,
                        TrackState = pastmedicalHistory.ALMedicalHistoryDetailId == 0 ? TrackState.Added : TrackState.UpdateModified
                    });
                }
            }
            if (alViewModel.CaseDetail_Data.ALMillimanConditions != null)
            {
                foreach (var millimanCondition in alViewModel.CaseDetail_Data.ALMillimanConditions)
                {
                    alCaseDetail.ALMillimanConditions.Add(new ECS.Model.Models.AuthorizationLetter.ALMillimanConditions
                    {
                        ALMillimanConditionId = millimanCondition.ALMillimanConditionId,
                        ConditionId = millimanCondition.ConditionId == 0 ? null : millimanCondition.ConditionId,
                        SeverityId = millimanCondition.SeverityId == 0 ? null : millimanCondition.SeverityId,
                        SignAndSymptomsId = millimanCondition.SignAndSymptomsId == 0 ? null : millimanCondition.SignAndSymptomsId,
                        MaxLOS = millimanCondition.MaxLOS,
                        MinLOS = millimanCondition.MinLOS,
                        IsDeleted = millimanCondition.IsDeleted,
                        IsPrimary = millimanCondition.IsPrimary,
                        IsEnabled = millimanCondition.IsEnabled,
                        TrackState = millimanCondition.ALMillimanConditionId == 0 ? TrackState.Added : TrackState.UpdateModified

                    });
                }
            }
            alCaseDetail.ALICDDiagnosisCodes = new List<ALICDDiagnosisCodes>();
            alCaseDetail.ALICDDiagnosisCodes = alICDDiagnosisCodes;
            alCaseDetail.ALDiseaseProcedureDetails = new List<ALDiseaseProcedureDetails>();
            alCaseDetail.ALDiseaseProcedureDetails = alDiseaseProcedureDetails;
            request.ALCaseDetail = alCaseDetail;
            request.TaskDetailId = alViewModel.TaskDetailId;
            request.TabName = alViewModel.TabName;
            request.ALCompletionPercentageDetails = new ALCompletionPercentageDetails();
            request.ALCompletionPercentageDetails.ALInwardDetailId = alViewModel.PercentageCompletionDetails.ALInwardDetailId;
            request.ALCompletionPercentageDetails.CaseDetailsPercentage = alViewModel.PercentageCompletionDetails.CaseDetailsPercentage;
            request.ALCompletionPercentageDetails.MaternityPercentage = alViewModel.PercentageCompletionDetails.MaternityPercentage;
            request.ALCompletionPercentageDetails.InjuryAccidentPercentage = alViewModel.PercentageCompletionDetails.InjuryAccidentPercentage;
            request.ALCompletionPercentageDetails.PastMedicalHistoryDetailsPercentage = alViewModel.PercentageCompletionDetails.PastMedicalHistoryDetailsPercentage;
            request.ALCompletionPercentageDetails.TrackState = TrackState.UpdateModified;
            if (alCaseDetail.DOA != null)
            {
                request.ALCaseDetail.DOA = Convert.ToDateTime(alCaseDetail.DOA);
            }
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.SaveProviderPortalALCaseDetails(request);
            }
            string caseDetailResponse = JsonConvert.SerializeObject(response.ALCaseDetail); // Serialization
            alViewModel.CaseDetail_Data = serializer.Deserialize<ALCaseDetails>(caseDetailResponse);
            if (response.ALCaseDetail.ALICDDiagnosisCodes != null && response.ALCaseDetail.ALICDDiagnosisCodes.Count > 0)
            {
                alViewModel.CaseDetail_Data.ALICDDiagnosisCodes = new List<RGISLICDMasterViewModel>();
                foreach (var item in response.ALCaseDetail.ALICDDiagnosisCodes.Where(x => x.IsDeleted == false).ToList())
                {
                    alViewModel.CaseDetail_Data.ALICDDiagnosisCodes.Add(new RGISLICDMasterViewModel { Code = item.ICDMaster.ICDCode, Description = item.ICDMaster.Description, ICDMasterId = item.ICDCodeId, IsDeleted = item.IsDeleted, ALICDDiagnosisCodeId = item.ALICDDiagnosisCodeId });
                }
            }
            if (response.ALCaseDetail.ALDiseaseProcedureDetails != null && response.ALCaseDetail.ALDiseaseProcedureDetails.Count > 0)
            {
                alViewModel.CaseDetail_Data.ALDiseaseCodes = new List<RGICLDiseaseProcedureMasterViewModel>();
                foreach (var item in response.ALCaseDetail.ALDiseaseProcedureDetails.Where(x => x.IsDeleted == false).ToList())
                {
                    alViewModel.CaseDetail_Data.ALDiseaseCodes.Add(new RGICLDiseaseProcedureMasterViewModel { Code = item.RGICLDiseaseProcedureMaster.RGICLCode, Description = item.RGICLDiseaseProcedureMaster.DiagnosisName, RGICLDiseaseProcedureID = item.DiseaseProcedureCodeId, IsDeleted = item.IsDeleted, ALDiseaseProcedureDetailId = item.ALDiseaseProcedureDetailId, IsPrimary = item.IsPrimary, Proportion = item.Proportion, NetworkProportion = item.NetworkProportion, Treatment = item.TreatmentType, HospitalCode = item.HospitalCode, HospitalDescription = item.HospitalCodeDescription });
                }
            }
            if (response.ALCaseDetail.ALDiagnosisInvestigations != null && response.ALCaseDetail.ALDiagnosisInvestigations.Count > 0)
            {
                alViewModel.CaseDetail_Data.ALInvestigationType = new List<InvestigationType>();
                foreach (var item in response.ALCaseDetail.ALDiagnosisInvestigations)
                {
                    alViewModel.CaseDetail_Data.ALInvestigationType.Add(new InvestigationType { Code = item.InvestigationTypeMaster.Description, Description = item.InvestigationFindings, MasterId = item.InvestigationTypeId, IsDeleted = item.IsDeleted, ALDiagnosisInvestigationId = item.ALDiagnosisInvestigationId });
                }
            }
            if (response.ALCaseDetail.ALMedicalHistoryDetails != null && response.ALCaseDetail.ALMedicalHistoryDetails.Count > 0)
            {
                alViewModel.CaseDetail_Data.ALMedicalHistoryDetails.Clear();
                foreach (var medicalHistoryDetail in response.ALCaseDetail.ALMedicalHistoryDetails)
                {
                    alViewModel.CaseDetail_Data.ALMedicalHistoryDetails.Add(new PastMedicalHistory
                    {
                        ALMedicalHistoryDetailId = medicalHistoryDetail.ALMedicalHistoryDetailId,
                        PastDiseaseId = medicalHistoryDetail.PastDiseaseId,
                        Details = medicalHistoryDetail.Details,
                        DurationFrequencyId = medicalHistoryDetail.DurationFrequencyId,
                        DurationDays = medicalHistoryDetail.DurationDays,
                        IsDeleted = medicalHistoryDetail.IsDeleted
                    });
                }
            }
            if (response.ALCaseDetail.ALMillimanConditions != null)
            {
                alViewModel.CaseDetail_Data.ALMillimanConditions.Clear();
                foreach (var millimanCondition in response.ALCaseDetail.ALMillimanConditions.Where(x => x.IsDeleted == false).ToList())
                {
                    alViewModel.CaseDetail_Data.ALMillimanConditions.Add(new RGICL.ProviderPortal.Domain.Models.AuthorizationLetter.ALMillimanConditions
                    {
                        ALMillimanConditionId = millimanCondition.ALMillimanConditionId,
                        ConditionId = millimanCondition.ConditionId == null ? 0 : millimanCondition.ConditionId,
                        SeverityId = millimanCondition.SeverityId == null ? 0 : millimanCondition.SeverityId,
                        SignAndSymptomsId = millimanCondition.SignAndSymptomsId == null ? 0 : millimanCondition.SignAndSymptomsId,
                        MinLOS = millimanCondition.MinLOS,
                        MaxLOS = millimanCondition.MaxLOS,
                        IsPrimary = millimanCondition.IsPrimary,
                        IsEnabled = millimanCondition.IsEnabled,
                        IsDeleted = millimanCondition.IsDeleted
                    });
                }
            }

            List<PastMedicalRecord> PastMedicalData = new List<PastMedicalRecord>();
            foreach (var PMRecords in response.ALCaseDetail.ALMedicalHistoryDetails)
            {
                if (PMRecords.IsDeleted == false)
                {

                    PastMedicalData.Add(new PastMedicalRecord
                    {
                        PastMedicalId = PMRecords.ALMedicalHistoryDetailId,
                        PastDiseaseId = PMRecords.PastDiseaseId,
                        DurationFrequencyId = PMRecords.DurationFrequencyId,
                        Code = PMRecords.RGICLDiseaseProcedureMaster.RGICLCode,
                        Desc = PMRecords.RGICLDiseaseProcedureMaster.DiagnosisName,
                        Duration = PMRecords.DurationDays,
                        Frequency = PMRecords.DurationFrequencyId.ToString(),
                        FreeText = PMRecords.Details
                    });
                }
            }

            RGICLDiseaseProcedureMasterViewModel cptMedicalList = new RGICLDiseaseProcedureMasterViewModel();
            cptMedicalList.Code = response.CPTMedicalList.RGICLCode;
            cptMedicalList.Description = response.CPTMedicalList.DiagnosisName;
            cptMedicalList.RGICLDiseaseProcedureID = response.CPTMedicalList.RGICLDiseaseProcedureID;
            alViewModel.CaseDetail_Data.CPTMedicalList = cptMedicalList;
            alViewModel.CaseDetail_Data.PMRecords = PastMedicalData;
            return alViewModel;
        }

        [HttpPost]
        public ALViewModel SaveALCostDetails(ALViewModel alViewModel)
        {
            PreAuthDetailsResponse response = new PreAuthDetailsResponse();
            PreAuthDetailsRequest request = new PreAuthDetailsRequest();

            string costDetail = JsonConvert.SerializeObject(alViewModel.CostDetail_Data);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ALCostDetails alCostDetail = serializer.Deserialize<ALCostDetails>(costDetail);
            alCostDetail.TrackState = TrackState.UpdateModified;
            request.ALCostDetail = alCostDetail;
            request.TaskDetailId = alViewModel.TaskDetailId;
            request.ALCompletionPercentageDetails = new ALCompletionPercentageDetails();
            request.ALCompletionPercentageDetails.ALInwardDetailId = alViewModel.PercentageCompletionDetails.ALInwardDetailId;
            request.ALCompletionPercentageDetails.CostDetailsPercentage = alViewModel.PercentageCompletionDetails.CostDetailsPercentage;
            request.ALCompletionPercentageDetails.TrackState = TrackState.UpdateModified;
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.SaveCostDetails(request);
            }

            List<CostBillRecord> CostBillData = new List<CostBillRecord>();
            foreach (var item in response.ALCostDetails.ALCostBillDetails)
            {
                if (item.IsDeleted == false)
                {
                    if (item.ALCostDetailId == request.ALCostDetail.ALCostDetailId)
                    {
                        string strBillType;
                        int BillID;
                        if (item.BenefitsMaster != null)
                        {
                            if (item.BenefitsMaster.BenefitsMainCoverage == ApplicationConstant.AuthorizationLetter.Maternity_BabyDay)
                            {
                                strBillType = item.BenefitsMaster.BenefitsMainCoverage;
                            }
                            else
                            {
                                strBillType = item.BenefitsMaster.BenefitsSubCoverage;
                            }
                            BillID = item.BenefitsMaster.BenefitsID;
                        }
                        else
                        {
                            strBillType = "";
                            BillID = 0;
                        }
                        string strPackageType = item.BillTypeMaster.BillType;
                        int PackageID = item.BillTypeMaster.BillTypeMasterId;
                        string strSubBillType = item.BillSubTypeMaster.SubBillType;
                        int SubBillID = item.BillSubTypeId.Value;

                        DateTime billdate;
                        String displayBillDate;
                        if (item.BillDate != null)
                        {
                            billdate = DateTime.Parse(item.BillDate.ToString());
                            displayBillDate = billdate.ToString("dd/MMM/yyyy");
                        }
                        else
                        {
                            displayBillDate = "";
                        }
                        CostBillData.Add(new CostBillRecord
                        {
                            ALCostBillID = item.ALCostBillDetailId,
                            MatBillType = strBillType,
                            PackageType = strPackageType,
                            SubBillType = strSubBillType.ToString(),
                            BillNumber = item.BillNo,
                            BillDate = displayBillDate,
                            NoOfUnit = item.NoOfUnit,
                            Amount = item.ClaimAmount.GetValueOrDefault(),
                            MatBillTypeID = BillID,
                            PackageTypeID = PackageID,
                            SubBillTypeID = SubBillID,
                            IsForAllowance = item.BillSubTypeMaster.IsForAllowance
                        });
                    }
                }
            }
            List<CostRoomRecord> CostRoomData = new List<CostRoomRecord>();
            foreach (var item in response.ALCostDetails.ALCostRoomSelectionDetails)
            {

                string strHospitalRoom = item.HospitalRoomName.RoomDescription;
                //string strRoomType = item.RequestedRoomType.RoomTypeDescription;

                DateTime fromdate;
                String displayFromDate;

                DateTime todate;
                String displayToDate;
                if (item.FromDate != null)
                {
                    fromdate = DateTime.Parse(item.FromDate.ToString());
                    displayFromDate = fromdate.ToString("dd/MMM/yyyy");
                }
                else
                {
                    displayFromDate = "";
                }
                if (item.ToDate != null)
                {
                    todate = DateTime.Parse(item.ToDate.ToString());
                    displayToDate = todate.ToString("dd/MMM/yyyy");
                }
                else
                {
                    displayToDate = "";
                }
                CostRoomData.Add(new CostRoomRecord
                {
                    ALCostRoomSelectionDetailId = item.ALCostRoomSelectionDetailId,
                    HospitalRoomNameId = item.HospitalRoomNameId,
                    HospitalRoomName = strHospitalRoom,
                    RequestedRoomTypeId = item.RequestedRoomTypeId,
                    RequestedRoomType = null,
                    RequestedRoomAndRGICLRoomDetailId = item.RequestedRoomAndRGICLRoomDetailId,
                    FromDate = displayFromDate,
                    ToDate = displayToDate,
                    NoOfUnit = item.NoOfUnits
                });
            }

            alViewModel.CostDetail_Data.CostRecords = CostBillData;
            alViewModel.CostDetail_Data.CostRoomRecord = CostRoomData;
            return alViewModel;
        }

        ////This method will be called using Ajax to update all bill as NULL, when the user uncheck the Maternity checkbox.
        ////[HttpPost]
        public ALCaseDetails UpdateMaternityBillAsNULL(ALCaseDetails UpdateID)
        {
            ALCaseDetails resp = new ALCaseDetails();
            PreAuthDetailsRequest req = new PreAuthDetailsRequest();
            bool result = false;
            req.ALDetailId = UpdateID.ALDetailId.Value;
            req.IsMaternity = UpdateID.IsMaternity;

            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                result = proxy.Channel.SaveMaternity(req);
            }

            if (result == true)
                resp.ALDetailId = req.ALDetailId;
            else
                resp.ALDetailId = 0;

            return resp;
        }

        public ALCaseDetails UpdateInjuryDetails(ALCaseDetails UpdateID)
        {
            ALCaseDetails resp = new ALCaseDetails();
            PreAuthDetailsRequest req = new PreAuthDetailsRequest();
            bool result = false;
            req.ALDetailId = UpdateID.ALDetailId.Value;
            req.IsInjury = UpdateID.IsInjury;

            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                result = proxy.Channel.SaveInjury(req);
            }
            if (result == true)
                resp.ALDetailId = req.ALDetailId;
            else
                resp.ALDetailId = 0;
            return resp;
        }

        [HttpPost]
        public ALViewModel SaveALPolicyDetails(ALViewModel alViewModel)
        {
            PreAuthDetailsResponse response = new PreAuthDetailsResponse();
            PreAuthDetailsRequest request = new PreAuthDetailsRequest();
            string policyPatientInsuredDetails = JsonConvert.SerializeObject(alViewModel.PolicyAndPatientDetails); // Serialization
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            request.ALPolicyPatientInsuredDetails = serializer.Deserialize<ALPolicyPatientInsuredDetails>(policyPatientInsuredDetails);
            request.ALPolicyPatientInsuredDetails.TrackState = TrackState.UpdateModified;
            request.TaskDetailId = alViewModel.TaskDetailId;
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.SaveALPolicyPatientInsuredDetails(request);
            }
            return alViewModel;
        }
        [HttpPost]
        public ALViewModel SaveALDNFPolicyDetails(ALViewModel alViewModel)
        {
            PreAuthDetailsResponse response = new PreAuthDetailsResponse();
            PreAuthDetailsRequest request = new PreAuthDetailsRequest();
            string policyPatientInsuredDetails = JsonConvert.SerializeObject(alViewModel.PolicyAndPatientDetails); // Serialization
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            request.ALPolicyPatientInsuredDetails = serializer.Deserialize<ALPolicyPatientInsuredDetails>(policyPatientInsuredDetails);
            request.ALPolicyPatientInsuredDetails.TrackState = TrackState.UpdateModified;
            request.TaskDetailId = alViewModel.TaskDetailId;
            request.ALCompletionPercentageDetails = new ALCompletionPercentageDetails();
            request.ALCompletionPercentageDetails.ALInwardDetailId = alViewModel.PercentageCompletionDetails.ALInwardDetailId;
            request.ALCompletionPercentageDetails.PolicyInsuredDetailsPercentage = alViewModel.PercentageCompletionDetails.PolicyInsuredDetailsPercentage;
            request.ALCompletionPercentageDetails.TrackState = TrackState.UpdateModified;
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.SaveALDNFPolicyPatientInsuredDetails(request);
            }
            return alViewModel;
        }

        //PAN number Auto-Adjudication changes
        public PanRequest IsPanNumberRequired(PanRequest obj)
        {
            PanRequest resp = new PanRequest();
            PanRequest req = new PanRequest();
            bool output = false;
            req.ClaimDetailId = obj.ClaimDetailId;
            req.ClaimedAmount = obj.ClaimedAmount;
            req.SanctionedAmount = obj.SanctionedAmount;
            req.result = obj.result;

            using (var proxy = new ServiceProxy<IAuthorizationLetterService>())
            {
                output = proxy.Channel.CheckIfPANisMandatory("AL", Convert.ToDouble(req.ClaimedAmount), Convert.ToDouble(req.SanctionedAmount), Convert.ToInt32(req.ClaimDetailId));
            }

            if (output == true)
                resp.result = true;
            else
                resp.result = false;

            return resp;
        }

        [HttpPost]
        public ALViewModel SubmitPreAuthInward(ALViewModel alModel)
        {
            PreAuthDetailsRequest request = new PreAuthDetailsRequest();
            PreAuthDetailsResponse response = new PreAuthDetailsResponse();
            request.ALInwardDetails = new ECS.Model.Models.AuthorizationLetter.ALInwardDetails();
            request.ALInwardDetails.ALInwardDetailId = (int)alModel.ALInwardDetailId;
            request.ALInwardDetails.IsDataFound = alModel.IsDataFound;
            request.ALInwardDetails.PatientUHID = alModel.PatientUHID;
            request.ALInwardDetails.PolicyNo = alModel.PolicyNumber;
            request.ALInwardDetails.PolicyClassificationID = alModel.PolicyClassificationId;
            request.ALInwardDetails.DOA = alModel.DOA;
            request.ALInwardDetails.RequestedAmount = alModel.TotalAmount;
            request.ALInwardDetails.InwardClassificationId = alModel.InwardClassificationId;
            request.IsAutoAdjudicated = alModel.IsAutoAdjudicated;
            request.Role = "TPAUser";

            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.SubmitPortalPreAuth(request);
            }

            alModel.IntimationNumber = response.PreAuthIPDDetails.IntimationNo;
            alModel.PatientName = response.PreAuthIPDDetails.PatientName;
            alModel.ParentAL = response.PreAuthIPDDetails.ParentALNo;
            alModel.ALNumber = response.PreAuthIPDDetails.ALNumber;
            //---rsannake---
            if (alModel.ALNumber == "" || alModel.ALNumber == string.Empty || alModel.ALNumber == null)
            {
                if (response.PreAuthIPDDetails.ALInwardNumber != null)
                    alModel.ALNumber = response.PreAuthIPDDetails.ALInwardNumber;
            }
            //---
            return alModel;
        }

        [HttpPost]
        public ALViewModel GetALDetails(ALViewModel alModel)
        {
            PreAuthDetailsRequest request = new PreAuthDetailsRequest();
            request.ALDetailId = (int)alModel.ALDetailId;
            request.ALInwardDetailId = (int)alModel.ALInwardDetailId;

            PreAuthDetailsResponse response = new PreAuthDetailsResponse();
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.GetALDetails(request);
            }

            alModel.ALInwardDetailId = response.PreAuthIPDDetails.ALInwardDetailID;
            alModel.ALDetailId = response.PreAuthIPDDetails.ALDetailID;
            alModel.TaskDetailId = response.PreAuthIPDDetails.TaskDetailID;
            alModel.CaseDetail_Data = new ALCaseDetails();
            alModel.CostDetail_Data = new ALCostDetail();
            alModel.CaseDetail_Data.ALCaseDetailId = response.PreAuthIPDDetails.ALCaseDetailID;
            alModel.CaseDetail_Data.ALDetailId = response.PreAuthIPDDetails.ALDetailID;
            alModel.CostDetail_Data.ALCostDetailId = response.PreAuthIPDDetails.ALCostDetailID;
            alModel.CostDetail_Data.ALDetailId = response.PreAuthIPDDetails.ALDetailID;
            alModel.PolicyNumber = response.PreAuthIPDDetails.PolicyNo;
            alModel.PatientUHID = response.PreAuthIPDDetails.PatientUHID;
            alModel.MemberName = response.PreAuthIPDDetails.PatientName;
            alModel.DOA = response.PreAuthIPDDetails.DOA;
            alModel.TotalAmount = response.PreAuthIPDDetails.TotalAmount;
            alModel.IsDataFound = response.PreAuthIPDDetails.IsDataFound;
            alModel.WizardIndex = response.PreAuthIPDDetails.WizardIndex;
            alModel.MaxFileUploadPerDocumnetType = response.PreAuthIPDDetails.MaxFileUploadPerDocumnetType;
            alModel.ALNumber = response.PreAuthIPDDetails.ALNumber;
            alModel.ProductId = response.PreAuthIPDDetails.ProductId;
            alModel.PolicyClassificationId = response.PreAuthIPDDetails.PolicyClassificationId;
            alModel.InwardClassificationId = response.PreAuthIPDDetails.InwardClassificationId;
            alModel.CaseDetail_Data.IsMaternity = response.PreAuthIPDDetails.IsMaternity;
            alModel.CaseDetail_Data.IsInjuryRTASelfIinjury = response.PreAuthIPDDetails.IsInjury;
            alModel.CaseDetail_Data.IsInjury = response.PreAuthIPDDetails.IsInjury;
            return alModel;
        }

        public ALCaseDetails GetALCaseDetails(int DetailID, int CaseDetailID)
        {
            ALCaseDetails ALCaseDetailsVM = new ALCaseDetails();

            GetALCaseDetailRequest ALCaseDetailsRequest = new GetALCaseDetailRequest();
            ALCaseDetailsRequest.alDetailId = DetailID;
            ALCaseDetailsRequest.alCaseDetailId = CaseDetailID;
            ALCaseDetailResponse ALCaseDetailsResponse = new ALCaseDetailResponse();
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                ALCaseDetailsResponse = proxy.Channel.GetALCaseDetail(ALCaseDetailsRequest);
            }
            ALCaseDetailsVM.ObstetricAVal = ALCaseDetailsResponse.ALCaseDetail.ObstetricAVal;
            ALCaseDetailsVM.ObstetricGVal = ALCaseDetailsResponse.ALCaseDetail.ObstetricGVal;
            ALCaseDetailsVM.ObstetricLVal = ALCaseDetailsResponse.ALCaseDetail.ObstetricLVal;
            ALCaseDetailsVM.ObstetricPVal = ALCaseDetailsResponse.ALCaseDetail.ObstetricPVal;
            ALCaseDetailsVM.LMPDate = DateTime.Parse(ALCaseDetailsResponse.ALCaseDetail.LMPDate.ToString(), System.Globalization.CultureInfo.InvariantCulture);
            ALCaseDetailsVM.EDD = DateTime.Parse(ALCaseDetailsResponse.ALCaseDetail.EDD.ToString(), System.Globalization.CultureInfo.InvariantCulture);
            return ALCaseDetailsVM;
        }

        public PolicyAndPatientDetails GetPatientInsuredDetails(int ALDetailID)
        {
            ALDetailRequest ALDetailsRequest = new ALDetailRequest();
            ALDetailsRequest.ALDetailID = ALDetailID;

            PreAuthDetailsResponse alDetailsResponse = new PreAuthDetailsResponse();
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                alDetailsResponse = proxy.Channel.GetPolicyPatientInsuredDetails(ALDetailsRequest);
            }
            ALPolicyPatientInsuredDetails alPolicyPatientInsuredDetails = alDetailsResponse.ALPolicyPatientInsuredDetails;
            string policyPatientInsuredDetails = JsonConvert.SerializeObject(alPolicyPatientInsuredDetails); // Serialization
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            PolicyAndPatientDetails ppDetails = serializer.Deserialize<PolicyAndPatientDetails>(policyPatientInsuredDetails);

            if (ppDetails != null)
            {
                ppDetails.AddressLine = string.Format("{0} {1} {2} {3} {4}.", ppDetails.AddressLine1 != null ? ppDetails.AddressLine1 : "", ppDetails.AddressLine2 != null ? "," + ppDetails.AddressLine2 : "", ppDetails.AddressLine3 != null ? "," + ppDetails.AddressLine3 : "", ppDetails.City != null ? "," + ppDetails.City : "", ppDetails.Pincode != null ? "-" + Convert.ToInt32(ppDetails.Pincode) : "");
                ppDetails.DisplayDOB = ppDetails.DOB != null ? ppDetails.DOB.Value.FriendlyDateTime() : null;
                if (ppDetails.AdditionalMobileNo != null && ppDetails.AdditionalMobileNo.Contains('-'))
                {
                    ppDetails.AdditionalMobileNo = ppDetails.AdditionalMobileNo.Split('-')[1];
                }
                if (ppDetails.MobileNo != null && ppDetails.MobileNo.Contains('-'))
                {
                    ppDetails.MobileNo = ppDetails.MobileNo.Split('-')[1];
                }
                if (ppDetails.AttendantMobileNo != null && ppDetails.AttendantMobileNo.Contains('-'))
                {
                    ppDetails.AttendantMobileNo = ppDetails.AttendantMobileNo.Split('-')[1];
                }
            }

            return ppDetails;
        }

        [HttpPost]
        public List<PhysicianDetails> SavePhysicianDetails(List<PhysicianDetails> alPhysicianDetails)
        {
            PreAuthDetailsResponse response = new PreAuthDetailsResponse();
            PreAuthDetailsRequest request = new PreAuthDetailsRequest();
            string physicianDetails = JsonConvert.SerializeObject(alPhysicianDetails.First());
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            request.ALPhysicianDetails = serializer.Deserialize<ALPhysicianDetails>(physicianDetails);
            request.TaskDetailId = alPhysicianDetails.First().TaskStateId;
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.SavePhysicianDetails(request);
            }
            physicianDetails = JsonConvert.SerializeObject(response.ALPhysicianDetails);
            serializer = new JavaScriptSerializer();
            List<PhysicianDetails> alPhysicianDetailsList = serializer.Deserialize<List<PhysicianDetails>>(physicianDetails);
            return alPhysicianDetailsList;
        }

        public List<PhysicianDetails> GetPhysicianDetails(int ALDetailId)
        {
            PreAuthDetailsResponse response = new PreAuthDetailsResponse();
            PreAuthDetailsRequest request = new PreAuthDetailsRequest();
            request.ALDetail = new ALDetail();
            request.ALDetail.ALDetailId = ALDetailId;
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.GetALPhysicianDetails(request);
            }
            //string physicianDetails = JsonConvert.SerializeObject(response.ALPhysicianDetails);
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //alPhysicianDetails = serializer.Deserialize<List<PhysicianDetails>>(physicianDetails);
            List<PhysicianDetails> alPhysicianDetails = new List<PhysicianDetails>();
            if (response.ALPhysicianDetails != null)
            {
                foreach (var item in response.ALPhysicianDetails)
                {
                    PhysicianDetails physicianDetails = new PhysicianDetails();
                    physicianDetails.PhysicianName = item.PhysicianName;
                    physicianDetails.RegistrationNo = item.RegistrationNo;
                    physicianDetails.QualificationId = item.QualificationId;
                    physicianDetails.SpecializationId = item.SpecializationId;
                    physicianDetails.MobileNo = item.MobileNo;
                    physicianDetails.ALDetailId = item.ALDetailId;
                    physicianDetails.ALPhysicianDetailId = item.ALPhysicianDetailId;
                    physicianDetails.IsDeleted = item.IsDeleted;
                    physicianDetails.QualificationMaster = new Qualification();
                    physicianDetails.SpecializationMaster = new SpecialityMaster();
                    if (item.QualificationMaster != null)
                    {
                        physicianDetails.QualificationMaster.Description = item.QualificationMaster.Description;
                        physicianDetails.QualificationMaster.QualificationId = item.QualificationMaster.QualificationId;
                    }
                    physicianDetails.CodeMaster = new RGICL.ProviderPortal.Domain.Models.Master.CodeMaster();
                    if (item.CodeMaster != null)
                    {
                        physicianDetails.CodeMaster.Description = item.CodeMaster.Description;
                        physicianDetails.CodeMaster.CodeMasterId = item.CodeMaster.CodeMasterId;
                    }
                    if (item.SpecializationMaster != null)
                    {
                        physicianDetails.SpecializationMaster.Description = item.SpecializationMaster.Description;
                        physicianDetails.SpecializationMaster.SpecializationId = item.SpecializationMaster.SpecializationId;
                    }
                    alPhysicianDetails.Add(physicianDetails);
                }
            }
            return alPhysicianDetails;
        }

        public PercentageCompletionDetails GetALCompletionPercentage(int alInwardDetailId)
        {
            PreAuthDetailsResponse response = new PreAuthDetailsResponse();
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.GetALCompletionPercentage(alInwardDetailId);
            }
            PercentageCompletionDetails PercentageDetails = new PercentageCompletionDetails();
            if (response.ALCompletionPercentageDetails != null)
            {
                string ALPercentageDetails = JsonConvert.SerializeObject(response.ALCompletionPercentageDetails);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                PercentageDetails = serializer.Deserialize<PercentageCompletionDetails>(ALPercentageDetails);
            }
            return PercentageDetails;
        }

        public List<QueryDetails> GetQueryCaseDetails(int alDetailId)
        {
            ALQueryDetailsRequest request = new ALQueryDetailsRequest();
            ALQueryDetailsResponse response = new ALQueryDetailsResponse();
            request.ALDetailId = alDetailId;
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.GetQueryCaseDetails(request);
            }

            List<QueryDetails> queryDetails = new List<QueryDetails>();
            List<ALQueryDetails> queryDetailsList = new List<ALQueryDetails>();
            if (response.ALQueryDetailsList != null)
            {
                queryDetailsList = response.ALQueryDetailsList.Where(x => x.QueryTypeId == Convert.ToInt32(QueryType.External)).ToList();
            }

            if (queryDetailsList != null)
            {
                foreach (var item in queryDetailsList)
                {
                    QueryDetails queryDetail = new QueryDetails();
                    queryDetail.ALqueryDetailId = item.ALqueryDetailId;
                    queryDetail.CodeMaster = new RGICL.ProviderPortal.Domain.Models.Master.CodeMaster();
                    queryDetail.CodeMaster.Description = item.CodeMaster.Description;
                    queryDetail.Remarks = item.Remarks;
                    queryDetail.QueryReasonMapping = new ExternalQueryReasonMapping();
                    queryDetail.QueryReasonMapping.QueryReasonMaster = new QueryRMaster();
                    queryDetail.QueryReasonMapping.QueryReasonMaster.Reason = item.QueryReasonMapping.QueryReasonMaster.Reason;
                    queryDetail.QueryReasonMapping.QueryReasonTypeMaster = new QueryRTypeMaster();
                    queryDetail.QueryReasonMapping.QueryReasonTypeMaster.ReasonType = item.QueryReasonMapping.QueryReasonTypeMaster.ReasonType;
                    queryDetails.Add(queryDetail);
                }
            }
            return queryDetails;
        }

        public List<RejectDetails> GetRejectCaseDetails(int alDetailId)
        {
            ALRejectDetailsRequest request = new ALRejectDetailsRequest();
            ALRejectDetailsResponse response = new ALRejectDetailsResponse();
            request.ALDetailId = alDetailId;
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.GetRejectDetails(request);
            }
            List<RejectDetails> rejectDetails = new List<RejectDetails>();
            if (response.ALRejectDetailsList != null)
            {
                string ALRejectDetails = JsonConvert.SerializeObject(response.ALRejectDetailsList);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                rejectDetails = serializer.Deserialize<List<RejectDetails>>(ALRejectDetails);
            }
            return rejectDetails;
        }

        #region "File Upload"


        #region Public method
        [HttpPost]
        public DocumentDetailModel SaveDocumentDetail(DocumentDetailModel documentDetail)
        {
            DocumentDetail doc = new DocumentDetail();
            DocumentDetailRequest request = new DocumentDetailRequest() { DocumentDetail = ConvertDocumentViewModelToDocument(documentDetail, TrackState.Added) };
            ALCompletionPercentageDetails requestPercentage = ConvertDocumentViewModelToALCompletionPercentageDetails(documentDetail.ALPercentageCompletionDetails);
            PreAuthDetailsResponse response = new PreAuthDetailsResponse();
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.SaveALDocumentDetails(request, requestPercentage);
            }
            DocumentDetailModel documentDetailModel = ConvertDocumentRespTotViewModel(response.ALDocumentDetailResponse);
            return documentDetailModel;
        }

        private ALCompletionPercentageDetails ConvertDocumentViewModelToALCompletionPercentageDetails(PercentageCompletionDetails percentageCompletionDetails)
        {
            ALCompletionPercentageDetails aLCompletionPercentageDetails = new ALCompletionPercentageDetails()
            {
                ALInwardDetailId = percentageCompletionDetails.ALInwardDetailId,
                UploadPercentage = percentageCompletionDetails.UploadPercentage
            };
            return aLCompletionPercentageDetails;
        }

        [HttpGet]
        public List<DocumentDetailModel> GetDocumentDetails(int transactionid, string transactiontype, string documentType)
        {
            ECS.Service.Contract.Response.ClaimsCommon.DocumentDetailsResponse response = new ECS.Service.Contract.Response.ClaimsCommon.DocumentDetailsResponse();
            using (var proxy = new ServiceProxy<IClaimsCommonService>())
            {
                ECS.Service.Contract.Request.ClaimsCommon.DocumentDetailsRequest request = new ECS.Service.Contract.Request.ClaimsCommon.DocumentDetailsRequest();
                request.transactionid = Convert.ToInt32(transactionid);
                request.transactiontype = transactiontype;
                request.DocumentType = documentType;
                response = proxy.Channel.GetDocumentDetails(request);
                return ConvertDocumentRespTotViewModelList(response);
            }
        }

        public DocumentDetail DocumentDelete(DocumentDetailModel documentDetail)
        {
            List<DocumentDetail> lstDocumentDetail = new List<DocumentDetail>();
            lstDocumentDetail.Add(ConvertDocumentViewModelToDocument(documentDetail, TrackState.UpdateModified));
            DeleteDocumentDetailsRequest request = new DeleteDocumentDetailsRequest() { DocumentDetails = lstDocumentDetail };
            DocumentDetailResponse response = new DocumentDetailResponse();
            using (var proxy = new ServiceProxy<IClaimsCommonService>())
            {
                proxy.Channel.DeleteDocumentDetailsPortal(request);
            }
            return response.DocumentDetail;
        }
        #endregion File Upload Public method

        #region private method
        private DocumentDetail ConvertDocumentViewModelToDocument(DocumentDetailModel documentDetailViewModel, TrackState state)
        {
            DocumentDetail documentDetail = new DocumentDetail()
            {
                DocumentDetailId = documentDetailViewModel.DocumentDetailId,
                DocumentTypeId = documentDetailViewModel.DocumentTypeId,
                IsOriginalCopy = documentDetailViewModel.IsOriginalCopy,
                FileName = documentDetailViewModel.FileName,
                Filepath = documentDetailViewModel.Filepath,
                SavedFileName = documentDetailViewModel.SavedFileName,
                TransactionId = documentDetailViewModel.TransactionId,
                TransactionType = documentDetailViewModel.TransactionType,
                IsDeleted = documentDetailViewModel.IsDeleted,
                TrackState = state,
            };
            return documentDetail;
        }

        private List<DocumentDetailModel> ConvertDocumentRespTotViewModelList(ECS.Service.Contract.Response.ClaimsCommon.DocumentDetailsResponse documentDetailViewModel)
        {
            List<DocumentDetailModel> responseViewModel = new List<DocumentDetailModel>();
            if (documentDetailViewModel != null)
            {
                if (documentDetailViewModel.DocumentDetails != null)
                {
                    foreach (DocumentDetail item in documentDetailViewModel.DocumentDetails)
                    {
                        DocumentDetailModel model = new DocumentDetailModel()
                        {
                            DocumentDetailId = item.DocumentDetailId,
                            TransactionId = item.TransactionId,
                            TransactionType = item.TransactionType,
                            DocumentTypeId = item.DocumentTypeId,
                            DocumentumId = item.DocumentumId,
                            Filepath = item.Filepath,
                            SavedFileName = item.SavedFileName,
                            FileName = item.FileName,
                            IsOriginalCopy = item.IsOriginalCopy,
                        };
                        responseViewModel.Add(model);
                    }
                }
            }
            return responseViewModel;
        }

        private DocumentDetailModel ConvertDocumentRespTotViewModel(DocumentDetail documentDetailViewModel)
        {
            DocumentDetail item = documentDetailViewModel;
            DocumentDetailModel model = new DocumentDetailModel()
            {
                DocumentDetailId = item.DocumentDetailId,
                TransactionId = item.TransactionId,
                TransactionType = item.TransactionType,
                DocumentTypeId = item.DocumentTypeId,
                DocumentumId = item.DocumentumId,
                Filepath = item.Filepath,
                SavedFileName = item.SavedFileName,
                FileName = item.FileName,
                IsOriginalCopy = item.IsOriginalCopy,
            };
            return model;
        }

        #endregion File Upload private method

        #endregion "File Upload End here"

        public List<string> GetALDeviations(int alDetailId, int alInwardDetailId, string patientUHID, string policyNumber, Nullable<DateTime> DOA,
                                                  int? productId, string productPlanType, int alCostDetailId) //// remove alcostdetailid param which is not used
        {
            PreAuthDetailsRequest request = new PreAuthDetailsRequest();
            PreAuthDetailsResponse response = new PreAuthDetailsResponse();
            ALDetail alDetail = new ALDetail();
            alDetail.ALDetailId = alDetailId;
            alDetail.PatientUHID = patientUHID;
            alDetail.PolicyNumber = policyNumber;
            alDetail.ProductId = productId;
            alDetail.ProductPlanType = productPlanType;
            alDetail.DOA = DOA;
            alDetail.ALInwardDetailId = alInwardDetailId;
            request.ALDetail = alDetail;

            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.GetPreAuthDeviations(request);
            }

            List<string> deviations = new List<string>();
            foreach (var item in response.Deviations)
            {
                item.Value.ForEach(x => deviations.Add(x));
            }
            return deviations;
        }



        public PortalClaimSubmitDetailsViewModel PortalPreAuthSubmit(PortalClaimSubmitDetailsViewModel claimSubmitDetailsViewModel)
        {
            PreAuthDetailsRequest request = new PreAuthDetailsRequest();
            PortalClaimSubmitDetailsResponse response = new PortalClaimSubmitDetailsResponse();
            ALDetail alDetail = new ALDetail();
            alDetail.ALDetailId = claimSubmitDetailsViewModel.ALDetailId;
            alDetail.PatientUHID = claimSubmitDetailsViewModel.PatientUHID;
            alDetail.PolicyNumber = claimSubmitDetailsViewModel.PolicyNumber;
            alDetail.ProductId = claimSubmitDetailsViewModel.ProductId;
            alDetail.ProductPlanType = claimSubmitDetailsViewModel.ProductPlanType;
            alDetail.DOA = claimSubmitDetailsViewModel.DOA;
            alDetail.ALInwardDetailId = claimSubmitDetailsViewModel.ALInwardDetailId;
            request.ALDetail = alDetail;

            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.PortalPreAuthSubmit(request);
            }

            PortalClaimSubmitDetailsViewModel portalClaimSubmitDetailsViewModel = new Domain.Models.AuthorizationLetter.PortalClaimSubmitDetailsViewModel();
            List<ClaimCPTDetailsViewModel> claimCPTDetailsViewModel = new List<ClaimCPTDetailsViewModel>();
            ClaimCPTDetailsViewModel claimCPTDetail = null;
            if (response != null)
            {
                if (response.portalClaimSubmitDetails != null)
                {
                    if (response.portalClaimSubmitDetails.ClaimCPTDetails != null)
                    {
                        foreach (var cptdetails in response.portalClaimSubmitDetails.ClaimCPTDetails)
                        {
                            claimCPTDetail = new ClaimCPTDetailsViewModel();
                            claimCPTDetail.Diagnosis = cptdetails.Diagnosis;
                            claimCPTDetail.RequestedAmount = cptdetails.RequestedAmount;
                            claimCPTDetail.CompulsoryDeductible = cptdetails.CompulsoryDeductible;
                            claimCPTDetail.NonPayableAmount = cptdetails.NonPayableAmount;
                            claimCPTDetail.Copay = cptdetails.Copay;
                            claimCPTDetail.Discount = cptdetails.Discount;
                            claimCPTDetail.NetSanctionedAmount = cptdetails.NetSanctionedAmount;
                            claimCPTDetailsViewModel.Add(claimCPTDetail);
                        }
                    }
                }
            }
            portalClaimSubmitDetailsViewModel.ClaimCPTDetails = claimCPTDetailsViewModel;
            portalClaimSubmitDetailsViewModel.Deviations = response.portalClaimSubmitDetails.Deviations;
            portalClaimSubmitDetailsViewModel.IsAutoApproved = response.portalClaimSubmitDetails.IsAutoApproved;
            portalClaimSubmitDetailsViewModel.IsPremiumBenefitApplied = response.portalClaimSubmitDetails.IsPremiumBenefitApplied;
            portalClaimSubmitDetailsViewModel.NetNsaAmount = response.portalClaimSubmitDetails.NetNsaAmount;
            return portalClaimSubmitDetailsViewModel;
        }

        [HttpGet]
        public List<PolicyAndPatientDetails> GetPolicyDetails(string UHID, string PolicyNo, string InsuredName, string PolicyType)
        {
            PreAuthDetailsRequest request = new PreAuthDetailsRequest();
            PreAuthDetailsResponse response = new PreAuthDetailsResponse();
            ALDNFSearchMemberParameter parm = new ALDNFSearchMemberParameter();

            parm.PageIndex = 0;
            parm.PageSize = 10000;
            parm.PolicyNo = PolicyNo;
            parm.UHID = UHID;
            parm.InsuredName = InsuredName;
            parm.PolicyClassificationType = PolicyType;
            request.ALDNFSearchMemberParameters = parm;
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.GetSearchMemberDetails(request);
            }

            Tuple<List<ALDNFSearchMemberResult>, int> policyMaster = response.ALDNFSearchMemberResults;
            List<PolicyAndPatientDetails> PPDetails = new List<PolicyAndPatientDetails>();
            foreach (var item in policyMaster.Item1)
            {
                if (item.UHIDSearchDetail == null)
                {
                    PPDetails.Add(new PolicyAndPatientDetails { PolicyNumber = item.PolicyNo, PatientUHID = item.UHID, MobileNo = item.RetailMobile, AddressLine1 = item.RetailAddress1, AddressLine2 = item.RetailAddress2, Age = item.Age, InsuredName = item.InsuredName, PolicyClassificationId = 1, ProductName = item.PolicyName, EmployeeId = item.MemberIdEmpId, OrganizationName = item.OrganizationName, DOB = item.DOB, State = item.RetailState, District = item.RetailDistrict, City = item.RetailCity, PatientName = item.InsuredName, Pincode = Convert.ToInt32(item.RetailPin), ProductPlanType = item.RetailPlanType });
                }
                else
                {
                    PPDetails.Add(new PolicyAndPatientDetails { PolicyNumber = item.UHIDSearchDetail.PolicyNo, PatientUHID = item.UHIDSearchDetail.UHID, MobileNo = item.UHIDSearchDetail.MobileNo, AddressLine1 = item.UHIDSearchDetail.Address1, AddressLine2 = item.UHIDSearchDetail.Address2, Age = item.UHIDSearchDetail.Age, InsuredName = item.UHIDSearchDetail.MemberName, PolicyClassificationId = 1, ProductName = item.UHIDSearchDetail.ProductName, EmployeeId = item.UHIDSearchDetail.EmployeeId, OrganizationName = item.UHIDSearchDetail.PolicyName, DOB = item.UHIDSearchDetail.DOB, State = item.UHIDSearchDetail.State, District = item.UHIDSearchDetail.District, City = item.UHIDSearchDetail.city, PatientName = item.UHIDSearchDetail.MemberName, Pincode = item.UHIDSearchDetail.Pincode, ProductPlanType = "" });
                }
            }
            return PPDetails;
        }

        [HttpGet]
        public ALReAdmissionErrorCheckingLogic GetALDetailsForErrorCheckingLogic(string UHID, string PolicyNo, int PolicyClassificationID, int ProviderBasicDetailId, string DOA, int ALDetailId)
        {
            PreAuthDetailsResponse response = new PreAuthDetailsResponse();
            ALErrorCheckingLogicRequest request = new ALErrorCheckingLogicRequest();
            request.DateOfAdmission = Convert.ToDateTime(DOA);
            request.UHID = UHID;
            request.PolicyNo = PolicyNo;
            request.PolicyClassificationID = PolicyClassificationID;
            request.ProviderBasicDetailId = ProviderBasicDetailId;
            request.ALDetailId = ALDetailId;
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.GetALDetailsForErrorCheckingLogic(request);
            }
            ALReAdmissionErrorCheckingLogic ALErrorCheckingReAdmissionResults = new ALReAdmissionErrorCheckingLogic();
            if (response.ReAdmissionErrorcheckingResults != null)
            {
                string ECSALErrorCheckingResults = JsonConvert.SerializeObject(response.ReAdmissionErrorcheckingResults);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ALErrorCheckingReAdmissionResults = serializer.Deserialize<ALReAdmissionErrorCheckingLogic>(ECSALErrorCheckingResults);
            }
            return ALErrorCheckingReAdmissionResults;

        }
        [HttpGet]
        public string CancelALRequest(int InwardDetailId)
        {
            string response = string.Empty;
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.DeleteALDetails(InwardDetailId);
            }
            return response;
        }

        [HttpGet]
        public List<MillimanConditionsViewModel> GetConditionMasterForICD(string iCDIdList)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<int> icdIds = serializer.Deserialize<List<int>>(iCDIdList);
            MillimanICDRequest request = new MillimanICDRequest() { ICDIdList = icdIds };
            MillimanConditionResponse response = new MillimanConditionResponse();
            List<MillimanConditionsViewModel> millimanConditionsViewModel = new List<MillimanConditionsViewModel>();
            using (var proxy = new ServiceProxy<IAuthorizationLetterService>())
            {
                response = proxy.Channel.GetConditionMasterForICD(request);
            }
            if (response != null)
            {
                if (response.MillimanConditionList != null)
                {
                    List<ECS.Model.CustomEntity.ClaimsCommon.MillimanConditions> millimanConditions = response.MillimanConditionList;
                    foreach (ECS.Model.CustomEntity.ClaimsCommon.MillimanConditions mConditions in millimanConditions)
                    {
                        MillimanConditionsViewModel mConditionViewModel = new MillimanConditionsViewModel()
                        {
                            ConditionId = mConditions.ConditionId,
                            ConditionDescription = mConditions.ConditionDescription,
                            MinAge = mConditions.MinAge,
                            MaxAge = mConditions.MaxAge,
                            Gender = mConditions.Gender,
                            IsEnabled = mConditions.IsEnabled,
                            DiseaseProcedureId = mConditions.DiseaseProcedureId,
                            ConditionSeveritySignAndSymptomsMappingViewModel = GetConditionSeveritySignAndSymptomsMappingViewModel(mConditions.ConditionSeveritySignAndSymptomsMapping)
                        };
                        millimanConditionsViewModel.Add(mConditionViewModel);
                    }
                }
            }
            return millimanConditionsViewModel;
        }
        [HttpGet]
        public List<MillimanConditionsViewModel> GetConditionMasterForCPT(string cPTIdList)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<int> cptids = serializer.Deserialize<List<int>>(cPTIdList);
            MillimanCPTRequest request = new MillimanCPTRequest() { CPTIdList = cptids };
            MillimanConditionResponse response = new MillimanConditionResponse();
            List<MillimanConditionsViewModel> millimanConditionsViewModel = new List<MillimanConditionsViewModel>();
            using (var proxy = new ServiceProxy<IAuthorizationLetterService>())
            {
                response = proxy.Channel.GetConditionMasterForCPT(request);
            }
            if (response != null)
            {
                if (response.MillimanConditionList != null)
                {
                    List<ECS.Model.CustomEntity.ClaimsCommon.MillimanConditions> millimanConditions = response.MillimanConditionList;
                    foreach (ECS.Model.CustomEntity.ClaimsCommon.MillimanConditions mConditions in millimanConditions)
                    {
                        MillimanConditionsViewModel mConditionViewModel = new MillimanConditionsViewModel()
                        {
                            ConditionId = mConditions.ConditionId,
                            ConditionDescription = mConditions.ConditionDescription,
                            MinAge = mConditions.MinAge,
                            MaxAge = mConditions.MaxAge,
                            Gender = mConditions.Gender,
                            IsEnabled = mConditions.IsEnabled,
                            DiseaseProcedureId = mConditions.DiseaseProcedureId,
                            ConditionSeveritySignAndSymptomsMappingViewModel = GetConditionSeveritySignAndSymptomsMappingViewModel(mConditions.ConditionSeveritySignAndSymptomsMapping)
                        };
                        millimanConditionsViewModel.Add(mConditionViewModel);
                    }
                }
            }
            return millimanConditionsViewModel;
        }
        private List<ConditionSeveritySignAndSymptomsMappingViewModel> GetConditionSeveritySignAndSymptomsMappingViewModel(List<ECS.Model.Models.ClaimCommon.ConditionSeveritySignAndSymptomsMapping> conditionSeveritySignAndSymptomsMapping)
        {
            List<ConditionSeveritySignAndSymptomsMappingViewModel> conditionSeveritySignAndSymptomsMappingViewModel = new List<ConditionSeveritySignAndSymptomsMappingViewModel>();

            foreach (ECS.Model.Models.ClaimCommon.ConditionSeveritySignAndSymptomsMapping iconditionSeveritySignAndSymptomsMapping in conditionSeveritySignAndSymptomsMapping)
            {
                ConditionSeveritySignAndSymptomsMappingViewModel ConditionViewModel = new ConditionSeveritySignAndSymptomsMappingViewModel()
                    {
                        SeverityId = iconditionSeveritySignAndSymptomsMapping.SeverityId,
                        SignandSymptomsId = iconditionSeveritySignAndSymptomsMapping.SignAndSymptomsId,
                        MaxLOS = iconditionSeveritySignAndSymptomsMapping.MaximumLOS,
                        MinLOS = iconditionSeveritySignAndSymptomsMapping.MinimumLOS
                    };
                conditionSeveritySignAndSymptomsMappingViewModel.Add(ConditionViewModel);
            }
            return conditionSeveritySignAndSymptomsMappingViewModel;
        }


        public ALCostDetail SaveCostRoomDetails(ALCostDetail addRoomDetails)
        {
            ////Request,Response to save Bill detail.
            PreAuthDetailsRequest req = new PreAuthDetailsRequest();
            PreAuthDetailsResponse resp = new PreAuthDetailsResponse();
            ALCostRoomSelectionDetails ALCost = new ALCostRoomSelectionDetails();

            ////Request,Response to fetch the Bill details
            ALCostDetailRequest reqALCostDetail = new ALCostDetailRequest();
            ALCostDetailResponse respALCostDetail = new ALCostDetailResponse();
            reqALCostDetail.ALDetailId = addRoomDetails.ALDetailId;
            reqALCostDetail.ALCostDetailId = addRoomDetails.ALCostDetailId;
            reqALCostDetail.ProviderBasicDetailId = addRoomDetails.ProviderBasicDetailId;

            req.ALCostRoomSelectionDetails = ALCost;
            req.ALCostDetail = new ALCostDetails(); //need to check if we get existing value
            req.ALCostDetail.ALDetailId = addRoomDetails.ALDetailId;
            req.ALCostDetail.ALCostDetailId = addRoomDetails.ALCostDetailId;
            req.TaskDetailId = addRoomDetails.TaskDetailID; //need to remove once integrated
            string CostRoom = JsonConvert.SerializeObject(addRoomDetails.ALCostRoomHistoryDetails);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<CostRoomHistory> alCostRoomDetail = serializer.Deserialize<List<CostRoomHistory>>(CostRoom);
            foreach (var item in alCostRoomDetail)
            {
                req.ALCostRoomSelectionDetails.ALCostRoomSelectionDetailId = item.ALCostRoomSelectionDetailId;
                req.ALCostRoomSelectionDetails.HospitalRoomNameId = item.HospitalRoomNameId;
                req.ALCostRoomSelectionDetails.RequestedRoomTypeId = null;//item.RequestedRoomTypeId;
                req.ALCostRoomSelectionDetails.FromDate = item.FromDate;
                req.ALCostRoomSelectionDetails.ToDate = item.ToDate;
                req.ALCostRoomSelectionDetails.NoOfUnits = item.NoOfUnit;
                req.ALCostRoomSelectionDetails.RequestedRoomAndRGICLRoomDetailId = item.RequestedRoomAndRGICLRoomDetailId;
                req.ALCostRoomSelectionDetails.IsDeleted = item.IsDeleted;
                req.ALCostRoomSelectionDetails.SanctionedRoomNameId = null;//item.HospitalRoomNameId;
                req.ALCostRoomSelectionDetails.SanctionedRoomTypeId = null;//item.RequestedRoomTypeId;
                req.ALCostRoomSelectionDetails.SanctionedRoomAndRGICLRoomDetailId = item.RequestedRoomAndRGICLRoomDetailId;
            }
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                resp = proxy.Channel.SaveCostRoomDetails(req);
                respALCostDetail = proxy.Channel.GetALCostDetail(reqALCostDetail);

            }

            ////Do Mapping to ALCostDetail from the response ALCostDetails
            List<CostRoomHistory> lstRoomHistory = new List<CostRoomHistory>();
            foreach (var item in resp.ALCostDetails.ALCostRoomSelectionDetails)
            {
                lstRoomHistory.Add(new CostRoomHistory
                {
                    ALCostRoomSelectionDetailId = item.ALCostRoomSelectionDetailId,
                    HospitalRoomNameId = item.HospitalRoomNameId,
                    RequestedRoomTypeId = item.RequestedRoomTypeId,
                    FromDate = item.FromDate,
                    ToDate = item.ToDate,
                    RequestedRoomAndRGICLRoomDetailId = item.SanctionedRoomAndRGICLRoomDetailId,
                    NoOfUnit = item.NoOfUnits,
                    IsDeleted = item.IsDeleted
                });
            }

            List<CostRoomRecord> CostRoomData = new List<CostRoomRecord>();
            foreach (var item in respALCostDetail.ALCostDetails.ALCostRoomSelectionDetails.Where(x => x.IsDeleted == false).ToList())
            {

                string strHospitalRoom = item.HospitalRoomName.RoomDescription;
                //string strRoomType = item.RequestedRoomType.RoomTypeDescription;

                DateTime fromdate;
                String displayFromDate;

                DateTime todate;
                String displayToDate;
                if (item.FromDate != null)
                {
                    fromdate = DateTime.Parse(item.FromDate.ToString());
                    displayFromDate = fromdate.ToString("dd/MMM/yyyy");
                }
                else
                {
                    displayFromDate = "";
                }
                if (item.ToDate != null)
                {
                    todate = DateTime.Parse(item.ToDate.ToString());
                    displayToDate = todate.ToString("dd/MMM/yyyy");
                }
                else
                {
                    displayToDate = "";
                }
                CostRoomData.Add(new CostRoomRecord
                {
                    ALCostRoomSelectionDetailId = item.ALCostRoomSelectionDetailId,
                    HospitalRoomNameId = item.HospitalRoomNameId,
                    HospitalRoomName = strHospitalRoom,
                    RequestedRoomTypeId = item.RequestedRoomTypeId,
                    RequestedRoomType = null,
                    RequestedRoomAndRGICLRoomDetailId = item.RequestedRoomAndRGICLRoomDetailId,
                    FromDate = displayFromDate,
                    ToDate = displayToDate,
                    NoOfUnit = item.NoOfUnits
                });
            }

            ALCostDetail respCostRoom = new ALCostDetail()
            {
                ALCostDetailId = req.ALCostDetail.ALCostDetailId,
                ALCostRoomHistoryDetails = lstRoomHistory,
                CostRoomRecord = CostRoomData
            };
            return respCostRoom;
        }

        [HttpGet]
        public ClaimAcknowledgmentDetailsViewModel AcknowledgementRequestForClaim(string transactionId, string transactiontype, string recepientEmail, string eventType)
        {
            ProviderPortalCommonResponseServices ipdResponse = new ProviderPortalCommonResponseServices();
            ProviderPortalIPDCommonRequest ipdRequest = new ProviderPortalIPDCommonRequest();
            ClaimAcknowledgmentDetailsViewModel claimAcknowledgmentDetailsViewModel = new ClaimAcknowledgmentDetailsViewModel();

            ipdRequest.TransactionId = Convert.ToInt32(transactionId);
            ipdRequest.TransactionType = transactiontype;
            ipdRequest.RecipientEmail = recepientEmail;
            ipdRequest.EventType = eventType;

            using (var proxy = new ServiceProxy<IProviderPortalCommonService>())
            {
                ipdResponse = proxy.Channel.AcknowledgementRequestForClaim(ipdRequest);
            }

            claimAcknowledgmentDetailsViewModel.SavedPdfFileName = ipdResponse.ClaimAcknowledgmentDetails.SavedFileFullPath;
            claimAcknowledgmentDetailsViewModel.PdfFileName = ipdResponse.ClaimAcknowledgmentDetails.DisplayFileName;
            return claimAcknowledgmentDetailsViewModel;
        }

        [HttpGet]
        public ValidationCheckResult ValidateEnhancementOrNrCreate(int parentAlInwardId)
        {
            ValidationCheckRequest request = new ValidationCheckRequest() { InwardDetailId = parentAlInwardId };
            ValidationCheckResponse response = new ValidationCheckResponse();
            using (var proxy = new ServiceProxy<IProviderPortalAuthorizationLetterService>())
            {
                response = proxy.Channel.ValidateEnhancementOrNrCreate(request);
            }

            return response.validationCheckResult;
        }
    }
}
