using Business.Handlers.StudentBranches.Commands;
using Business.Handlers.StudentBranches.Queries;
using Core.Entities.Dtos.StudentBranchDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentBranchesController : BaseApiController
    {
        /// <summary>
        /// List StudentBranches
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<StudentBranchGetAllDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList([FromQuery] int? studentId, [FromQuery] int? branchId)
        {
            var result = await Mediator.Send(new GetStudentBranchesQuery { StudentId = studentId, BranchId = branchId });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Get StudentBranch by StudentId and BranchId
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentBranchGetByIdDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById([FromQuery] int studentId, [FromQuery] int branchId)
        {
            var result = await Mediator.Send(new GetStudentBranchQuery { StudentId = studentId, BranchId = branchId });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Add StudentBranch
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentBranchCreateResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateStudentBranchCommand createStudentBranch)
        {
            var result = await Mediator.Send(createStudentBranch);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Update StudentBranch
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentBranchUpdateResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateStudentBranchCommand updateStudentBranch)
        {
            var result = await Mediator.Send(updateStudentBranch);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Delete StudentBranch
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteStudentBranchCommand deleteStudentBranch)
        {
            var result = await Mediator.Send(deleteStudentBranch);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}
