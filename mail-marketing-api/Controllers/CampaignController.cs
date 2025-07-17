using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mail_marketing_api.Models;
using mail_marketing_api.Services;
using Microsoft.AspNetCore.Mvc;
using mail_marketing_api.Extensions;

namespace mail_marketing_api.Controllers
{


    [ApiController]
    [Route("[controller]/[action]")]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCampaigns()
        {
            try
            {
                var campaigns = await _campaignService.GetAll();

                return Ok(new ResponseDTO<List<Campaign>>
                {
                    Code = 200,
                    Data = campaigns,
                    Message = "Lấy danh sách chiến dịch thành công!",
                    IsSuccessed = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<string>
                {
                    Code = 500,
                    Data = null,
                    Message = $"Lỗi hệ thống: {ex.Message}",
                    IsSuccessed = false
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCampaignById(int id)
        {
            try
            {
                var campaign = await _campaignService.GetByIdAsync(id);

                if (campaign == null)
                {
                    return NotFound(new ResponseDTO<string>
                    {
                        Code = 404,
                        Data = null,
                        Message = "Không tìm thấy chiến dịch.",
                        IsSuccessed = false
                    });
                }

                return Ok(new ResponseDTO<Campaign>
                {
                    Code = 200,
                    Data = campaign,
                    Message = "Lấy chiến dịch thành công!",
                    IsSuccessed = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<string>
                {
                    Code = 500,
                    Data = null,
                    Message = $"Lỗi hệ thống: {ex.Message}",
                    IsSuccessed = false
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCampaign([FromBody] Campaign campaign)
        {
            try
            {
                var result = await _campaignService.CreateCampaignAsync(campaign);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<string>
                {
                    Code = 500,
                    Data = null,
                    Message = $"Lỗi hệ thống: {ex.Message}",
                    IsSuccessed = false
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCampaign(int id, [FromBody] Campaign campaign)
        {
            try
            {
                var updated = await _campaignService.UpdateCampaignAsync(id, campaign);

                return Ok(new ResponseDTO<Campaign>
                {
                    Code = 200,
                    Data = updated,
                    Message = "Cập nhật chiến dịch thành công!",
                    IsSuccessed = true
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ResponseDTO<string>
                {
                    Code = 404,
                    Data = null,
                    Message = "Không tìm thấy chiến dịch để cập nhật.",
                    IsSuccessed = false
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<string>
                {
                    Code = 500,
                    Data = null,
                    Message = $"Lỗi hệ thống: {ex.Message}",
                    IsSuccessed = false
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaign(int id)
        {
            try
            {
                var deleted = await _campaignService.DeleteCampaignAsync(id);

                if (!deleted)
                {
                    return NotFound(new ResponseDTO<string>
                    {
                        Code = 404,
                        Data = null,
                        Message = "Không tìm thấy chiến dịch để xoá.",
                        IsSuccessed = false
                    });
                }

                return Ok(new ResponseDTO<string>
                {
                    Code = 200,
                    Data = null,
                    Message = "Xoá chiến dịch thành công!",
                    IsSuccessed = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO<string>
                {
                    Code = 500,
                    Data = null,
                    Message = $"Lỗi hệ thống: {ex.Message}",
                    IsSuccessed = false
                });
            }
        }
    }
}

