using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using SWP391BackEnd.Helpers;
using System.Net;

namespace SWP391BackEnd.Controllers
{
    [ApiController]
    [Route("api/contract-templates")]
    [ApiExplorerSettings(GroupName = "Contracts Template")]
    public class ContractTemplateController : ControllerBase
    {
        private readonly IContractTemplateService _service;

        public ContractTemplateController(IContractTemplateService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContractTemplateSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ContractTemplateDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDetail(Guid id) => Ok(await _service.GetDetailAsync(id));

        [Authorize(Roles = "Staff,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTemplateDto dto)
            => CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Template created", await _service.CreateAsync(dto));

        [Authorize(Roles = "Staff,Admin")]
        [HttpPut("{id}/content")]
        public async Task<IActionResult> UpdateContent(Guid id, [FromBody] UpdateContentDto dto)
        {
            await _service.UpdateContentAsync(id, dto.Content);
            return Ok("Updated content successfully");
        }

        [Authorize(Roles = "Staff,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(Guid id, [FromBody] UpdateTemplateDto dto)
        {
            var result = await _service.UpdateTemplateAsync(id, dto);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Template updated successfully", result);
        }

        [Authorize(Roles = "Staff,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok("Template deleted");
        }

        [HttpGet("{id}/clauses")]
        public async Task<IActionResult> GetClauses(Guid id) => Ok(await _service.GetClausesAsync(id));

        [Authorize(Roles = "Staff,Admin")]
        [HttpPost("{id}/clauses")]
        public async Task<IActionResult> AddClause(Guid id, [FromBody] CreateClauseDto dto)
        {
            await _service.AddClauseAsync(id, dto);
            return Ok("Clause added");
        }

        [Authorize(Roles = "Staff,Admin")]
        [HttpPut("clauses/{clauseId}")]
        public async Task<IActionResult> UpdateClause(Guid clauseId, [FromBody] UpdateClauseDto dto)
        {
            await _service.UpdateClauseAsync(clauseId, dto);
            return Ok("Clause updated");
        }

        [Authorize(Roles = "Staff,Admin")]
        [HttpDelete("clauses/{clauseId}")]
        public async Task<IActionResult> DeleteClause(Guid clauseId)
        {
            await _service.DeleteClauseAsync(clauseId);
            return Ok("Clause deleted");
        }

        [HttpGet("{id}/variables")]
        public async Task<IActionResult> GetVariables(Guid id) => Ok(await _service.GetVariablesAsync(id));

        [Authorize(Roles = "Staff,Admin")]
        [HttpPost("{id}/variables")]
        public async Task<IActionResult> AddVariable(Guid id, [FromBody] CreateVariableDto dto)
        {
            await _service.AddVariableAsync(id, dto);
            return Ok("Variable added");
        }

        [Authorize(Roles = "Staff,Admin")]
        [HttpPut("variables/{variableId}")]
        public async Task<IActionResult> UpdateVariable(Guid variableId, [FromBody] UpdateVariableDto dto)
        {
            await _service.UpdateVariableAsync(variableId, dto);
            return Ok("Variable updated");
        }

        [Authorize(Roles = "Staff,Admin")]
        [HttpDelete("variables/{variableId}")]
        public async Task<IActionResult> DeleteVariable(Guid variableId)
        {
            await _service.DeleteVariableAsync(variableId);
            return Ok("Variable deleted");
        }
    }
}
