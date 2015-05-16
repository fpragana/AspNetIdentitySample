﻿using AspNetIdentitySample.Models;
using Base32;
using Microsoft.AspNet.Identity;
using OtpSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetIdentitySample.App_Start.Identity
{
    public class GoogleAuthenticatorTokenProvider : IUserTokenProvider<ApplicationUser, string>
    {
        public Task<string> GenerateAsync(string purpose, UserManager<ApplicationUser, string> manager, ApplicationUser user)
        {
            return Task.FromResult((string)null);
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<ApplicationUser, string> manager, ApplicationUser user)
        {
            long timeStepMatched = 0;

            var otp = new Totp(Base32Encoder.Decode(user.GoogleAuthenticatorSecretKey));
            bool valid = otp.VerifyTotp(token, out timeStepMatched, new VerificationWindow(2, 2));

            return Task.FromResult(valid);
        }

        public Task NotifyAsync(string token, UserManager<ApplicationUser, string> manager, ApplicationUser user)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsValidProviderForUserAsync(UserManager<ApplicationUser, string> manager, ApplicationUser user)
        {
            return Task.FromResult(user.IsGoogleAuthenticatorEnabled);
        }
    }
}
