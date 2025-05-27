
using Microsoft.AspNetCore.Mvc;
using mail_marketing_api.Models;

namespace mail_marketing_api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SendMailController : Controller
    {
        [HttpPost]
        public IActionResult SendMail() {
            try
            {
                return Ok("Thanh cong");
            }
            catch (Exception ex) {
                return BadRequest("Cos looix xayr ra: " + ex.Message);
            }
        
        }
    }
}

