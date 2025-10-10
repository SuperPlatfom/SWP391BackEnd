using BusinessObject;
using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Mappers
{
    public static class AccountMapper
    {
        public static AccountResponseModel ToAccountResponseModel(this Account model)
        {
            return new AccountResponseModel
            {
                DateOfBirth = model.DateOfBirth,
                Email = model.Email,
                FullName = model.FullName,
                Gender = model.Gender,
                Phone = model.Phone,
                RoleId = model.RoleId,
                Id = model.Id,
                ImageUrl = model.ImageUrl,
                RoleName = model.Role.Name,
                Status = model.Status,
                Address = model.CitizenIdentityCard != null ? model.CitizenIdentityCard.Address : "N/A",
            };
        }

        public static OfficerResponseModel ToOfficerResponseModel(this Account model)
        {
            return new OfficerResponseModel
            {
                DateOfBirth = model.DateOfBirth,
                Email = model.Email,
                FullName = model.FullName,
                Gender = model.Gender,
                Phone = model.Phone,
                Id = model.Id,
                ImageUrl = model.ImageUrl,
                RoleName = model.Role.Name,
                Status = model.Status,
                Address = model.CitizenIdentityCard != null ? model.CitizenIdentityCard.Address : "N/A",
            };
        }

        //public static Account ToAccount(this AddAccountRequestModel requestModel)
        //{
        //    return new Account
        //    {
        //        Status = requestModel.Status,
        //        RoleId = requestModel.RoleId,
        //        Phone = requestModel.Phone,
        //        PasswordHash = requestModel.Password,
        //        DateOfBirth = requestModel.DateOfBirth,
        //        IsLoggedIn = false,
        //        Email = requestModel.Email,
        //        FullName = requestModel.FullName,
        //        Gender = requestModel.Gender,
        //        ImageUrl = requestModel.ImageUrl
        //    };
        //}
        public static Account ToAccount(this UpdateAccountRequestModel requestModel, Guid id)
        {
            return new Account
            {
                Id = id,
                Status = requestModel.Status,
                RoleId = requestModel.RoleId,
                Phone = requestModel.Phone,
                DateOfBirth = requestModel.DateOfBirth,
                Email = requestModel.Email,
                FullName = requestModel.FullName,
                Gender = requestModel.Gender,
                ImageUrl = requestModel.ImageUrl,
            };
        }
    }
}
