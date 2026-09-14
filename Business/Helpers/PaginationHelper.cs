using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.URI;

namespace Business.Helpers
{
    public static class PaginationHelper
    {
        /// <summary>
        /// Create paginated response with int page numbers
        /// </summary>
        /// <param name="data"></param>
        /// <param name="paginationFilter"></param>
        /// <param name="totalRecords"></param>
        /// <param name="uriService">Optional for backward compatibility</param>
        /// <param name="route">Optional for backward compatibility</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Paginated result with int page numbers</returns>
        public static PaginatedResult<IEnumerable<T>> CreatePaginatedResponse<T>(IEnumerable<T> data, PaginationFilter paginationFilter, int totalRecords, IUriService uriService = null, string route = null)
        {
            data = data.Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize);
            
            int roundedTotalPages;
            var response = new PaginatedResult<IEnumerable<T>>(data, paginationFilter.PageNumber, paginationFilter.PageSize);
            
            var totalPages = totalRecords / (double)paginationFilter.PageSize;
            
            if (paginationFilter.PageNumber <= 0 || paginationFilter.PageSize <= 0)
            {
                roundedTotalPages = 1;
                paginationFilter.PageNumber = 1;
                paginationFilter.PageSize = 1;
            }
            else
            {
                roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
            }

            // NextPage calculation
            if (paginationFilter.PageNumber >= 1 && paginationFilter.PageNumber < roundedTotalPages)
            {
                response.NextPage = paginationFilter.PageNumber + 1;
            }
            else
            {
                response.NextPage = null;
            }

            // PreviousPage calculation
            if (paginationFilter.PageNumber - 1 >= 1 && paginationFilter.PageNumber <= roundedTotalPages)
            {
                response.PreviousPage = paginationFilter.PageNumber - 1;
            }
            else
            {
                response.PreviousPage = null;
            }

            response.FirstPage = 1;
            response.LastPage = roundedTotalPages;
            response.TotalPages = roundedTotalPages;
            response.TotalRecords = totalRecords;
            
            return response;
        }
    }
}