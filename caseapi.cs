//

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
