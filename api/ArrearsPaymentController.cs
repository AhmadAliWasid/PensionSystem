using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionSystem.Interfaces;

namespace WebAPI.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArrearsPaymentController(IMapper mapper, IArrearsPayment arrearsPayment) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;
        private readonly IArrearsPayment _arrearsPayment = arrearsPayment;
    }
}
