#region Copyright
/********************************************************************************
 * Aadhaar API for .NET
 * Copyright © 2015 Souvik Dey Chowdhury
 * 
 * This file is part of Aadhaar API for .NET.
 * 
 * Aadhaar API for .NET is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 * 
 * Aadhaar API for .NET is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
 * for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with Aadhaar API for .NET. If not, see http://www.gnu.org/licenses.
 ********************************************************************************/
#endregion

using System;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ErrorMessage;

namespace Uidai.Aadhaar.Internal
{
    internal static class ExceptionHelper
    {
        public static string ValidateAadhaarNumber(string aadhaarNumber, string argumentName)
        {
            if (!string.IsNullOrEmpty(aadhaarNumber) && !AadhaarHelper.ValidateAadhaarNumber(aadhaarNumber))
                throw new ArgumentException(InvalidAadhaarNumber, argumentName);

            return aadhaarNumber;
        }

        public static string ValidateEmptyString(string argument, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argument))
                throw new ArgumentException(RequiredNonEmptyString, argumentName);

            return argument;
        }

        public static int ValidateMatchPercent(int percent, string argumentName)
        {
            if (percent < 1 || percent > 100)
                throw new ArgumentOutOfRangeException(argumentName, OutOfRangeMatchPercent);

            return percent;
        }

        public static T ValidateNull<T>(T argument, string argumentName) where T : class
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);

            return argument;
        }
    }
}