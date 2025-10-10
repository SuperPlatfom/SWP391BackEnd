using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Service.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Repository.HandleException;
using System.ComponentModel.DataAnnotations;

namespace SWP391BackEnd.Controllers
{
    [Route("api/accounts")]
    [ApiExplorerSettings(GroupName = "Accounts")]
    [ApiController]
    public class AccountsController : ODataController
    {
        private readonly IAccountService _accountService;
        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }



    }






}

