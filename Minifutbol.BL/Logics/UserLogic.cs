﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using Minifutbol.BL.Base;
using Minifutbol.BL.Mapping;
using Minifutbol.BL.Models.Core;
using Minifutbol.BL.Models.User;
using Minifutbol.DAL.Context;
using Minifutbol.Utils;
using System.Web;
using Minifutbol.BL.Extensions;
using System.Net.Http;

namespace Minifutbol.BL.Logics
{
    public class UserLogic : BaseLogic<UserViewModel, UserCreateModel, UserFindModel, UserUpdateModel, UserDeleteModel>
    {
        #region Properties

        public MinifutbolContext MinifutbolContext;
        private IMapper _mapper;
        public IMapper Mapper => _mapper ?? (_mapper = AutoMapperConfig.CreateMapper());
        public string[] OtherEntities = new[] { "UserClaims"};

        #endregion

        #region Constructor

        public UserLogic()
        {
            MinifutbolContext = new MinifutbolContext();
        }

        #endregion


        #region Validate User

        public User ValidateUser(string userName, string password)
        {
            try
            {
                var user = this.Uow.GetRepository<User>().Find(x => x.UserName == userName, new string[] { "UserClaims" });
                if (user == null)
                {
                    return null;
                }
                var at = user.Password == CHashing.Hash(password, user.Salt) ? user : null;
                return at;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


        #endregion

        #region User Add

        protected override void AddAbstract(UserCreateModel entity)
        {
            var output = new LogicResult<UserViewModel>();
            int seconds = DateTime.Now.Second;
            Random rnd = new Random(seconds);
            string input = rnd.Next().ToString();
            var data = new User
            {
                Email = entity.Email,
                MobileNumber = entity.MobileNumber,
                AccessFailedCount = entity.AccessFailedCount,
                Salt = CHashing.RandomSalt(input),
                LastLoginDate = DateTime.Now,
                UserName = entity.UserName,
                FirstName = entity.FirstName,
                LastName = entity.LastName

            };
            data.Password = CHashing.Hash(entity.Password, data.Salt);
            data.UserClaims.Add(new UserClaim
            {
                ClaimType = "role",
                ClaimValue = "user"
            });
            this.Uow.GetRepository<User>().Add(data, OtherEntities);
            this.Uow.SaveChanges();
            output.Output = Mapper.Map<User, UserViewModel>(data);
            Result = output;
        }

        protected override void AddRangeAbstract(List<UserCreateModel> parameters)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region User Get

        /// <summary>
        /// /
        /// </summary>
        /// <param name="filter"></param>
        protected override void GetAllAbstract(Filter filter)
        {
            var output = new LogicResult<ICollection<UserViewModel>>();
            var data = this.Uow.GetRepository<User>().GetAll(OtherEntities);
            var userId = HttpContext.Current.GetOwinContext().Authentication.User.GetUserId();
           
            //data = data.Where(w => w.IsDeleted == false);
            if (filter.PageSize > 0 && filter.PageNumber > 0)
            {
                data =
               data.OrderByDescending(x => x.Id).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
            }

            output.Output = Mapper.Map<ICollection<User>, ICollection<UserViewModel>>(data.ToList());
            ResultAll = output;
            this.Uow.Dispose();
        }

        protected override void GetByIdAbstract(int id)
        {
            var output = new LogicResult<UserViewModel>();
            var data = this.Uow.GetRepository<User>().GetById(id, OtherEntities);
            output.Output = Mapper.Map<User, UserViewModel>(data);
            Result = output;
        }

        #endregion

        #region User Find

        protected override void FindAbstract(UserFindModel parameters)
        {
            var output = new LogicResult<UserViewModel>();
            var predicate = PredicateBuilder.True<User>();
            if (parameters == null)
            {
                parameters = new UserFindModel();
            }
            if (parameters.TeamId != null)
            {
                predicate = predicate.And(r => r.TeamId == parameters.TeamId);
            }
            if (parameters.Id != null)
            {
                predicate = predicate.And(r => r.Id == parameters.Id);
            }
            if (parameters.LastLoginDate != null)
            {
                predicate = predicate.And(r => r.LastLoginDate == parameters.LastLoginDate);
            }
            if (parameters.FirstName != null)
            {
                predicate = predicate.And(r => r.FirstName == parameters.FirstName);
            }
            if (parameters.LastName != null)
            {
                predicate = predicate.And(r => r.LastName == parameters.LastName);
            }
            if (parameters.Email != null)
            {
                predicate = predicate.And(r => r.Email == parameters.Email);
            }
            if (parameters.PhoneNumber != null)
            {
                predicate = predicate.And(r => r.PhoneNumber == parameters.PhoneNumber);
            }
            if (parameters.AccessFailedCount != null)
            {
                predicate = predicate.And(r => r.AccessFailedCount == parameters.AccessFailedCount);
            }
            if (parameters.UserName != null)
            {
                predicate = predicate.And(r => r.UserName == parameters.UserName);
            }
            if (parameters.MobileNumber != null)
            {
                predicate = predicate.And(r => r.MobileNumber == parameters.MobileNumber);
            }
           
            var data = this.Uow.GetRepository<User>().Find(predicate, OtherEntities);
            output.Output = Mapper.Map<User, UserViewModel>(data);
            Result = output;
        }

        protected override void FindAllAbstract(UserFindModel parameters)
        {
            var output = new LogicResult<ICollection<UserViewModel>>();
            var predicate = PredicateBuilder.True<User>();
            if (parameters == null)
            {
                parameters = new UserFindModel();
            }
            if (parameters.Id != null)
            {
                predicate = predicate.And(r => r.Id == parameters.Id);
            }
            if (parameters.TeamId != null)
            {
                predicate = predicate.And(r => r.TeamId == parameters.TeamId);
            }
            if (parameters.LastLoginDate != null)
            {
                predicate = predicate.And(r => r.LastLoginDate == parameters.LastLoginDate);
            }
            if (parameters.FirstName != null)
            {
                predicate = predicate.And(r => r.FirstName == parameters.FirstName);
            }
            if (parameters.LastName != null)
            {
                predicate = predicate.And(r => r.LastName == parameters.LastName);
            }
            if (parameters.Email != null)
            {
                predicate = predicate.And(r => r.Email == parameters.Email);
            }
            if (parameters.PhoneNumber != null)
            {
                predicate = predicate.And(r => r.PhoneNumber == parameters.PhoneNumber);
            }
            if (parameters.AccessFailedCount != null)
            {
                predicate = predicate.And(r => r.AccessFailedCount == parameters.AccessFailedCount);
            }
            if (parameters.UserName != null)
            {
                predicate = predicate.And(r => r.UserName == parameters.UserName);
            }
            if (parameters.MobileNumber != null)
            {
                predicate = predicate.And(r => r.MobileNumber == parameters.MobileNumber);
            }

            var data = this.Uow.GetRepository<User>().FindAll(predicate, OtherEntities);
            var userId = HttpContext.Current.GetOwinContext().Authentication.User.GetUserId();
          
            if (parameters.PageSize > 0 && parameters.PageNumber > 0)
            {
                data = data.OrderByDescending(x => x.Id).Skip((int)((parameters.PageNumber - 1) * parameters.PageSize)).Take((int)parameters.PageSize);
            }
            var at = data.ToList();
            output.Output = Mapper.Map<ICollection<User>, ICollection<UserViewModel>>(data.ToList());
            ResultAll = output;
            this.Uow.Dispose();
        }

        #endregion

        #region User Update

        protected override void UpdateAbstract(UserUpdateModel entity)
        {
            var output = new LogicResult<UserViewModel>();
            var curentUser = this.Uow.GetRepository<User>().GetById(entity.Id);
            var user = Mapper.Map(entity, curentUser);
            var data = this.Uow.GetRepository<User>().Update(user);
            output.Output = Mapper.Map<User, UserViewModel>(data);
            Result = output;
        }

        protected override void UpdateRangeAbstract(UserUpdateModel parameter)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region User Remove

        protected override void RemoveAbstract(UserDeleteModel entity)
        {
            var output = new LogicResult<UserViewModel>();
            var user = this.Uow.GetRepository<User>().Find(x => x.Id == entity.Id);
            if (user != null)
            {
                user.IsDeleted = true;
                this.Uow.GetRepository<User>().Update(user);
            }

            Result.Output = output.Output;
        }


        #endregion
    }
}
