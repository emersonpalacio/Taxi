using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taxi.Common.Enum;
using Taxi.Common.Models;
using Taxi.Web.Data;
using Taxi.Web.Data.Entities;
using Taxi.Web.Helpers;

namespace Taxi.Web.Controllers.Api
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]

    public class UserGroupsController : Controller
    {
        private readonly DataContext _context;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public UserGroupsController(DataContext context,
                                    IConverterHelper converterHelper,
                                    IUserHelper userHelper,
                                    IMailHelper mailHelper)
        {
            this._context = context;
            this._converterHelper = converterHelper;
            this._userHelper = userHelper;
            this._mailHelper = mailHelper;
        }


        [HttpPost]
        public async Task<IActionResult> PostUserGroup([FromBody] AddUserGroupRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            UserEntity proposalUser = await _userHelper.GetUserAsync(request.UserId);
            if (proposalUser == null)
            {
                return BadRequest();
            }

            UserEntity requiredUser = await _userHelper.GetUserAsync(request.Email);
            if (requiredUser == null)
            {
                return BadRequest();
            }

            UserGroupEntity userGroup = await _context.UserGroups
                .Include(ug => ug.Users)
                .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(ug => ug.User.Id == request.UserId.ToString());
            if (userGroup != null)
            {
                UserGroupDetailEntity user = userGroup.Users.FirstOrDefault(u => u.User.Email == request.Email);
                if (user != null)
                {
                    return BadRequest();
                }
            }

            UserGroupRequestEntity userGroupRequest = new UserGroupRequestEntity
            {
                ProposalUser = proposalUser,
                RequiredUser = requiredUser,
                Status = UserGroupStatus.Pending,
                Token = Guid.NewGuid()
            };

            try
            {
                _context.UserGroupRequests.Add(userGroupRequest);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            string linkConfirm = Url.Action("ConfirmUserGroup", "Account", new
            {
                requestId = userGroupRequest.Id,
                token = userGroupRequest.Token
            }, protocol: HttpContext.Request.Scheme);

            string linkReject = Url.Action("RejectUserGroup", "Account", new
            {
                requestId = userGroupRequest.Id,
                token = userGroupRequest.Token
            }, protocol: HttpContext.Request.Scheme);

            Response response = _mailHelper.SendMail(request.Email, "user request", $"<h1>{"User group"}</h1>" +
                $"{"convert user helper"}: {proposalUser.FullName} ({proposalUser.Email}), {"passs body"}" +
                $"</hr></br></br>{"Sccept"} <a href = \"{linkConfirm}\">{"Confirm"}</a>" +
                $"</hr></br></br>{"in join"} <a href = \"{linkReject}\">{"Reject"}</a>");

            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }

            return Ok();
        }






        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserGroup([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserGroupEntity userGroup = await _context.UserGroups
                .Include(ug => ug.Users)
                .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(u => u.User.Id == id);

            if (userGroup == null || userGroup?.Users == null)
            {
                return Ok();
            }

            return Ok(_converterHelper.ToUserGroupResponse(userGroup.Users.ToList()));
        }



        public async Task<IActionResult> ConfirmUserGroup(int requestId, string token)
        {
            if (requestId == 0 || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            UserGroupRequestEntity userGroupRequest = await _context.UserGroupRequests
                .Include(ugr => ugr.ProposalUser)
                .Include(ugr => ugr.RequiredUser)
                .FirstOrDefaultAsync(ugr => ugr.Id == requestId &&
                                            ugr.Token == new Guid(token));
            if (userGroupRequest == null)
            {
                return NotFound();
            }

            await AddGroupAsync(userGroupRequest.ProposalUser, userGroupRequest.RequiredUser);
            await AddGroupAsync(userGroupRequest.RequiredUser, userGroupRequest.ProposalUser);

            userGroupRequest.Status = UserGroupStatus.Accepted;
            _context.UserGroupRequests.Update(userGroupRequest);
            await _context.SaveChangesAsync();
            return View();
        }

        private async Task AddGroupAsync(UserEntity proposalUser, UserEntity requiredUser)
        {
            UserGroupEntity userGroup = await _context.UserGroups
                .Include(ug => ug.Users)
                .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(ug => ug.User.Id == proposalUser.Id);
            if (userGroup != null)
            {
                UserGroupDetailEntity user = userGroup.Users.FirstOrDefault(u => u.User.Id == requiredUser.Id);
                if (user == null)
                {
                    userGroup.Users.Add(new UserGroupDetailEntity { User = requiredUser });
                }

                _context.UserGroups.Update(userGroup);
            }
            else
            {
                _context.UserGroups.Add(new UserGroupEntity
                {
                    User = proposalUser,
                    Users = new List<UserGroupDetailEntity>
                    {
                        new UserGroupDetailEntity { User = requiredUser }
                    }
                });
            }
        }

        public async Task<IActionResult> RejectUserGroup(int requestId, string token)
        {
            if (requestId == 0 || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            UserGroupRequestEntity userGroupRequest = await _context
                .UserGroupRequests.FirstOrDefaultAsync(ugr => ugr.Id == requestId &&
                                                        ugr.Token == new Guid(token));
            if (userGroupRequest == null)
            {
                return NotFound();
            }

            userGroupRequest.Status = UserGroupStatus.Rejected;
            _context.UserGroupRequests.Update(userGroupRequest);
            await _context.SaveChangesAsync();
            return View();
        }



    }
}