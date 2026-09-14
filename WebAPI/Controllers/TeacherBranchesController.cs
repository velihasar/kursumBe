using Business.Handlers.TeacherBranches.Commands;
using Business.Handlers.TeacherBranches.Queries;
using Core.Entities.Dtos.TeacherBranchDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherBranchesController : BaseApiController
    {
        /// <summary>
        /// List TeacherBranches
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TeacherBranchGetAllDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList([FromQuery] int? teacherId, [FromQuery] int? branchId)
        {
            var result = await Mediator.Send(new GetTeacherBranchesQuery { TeacherId = teacherId, BranchId = branchId });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Get TeacherBranch by TeacherId and BranchId
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TeacherBranchGetByIdDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById([FromQuery] int teacherId, [FromQuery] int branchId)
        {
            var result = await Mediator.Send(new GetTeacherBranchQuery { TeacherId = teacherId, BranchId = branchId });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Add TeacherBranch
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TeacherBranchCreateResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateTeacherBranchCommand createTeacherBranch)
        {
            var result = await Mediator.Send(createTeacherBranch);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Update TeacherBranch
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TeacherBranchUpdateResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateTeacherBranchCommand updateTeacherBranch)
        {
            var result = await Mediator.Send(updateTeacherBranch);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Delete TeacherBranch
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteTeacherBranchCommand deleteTeacherBranch)
        {
            var result = await Mediator.Send(deleteTeacherBranch);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}
