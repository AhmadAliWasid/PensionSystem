using AutoMapper;
using PensionSystem.DTOs;
using PensionSystem.Entities.DTOs;
using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace PensionSystem.Mappings
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
        }

    }
}
