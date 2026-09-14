
using Business.Handlers.Tenants.Commands;
using Business.Handlers.Tenants.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Entities.Concrete;
using System.Collections.Generic;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.TenantDto;
using Core.Services;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Tenants If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : BaseApiController
    {
        ///<summary>
        ///List Tenants
        ///</summary>
        ///<remarks>Tenants</remarks>
        ///<return>List Tenants</return>
        ///<response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TenantGetAllDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetTenantsQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        ///<summary>
        ///It brings the details according to its id.
        ///</summary>
        ///<remarks>Tenants</remarks>
        ///<return>Tenants List</return>
        ///<response code="200"></response>  
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TenantGetByIdDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetTenantQuery { Id = id });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Add Tenant.
        /// </summary>
        /// <param name="createTenant"></param>
        /// <returns></returns>
        [Consumes("multipart/form-data")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TenantCreateResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] CreateTenantCommand createTenant)
        {
            var result = await Mediator.Send(createTenant);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Update Tenant.
        /// </summary>
        /// <param name="updateTenant"></param>
        /// <returns></returns>
        [Consumes("multipart/form-data")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TenantUpdateResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateTenantCommand updateTenant)
        {
            var result = await Mediator.Send(updateTenant);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Standalone Upload Logo to MinIO.
        /// </summary>
        [Consumes("multipart/form-data")]
        [HttpPost("upload-logo")]
        public async Task<IActionResult> UploadLogo(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Dosya yüklenmedi.");

            var minioService = (IMinioService)HttpContext.RequestServices.GetService(typeof(IMinioService));
            if (minioService == null)
                return StatusCode(500, "MinioService sunucuda tanımlı değil.");

            using (var stream = file.OpenReadStream())
            {
                var logoUrl = await minioService.UploadFileAsync(stream, file.FileName);
                return Ok(new { logoUrl, url = logoUrl });
            }
        }

        /// <summary>
        /// Delete Tenant.
        /// </summary>
        /// <param name="deleteTenant"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteTenantCommand deleteTenant)
        {
            var result = await Mediator.Send(deleteTenant);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Public endpoint to get Tenant logo image from MinIO
        /// </summary>
        [AllowAnonymous]
        [HttpGet("logo/{*fileName}")]
        public async Task<IActionResult> GetLogo(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName)) return NotFound();
                
                // Clean bucket name prefix if included in path (e.g. "okul/guid_file.png" or "/okul/guid_file.png")
                var cleanName = System.IO.Path.GetFileName(fileName);
                
                var minioService = (IMinioService)HttpContext.RequestServices.GetService(typeof(IMinioService));
                if (minioService == null) return NotFound();

                var stream = await minioService.GetFileAsync(cleanName);
                if (stream == null) return NotFound();

                string contentType = "image/png";
                var lower = cleanName.ToLowerInvariant();
                if (lower.EndsWith(".jpg") || lower.EndsWith(".jpeg")) contentType = "image/jpeg";
                else if (lower.EndsWith(".gif")) contentType = "image/gif";
                else if (lower.EndsWith(".svg")) contentType = "image/svg+xml";
                else if (lower.EndsWith(".webp")) contentType = "image/webp";

                return File(stream, contentType);
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
