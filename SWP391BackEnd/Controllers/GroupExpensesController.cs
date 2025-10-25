using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using SWP391BackEnd.Helpers;

namespace SWP391BackEnd.Controllers
{
    [ApiController]
    [Route("api/group-expenses")]
    [ApiExplorerSettings(GroupName = "Group Expense Management")]
    public class GroupExpensesController : Controller
    {
        private readonly IGroupExpenseService _service;

        public GroupExpensesController(IGroupExpenseService service)
        {
            _service = service;
        }

        [HttpGet("group/{groupId}")]
        [Authorize]
        public async Task<IActionResult> GetByGroup(Guid groupId)
        {
            var result = await _service.GetByGroupAsync(groupId);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                "Lấy danh sách chi phí nhóm thành công", result);
        }
    }
}
