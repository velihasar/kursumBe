
using Business.Handlers.StudentParents.Commands;
using Business.Handlers.StudentParents.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Entities.Concrete;
using System.Collections.Generic;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.StudentParentDto;

namespace WebAPI.Controllers
{
    /// <summary>
    /// StudentParents If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StudentParentsController : BaseApiController
    {
        ///<summary>
        ///List StudentParents
        ///</summary>
        ///<remarks>StudentParents</remarks>
        ///<return>List StudentParents</return>
        ///<response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<StudentParentGetAllDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetStudentParentsQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        ///<summary>
        ///It brings the details according to its id.
        ///</summary>
        ///<remarks>StudentParents</remarks>
        ///<return>StudentParents List</return>
        ///<response code="200"></response>  
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentParentGetByIdDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetStudentParentQuery { Id = id });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Add StudentParent.
        /// </summary>
        /// <param name="createStudentParent"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentParentCreateResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateStudentParentCommand createStudentParent)
        {
            var result = await Mediator.Send(createStudentParent);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Update StudentParent.
        /// </summary>
        /// <param name="updateStudentParent"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentParentUpdateResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateStudentParentCommand updateStudentParent)
        {
            var result = await Mediator.Send(updateStudentParent);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Delete StudentParent.
        /// </summary>
        /// <param name="deleteStudentParent"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteStudentParentCommand deleteStudentParent)
        {
            var result = await Mediator.Send(deleteStudentParent);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}
