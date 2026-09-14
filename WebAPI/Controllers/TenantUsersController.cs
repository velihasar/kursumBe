
using Business.Handlers.TenantUsers.Commands;
using Business.Handlers.TenantUsers.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Entities.Concrete;
using System.Collections.Generic;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.TenantUserDto;

namespace WebAPI.Controllers
{
    /// <summary>
    /// TenantUsers If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TenantUsersController : BaseApiController
    {
        ///<summary>
        ///List TenantUsers
        ///</summary>
        ///<remarks>TenantUsers</remarks>
        ///<return>List TenantUsers</return>
        ///<response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TenantUserGetAllDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetTenantUsersQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        ///<summary>
        ///It brings the details according to its id.
        ///</summary>
        ///<remarks>TenantUsers</remarks>
        ///<return>TenantUsers List</return>
        ///<response code="200"></response>  
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TenantUserGetByIdDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetTenantUserQuery { Id = id });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Add TenantUser.
        /// </summary>
        /// <param name="createTenantUser"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TenantUserCreateResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateTenantUserCommand createTenantUser)
        {
            var result = await Mediator.Send(createTenantUser);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Update TenantUser.
        /// </summary>
        /// <param name="updateTenantUser"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TenantUserUpdateResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateTenantUserCommand updateTenantUser)
        {
            var result = await Mediator.Send(updateTenantUser);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Delete TenantUser.
        /// </summary>
        /// <param name="deleteTenantUser"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteTenantUserCommand deleteTenantUser)
        {
            var result = await Mediator.Send(deleteTenantUser);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}
