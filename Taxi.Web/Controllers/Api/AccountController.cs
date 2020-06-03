using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taxi.Common.Enum;
using Taxi.Common.Models;
using Taxi.Web.Data;
using Taxi.Web.Data.Entities;
using Taxi.Web.Helpers;

namespace Taxi.Web.Controllers.Api
{
    [Route("api/[Controller]")]
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public AccountController(
            DataContext dataContext,
            IUserHelper userHelper,
            IMailHelper mailHelper,
            IImageHelper imageHelper,
            IConverterHelper converterHelper)
        {
            _context = dataContext;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _imageHelper = imageHelper;
            this._converterHelper = converterHelper;
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request",
                    Result = ModelState
                });
            }


            UserEntity user = await _userHelper.GetUserAsync(request.Email);
            if (user != null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "User al ready exist"
                });
            }

            string picturePath = string.Empty;
            if (request.PictureArray != null && request.PictureArray.Length > 0)
            {
                picturePath = _imageHelper.UploadImage(request.PictureArray, "Users");
            }

            user = new UserEntity
            {
                Address = request.Address,
                Document = request.Document,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.Phone,
                UserName = request.Email,
                PicturePath = picturePath,
                UserType = request.UserTypeId == 1 ? UserType.User : UserType.Driver
            };

            IdentityResult result = await _userHelper.AddUserAsync(user, request.Password);
            if (result != IdentityResult.Success)
            {
                return BadRequest(result.Errors.FirstOrDefault().Description);
            }

            UserEntity userNew = await _userHelper.GetUserAsync(request.Email);
            await _userHelper.AddUserToRoleAsync(userNew, user.UserType.ToString());

            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            string tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = user.Id,
                token = myToken
            }, protocol: HttpContext.Request.Scheme);

            _mailHelper.SendMail(request.Email, "Email Confimr", $"<h1>{"Confirm"}</h1>" +
                $"{"Fine"}</br></br><a href = \"{tokenLink}\">{"this fine email"}</a>");

            return Ok(new Response
            {
                IsSuccess = true,
                Message = "User posibility Activity"
            });
        }


        [HttpPost]
        [Route("RecoverPassword")]
        public async Task<IActionResult> RecoverPassword([FromBody] EmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request",
                    Result = ModelState
                });
            }        

            UserEntity user = await _userHelper.GetUserAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "User exist"
                });
            }

            string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
            string link = Url.Action("ResetPassword", "Account", new { token = myToken }, protocol: HttpContext.Request.Scheme);
            _mailHelper.SendMail(request.Email, "User ", $"<h1>{"Recover password"}</h1>" +
                $"{"For reset password clic next email"}</br></br><a href = \"{link}\">{"Recover password now"}</a>");

            return Ok(new Response
            {
                IsSuccess = true,
                Message = "Recover password"
            });
        }


        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutUser([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            UserEntity userEntity = await _userHelper.GetUserAsync(request.Email);
            if (userEntity == null)
            {
                return BadRequest();
            }

            string picturePath = userEntity.PicturePath;
            if (request.PictureArray != null && request.PictureArray.Length > 0)
            {
                picturePath = _imageHelper.UploadImage(request.PictureArray, "Users");
            }

            userEntity.FirstName = request.FirstName;
            userEntity.LastName = request.LastName;
            userEntity.Address = request.Address;
            userEntity.PhoneNumber = request.Phone;
            userEntity.Document = request.Phone;
            userEntity.PicturePath = picturePath;

            IdentityResult respose = await _userHelper.UpdateUserAsync(userEntity);
            if (!respose.Succeeded)
            {
                return BadRequest(respose.Errors.FirstOrDefault().Description);
            }

            UserEntity updatedUser = await _userHelper.GetUserAsync(request.Email);
            return Ok(updatedUser);
        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request",
                    Result = ModelState
                });
            }

         

            UserEntity user = await _userHelper.GetUserAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Error to user"
                });
            }

            IdentityResult result = await _userHelper.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = result.Errors.FirstOrDefault().Description
                });
            }

            return Ok(new Response
            {
                IsSuccess = true,
                Message = "Reponse succes"
            });
        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail([FromBody] EmailRequest emailRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }


            UserEntity userEntity = await _userHelper.GetUserAsync(emailRequest.Email);
            if (userEntity == null)
            {
                return NotFound();
            }

            return Ok(_converterHelper.ToUserResponse(userEntity));
        }


        //[HttpPost]
        //[Route("CompleteTrip")]
        //public async Task<IActionResult> CompleteTrip([FromBody] CompleteTripRequest completeTripRequest)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    TripEntity trip = await _context.Trips
        //        .Include(t => t.TripDetails)
        //        .FirstOrDefaultAsync(t => t.Id == completeTripRequest.TripId);
        //    if (trip == null)
        //    {
        //        return BadRequest("Trip not found.");
        //    }

        //    trip.EndDate = DateTime.UtcNow;
        //    trip.Qualification = completeTripRequest.Qualification;
        //    trip.Remarks = completeTripRequest.Remarks;
        //    trip.Target = completeTripRequest.Target;
        //    trip.TargetLatitude = completeTripRequest.TargetLatitude;
        //    trip.TargetLongitude = completeTripRequest.TargetLongitude;
        //    trip.TripDetails.Add(new TripDetailEntity
        //    {
               
        //        Date = DateTime.UtcNow,
        //        Latitude = completeTripRequest.TargetLatitude,
        //        Longitude = completeTripRequest.TargetLongitude
        //    });

        //    _context.Trips.Update(trip);
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTripEntity([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var tripEntity = await _context.Trips
        //        .Include(t => t.TripDetails)
        //        .FirstOrDefaultAsync(t => t.Id == id);
        //    if (tripEntity == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Trips.Remove(tripEntity);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //[HttpPost]
        //[Route("AddTripDetails")]
        //public async Task<IActionResult> AddTripDetails([FromBody] TripDetailsRequest tripDetailsRequest)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (tripDetailsRequest.TripDetails == null || tripDetailsRequest.TripDetails.Count == 0)
        //    {
        //        return NoContent();
        //    }

        //    TripEntity trip = await _context.Trips
        //        .Include(t => t.TripDetails)
        //        .FirstOrDefaultAsync(t => t.Id == tripDetailsRequest.TripDetails.FirstOrDefault().TripId);
        //    if (trip == null)
        //    {
        //        return BadRequest("Trip not found.");
        //    }

        //    if (trip.TripDetails == null)
        //    {
        //        trip.TripDetails = new List<TripDetailEntity>();
        //    }

        //    foreach (TripDetailRequest tripDetailRequest in tripDetailsRequest.TripDetails)
        //    {
        //        trip.TripDetails.Add(new TripDetailEntity
        //        {
                   
        //            Date = DateTime.UtcNow,
        //            Latitude = tripDetailRequest.Latitude,
        //            Longitude = tripDetailRequest.Longitude
        //        });
        //    }

        //    _context.Trips.Update(trip);
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}




    }
}