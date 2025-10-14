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
        [ProducesResponseType(typeof(IEnumerable<ContractTemplateDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ContractTemplateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetail(Guid id)
            => Ok(await _service.GetDetailAsync(id));

        [HttpPost]
        [Authorize] // Admin
        [ProducesResponseType(typeof(ContractTemplateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTemplateDto dto)
            => CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Template created", await _service.CreateAsync(dto));

        [HttpPut("{id}/content")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateContent(Guid id, [FromBody] UpdateContentDto dto)
        {
            await _service.UpdateContentAsync(id, dto.Content);
            return Ok("Updated content successfully");
        }

        [HttpPost("{id}/clauses")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddClause(Guid id, [FromBody] CreateClauseDto dto)
        {
            await _service.AddClauseAsync(id, dto);
            return Ok("Clause added");
        }

        [HttpPost("{id}/variables")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddVariable(Guid id, [FromBody] CreateVariableDto dto)
        {
            await _service.AddVariableAsync(id, dto);
            return Ok("Variable added");
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok("Template deleted");
        }
    }
}
