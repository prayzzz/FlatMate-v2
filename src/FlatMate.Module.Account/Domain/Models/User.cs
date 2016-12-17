using System;
using FlatMate.Module.Common.Domain.Entities;
using prayzzz.Common.Result;

namespace FlatMate.Module.Account.Domain.Models
{
    internal class User : Entity
    {
        private User(int id, string userName, string email, DateTime creationDate)
            : base(id)
        {
            CreationDate = creationDate;
            EMail = email;
            UserName = userName;
        }

        public DateTime CreationDate { get; }

        public string EMail { get; }

        public string UserName { get; }

        internal static Result<User> Create(string userName, string email)
        {
            return Create(DefaultId, userName, email, DateTime.Now);
        }

        internal static Result<User> Create(int id, string userName, string email, DateTime creationDate)
        {
            #region Validation

            var result = ValidateName(userName);
            if (!result.IsSuccess)
            {
                return new ErrorResult<User>(result);
            }

            result = ValidateEmail(userName);
            if (!result.IsSuccess)
            {
                return new ErrorResult<User>(result);
            }

            #endregion

            return new SuccessResult<User>(new User(id, userName, email, creationDate));
        }

        private static Result ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new ErrorResult(ErrorType.ValidationError, $"{nameof(email)} must not be empty.");
            }

            return SuccessResult.Default;
        }

        private static Result ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new ErrorResult(ErrorType.ValidationError, $"{nameof(name)} must not be empty.");
            }

            return SuccessResult.Default;
        }
    }
}