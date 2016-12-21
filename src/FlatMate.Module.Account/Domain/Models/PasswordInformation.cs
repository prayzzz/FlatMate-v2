using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using prayzzz.Common.Result;

namespace FlatMate.Module.Account.Domain.Models
{
    internal class PasswordInformation
    {
        private PasswordInformation(string passwordHash, string salt, User user)
        {
            PasswordHash = passwordHash;
            Salt = salt;
            User = user;
        }

        public string PasswordHash { get; }

        public string Salt { get; }

        public User User { get; }

        /// <summary>
        /// Creates new PasswordInformations
        /// </summary>
        public static Result<PasswordInformation> Create(string password, User user)
        {
            var result = ValidatePassword(password);
            if (!result.IsSuccess)
            {
                return new ErrorResult<PasswordInformation>(result);
            }

            var salt = CreateSalt();
            var passwordHash = Convert.ToBase64String(SHA512.Create().ComputeHash(Encoding.Unicode.GetBytes(salt + password)));

            return Create(passwordHash, salt, user);
        }

        /// <summary>
        /// Creates existing PasswordInformation
        /// </summary>
        public static Result<PasswordInformation> Create(string passwordHash, string salt, User user)
        {
            return new SuccessResult<PasswordInformation>(new PasswordInformation(passwordHash, salt, user));
        }

        private static string CreateSalt()
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        private static Result ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return new ErrorResult(ErrorType.ValidationError, $"{password} must not be empty.");
            }

            if (password.Length < 8)
            {
                return new ErrorResult(ErrorType.ValidationError, $"{password} must contain atleast 8 characters.");
            }

            return SuccessResult.Default;
        }
    }
}