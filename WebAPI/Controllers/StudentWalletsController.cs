using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Handlers.StudentWallets.Commands;
using Business.Handlers.StudentWallets.Queries;
using Core.Entities.Dtos.StudentWalletDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Student Wallets (Kantin / Dolap / Bakiye) Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StudentWalletsController : BaseApiController
    {
        /// <summary>
        /// List Student Wallets
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<StudentWalletGetAllDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList([FromQuery] int? tenantId, [FromQuery] string searchTerm)
        {
            var result = await Mediator.Send(new GetStudentWalletsQuery { TenantId = tenantId, SearchTerm = searchTerm });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Get Student Wallet Details by StudentId
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentWalletGetByIdDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getbystudentid")]
        public async Task<IActionResult> GetByStudentId([FromQuery] int studentId)
        {
            var result = await Mediator.Send(new GetStudentWalletByStudentIdQuery { StudentId = studentId });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// List Student Wallet Transactions
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<StudentWalletTransactionGetAllDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("transactions/getall")]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] int? tenantId,
            [FromQuery] int? studentId,
            [FromQuery] int? studentWalletId,
            [FromQuery] int? transactionType,
            [FromQuery] string category,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var result = await Mediator.Send(new GetStudentWalletTransactionsQuery
            {
                TenantId = tenantId,
                StudentId = studentId,
                StudentWalletId = studentWalletId,
                TransactionType = transactionType,
                Category = category,
                StartDate = startDate,
                EndDate = endDate
            });

            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Deposit balance to Student Wallet (Bakiye Yükleme)
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentWalletGetAllDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositStudentWalletCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Spend balance from Student Wallet (Dolap / Kantin Harcaması)
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentWalletGetAllDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost("spend")]
        public async Task<IActionResult> Spend([FromBody] SpendStudentWalletCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Cancel/Delete a wallet transaction (İşlem İptali)
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpDelete("transactions")]
        public async Task<IActionResult> DeleteTransaction([FromBody] DeleteStudentWalletTransactionCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}
