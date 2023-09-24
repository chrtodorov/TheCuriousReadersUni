using BusinessLayer.Enumerations;
using BusinessLayer.Interfaces.Notifications;
using BusinessLayer.Models;
using DataAccess.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.RequireLibrarianRole)]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsService notificationsService;

        public NotificationsController(INotificationsService notificationsService)
        {
            this.notificationsService = notificationsService;
        }


        [HttpGet("expiring")]
        public IActionResult GetExpiringBookLoans([FromQuery] PagingParameters pagingParameters)
        {
            var pagedList = notificationsService.GetExpiringBookLoans(pagingParameters);
            var data = pagedList.Data.Select(bookLoan => bookLoan.ToBookLoanResponse());

            return Ok(data.ToPagedList(pagedList.TotalCount, pagedList.CurrentPage, pagedList.PageSize));
        }

        [HttpGet("expired")]
        public IActionResult GetExpiredBookLoans([FromQuery] PagingParameters pagingParameters)
        {
            var pagedList = notificationsService.GetExpiredBookLoans(pagingParameters);
            var data = pagedList.Data.Select(bookLoan => bookLoan.ToBookLoanResponse());

            return Ok(data.ToPagedList(pagedList.TotalCount, pagedList.CurrentPage, pagedList.PageSize));
        }
    }
}