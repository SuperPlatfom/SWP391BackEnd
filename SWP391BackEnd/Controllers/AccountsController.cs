using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Service.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Repository.HandleException;
using System.ComponentModel.DataAnnotations;
using SWP391BackEnd.Helpers;
using Service;

namespace SWP391BackEnd.Controllers
{
    [Route("api/accounts")]
    [ApiExplorerSettings(GroupName = "Accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("create-staff")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStaff([FromForm] AddAccountRequestModel request)
        {
            try
            {
                await _accountService.AddAsync(request, "Staff");
                return CustomSuccessHandler.ResponseBuilder(
                    HttpStatusCode.Created,
                    "Tạo tài khoản Staff thành công",
                    null
                );
            }
            catch (InvalidOperationException ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 400);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 500);
            }
        }

        [HttpPost("create-technician")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTechnician([FromForm] AddAccountRequestModel request)
        {
            try
            {
                await _accountService.AddAsync(request, "Technician");
                return CustomSuccessHandler.ResponseBuilder(
                    HttpStatusCode.Created,
                    "Tạo tài khoản Technician thành công",
                    null
                );
            }
            catch (InvalidOperationException ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 400);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return CustomErrorHandler.SimpleError(ex.Message, 500);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAll([FromQuery] string? role = null)
        {
            var result = await _accountService.GetAllAsync(role);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                "Lấy danh sách tài khoản thành công", result);
        }
    }

}

