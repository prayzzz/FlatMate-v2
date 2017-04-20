using System;
using System.Text.RegularExpressions;
using FlatMate.Module.Common.Domain.Entities;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.Domain.Models
{
    public class User : Entity
    {
        private static readonly Regex EmailRegex = new Regex("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$");

        private User(int? id, string userName, string email) : base(id)
        {
            Created = DateTime.Now;
            Email = email;
            UserName = userName;
        }

        public DateTime Created { get; set; }

        public string Email { get; }

        public string UserName { get; }

        internal static Result<User> Create(string userName, string email)
        {
            return Create(null, userName, email);
        }

        internal static Result<User> Create(int? id, string userName, string email)
        {
            #region Validation

            var result = ValidateUserName(userName);
            if (!result.IsSuccess)
            {
                return new ErrorResult<User>(result);
            }

            result = ValidateEmail(email);
            if (!result.IsSuccess)
            {
                return new ErrorResult<User>(result);
            }

            #endregion

            return new SuccessResult<User>(new User(id, userName, email));
        }

        private static Result ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new ErrorResult(ErrorType.ValidationError, "Email must not be empty.");
            }

            if (!EmailRegex.IsMatch(email))
            {
                return new ErrorResult(ErrorType.ValidationError, "Invalid email address");
            }

            return SuccessResult.Default;
        }

        private static Result ValidateUserName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new ErrorResult(ErrorType.ValidationError, "UserName must not be empty.");
            }

            return SuccessResult.Default;
        }
    }
}