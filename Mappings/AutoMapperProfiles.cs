using AutoMapper;
using PensionSystem.Entities.DTOs;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace WebAPI.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ChequeDTO, Cheque>().ReverseMap();
            CreateMap<CreateCashBookDTO, CashBook>().ReverseMap();
            CreateMap<UpdateCashBookDTO, CashBook>().ReverseMap();
            CreateMap<CreateMPDemandDTO, MonthlyPensionDemand>().ReverseMap();
            CreateMap<UpdateMPDemandDTO, MonthlyPensionDemand>().ReverseMap();
            CreateMap<PensionerModel, Pensioner>().ReverseMap();
            CreateMap<CreateArrearDemandModel, ArrearsDemand>().ReverseMap();
            CreateMap<CreateArreardDemandDTO, ArrearsDemand>().ReverseMap();
            CreateMap<UpdateArreardDemandDTO, ArrearsDemand>().ReverseMap();
            CreateMap<CreateArreardDemandDTO, ArreardDemandDTO>().ReverseMap();
            CreateMap<UpdateArreardDemandDTO, ArreardDemandDTO>().ReverseMap();
            CreateMap<CreateWWFSanctionDTO, WWFSanctionDTO>().ReverseMap();
            CreateMap<CreateWWFSanctionDTO, WWFSanction>().ReverseMap();
            CreateMap<WWFSanctionDTO, WWFSanction>().ReverseMap();
            CreateMap<WWFReimbursmentDTO, WGReimbursment>().ReverseMap();
            CreateMap<CreateWWFReimbursmentDTO, WGReimbursment>().ReverseMap();
            CreateMap<CreateWWFReimbursmentDTO, WWFReimbursmentDTO>().ReverseMap();
            CreateMap<PensionerOptionDTO, Pensioner>().ReverseMap();
            CreateMap<PensionerDTO, Pensioner>();
            CreateMap<Pensioner, PensionerDTO>()
                .ForMember(d => d.Session, o => o.Ignore());
            CreateMap<MPDemandDTO, MonthlyPensionDemand>().ReverseMap();
            CreateMap<CommutationDTO, Commutation>().ReverseMap();
            CreateMap<CashBookDTO, CashBook>().ReverseMap();
        }

    }
}
