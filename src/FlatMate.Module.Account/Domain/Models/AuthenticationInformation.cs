﻿using System;
using System.Security.Cryptography;
using System.Text;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.Domain.Models
{
    public class AuthenticationInformation
    {
        private AuthenticationInformation(string passwordHash, string salt, int userId)
        {
            PasswordHash = passwordHash;
            Salt = salt;
            UserId = userId;
        }

        public string PasswordHash { get; }

        public string Salt { get; }

        public int UserId { get; }

        /// <summary>
        ///     Creates new PasswordInformations
        /// </summary>
        public static (Result, AuthenticationInformation) Create(string password, int userId)
        {
            var result = ValidatePlainPassword(password);
            if (result.IsError)
            {
                return (result, null);
            }

            var salt = CreateSalt();
            var passwordHash = CreateHash(password, salt);

            return Create(passwordHash, salt, userId);
        }

        /// <summary>
        ///     Creates existing AuthenticationInformation
        /// </summary>
        public static (Result, AuthenticationInformation) Create(string passwordHash, string salt, int userId)
        {
            return (Result.Success, new AuthenticationInformation(passwordHash, salt, userId));
        }

        public static Result ValidatePlainPassword(string password)
        {
            if (password.Length < 8)
            {
                return new Result(ErrorType.ValidationError, "Password must contain atleast 8 characters.");
            }

            return Result.Success;
        }

        public bool VerifyPassword(string otherPassword)
        {
            var otherHash = CreateHash(otherPassword, Salt);
            return otherHash == PasswordHash;
        }

        private static string CreateHash(string password, string salt)
        {
            return Convert.ToBase64String(SHA512.Create().ComputeHash(Encoding.Unicode.GetBytes(salt + password)));
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
    }
}