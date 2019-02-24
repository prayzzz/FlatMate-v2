using System;
using System.Text.RegularExpressions;
using FlatMate.Module.Common.Domain.Entities;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.Domain.Models
{
    public class User : Entity
    {
        private static readonly Regex EmailRegex = new Regex("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$");

        private User(int? id, string userName, string email, DateTime created) : base(id)
        {
            Created = created;
            Email = email;
            UserName = userName;
            IsActivated = false;
        }

        public DateTime Created { get; }

        public string Email { get; }

        public bool IsActivated { get; private set; }

        public string UserName { get; }

        public Result Activate()
        {
            IsActivated = true;
            return Result.Success;
        }

        public static (Result, User) Create(string userName, string email)
        {
            return Create(null, userName, email, DateTime.UtcNow);
        }

        public static (Result, User) Create(int? id, string userName, string email, DateTime created)
        {
            #region Validation

            var result = ValidateUserName(userName);
            if (result.IsError)
            {
                return (result, null);
            }

            result = ValidateEmail(email);
            if (result.IsError)
            {
                return (result, null);
            }

            #endregion

            return (Result.Success, new User(id, userName, email, created));
        }

        private static Result ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new Result(ErrorType.ValidationError, "Email must not be empty.");
            }

            if (!EmailRegex.IsMatch(email))
            {
                return new Result(ErrorType.ValidationError, "Invalid email address");
            }

            return Result.Success;
        }

        private static Result ValidateUserName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new Result(ErrorType.ValidationError, "UserName must not be empty.");
            }

            return Result.Success;
        }
    }
}