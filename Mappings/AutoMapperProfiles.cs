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
        }

    }
}
