namespace WebAPI.Helpers
{
    public class SessionHelper(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IConfiguration _configuration = configuration;
        public int GetUserPDUId()
        {
            int iPDUId = 0;
            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
            {
                var pduId = _httpContextAccessor.HttpContext.Session.GetInt32("PDUId");
                if (pduId.HasValue)
                {
                    iPDUId = pduId.Value;
                }
            }
            return iPDUId;
        }

        public string GetDMStamp()
        {
            string DeputyManagerForStamp = string.Empty;
            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
            {
                var sDeputyManagerForStamp = _httpContextAccessor.HttpContext.Session.GetString("DMStamp");
                if (sDeputyManagerForStamp != null)
                    DeputyManagerForStamp = sDeputyManagerForStamp;
            }
            return DeputyManagerForStamp;
        }

        public string GetAMStamp()
        {
            string AMForStamp = string.Empty;
            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
            {
                var sAMForStamp = _httpContextAccessor.HttpContext.Session.GetString("AMStamp");
                if (sAMForStamp != null)
                    AMForStamp = sAMForStamp;
            }
            return AMForStamp;
        }

        public string GetBaseStamp()
        {
            string BaseForStamp = string.Empty;
            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
            {
                var sBaseForStampForStamp = _httpContextAccessor.HttpContext.Session.GetString("BaseStamp");
                if (sBaseForStampForStamp != null)
                    BaseForStamp = sBaseForStampForStamp;
            }
            return BaseForStamp;
        }
        public Uri GetUri()
        {
            string Uri = _configuration["BaseUrl"];
            if (Uri != null)
                return new Uri(Uri);
            else
                return new Uri("");
        }
    }

    public class SessionViewModel
    {
        public int PDUId { get; set; }
        public string DMStamp { get; set; } = string.Empty;
        public string AMStamp { get; set; } = string.Empty;
        public string BaseStamp { get; set; } = string.Empty;
    }
}