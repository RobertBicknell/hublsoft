using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//
using LoginMetricsInterfaces;

namespace Hublsoft.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginMetricsController : ControllerBase
    {
        ILoginMetricsEngine _engine;
        public LoginMetricsController(ILoginMetricsEngine engine) {
            _engine = engine;
        }

        [HttpGet("{N:int}/{start}/{end}")]
        public JsonResult GetTopNLogins(int N, DateTime start, DateTime end)
        {
            return new JsonResult(_engine.TopNLoginsInPeriod(start, end, N));
        }
        
        [HttpGet("{start}/{end}/{period}")]
        public JsonResult GetAverageLoginsByPeriod(DateTime start, DateTime end, Period period)
        {
            return new JsonResult(_engine.AverageLoginsByPeriod(start, end, period));
        }     
        
        [HttpGet("{start}/{end}/{period}/{user}")]
        public JsonResult GetAverageUserLoginsByPeriod(DateTime start, DateTime end, Period period, string user)
        {
            return new JsonResult(_engine.AverageUserLoginsByPeriod(start, end, period, user));
        }
        
    }
}
