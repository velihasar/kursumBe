using Business.Handlers.CourseEnrollments.Commands;
using Business.Handlers.CourseEnrollments.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Core.Entities.Dtos.CourseEnrollmentDto;

namespace WebAPI.Controllers
{
    /// <summary>
    /// CourseEnrollments Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CourseEnrollmentsController : BaseApiController
    {
        ///<summary>
        ///List CourseEnrollments
        ///</summary>
        ///<remarks>CourseEnrollments</remarks>
        ///<return>List CourseEnrollments</return>
        ///<response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CourseEnrollmentGetAllDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList([FromQuery] int? tenantId)
        {
            var result = await Mediator.Send(new GetCourseEnrollmentsQuery { TenantId = tenantId });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        ///<summary>
        ///It brings the details according to its id.
        ///</summary>
        ///<remarks>CourseEnrollments</remarks>
        ///<return>CourseEnrollment Detail</return>
        ///<response code="200"></response>  
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseEnrollmentGetByIdDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetCourseEnrollmentQuery { Id = id });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Add CourseEnrollment.
        /// </summary>
        /// <param name="createCourseEnrollment"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseEnrollmentCreateResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateCourseEnrollmentCommand createCourseEnrollment)
        {
            var result = await Mediator.Send(createCourseEnrollment);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Update CourseEnrollment.
        /// </summary>
        /// <param name="updateCourseEnrollment"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseEnrollmentUpdateResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCourseEnrollmentCommand updateCourseEnrollment)
        {
            var result = await Mediator.Send(updateCourseEnrollment);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Delete CourseEnrollment.
        /// </summary>
        /// <param name="deleteCourseEnrollment"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteCourseEnrollmentCommand deleteCourseEnrollment)
        {
            var result = await Mediator.Send(deleteCourseEnrollment);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}
