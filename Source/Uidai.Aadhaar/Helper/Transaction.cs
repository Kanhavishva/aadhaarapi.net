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
using System.Globalization;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Helper
{
    /// <summary>
    /// Provides a wrapper to generate transaction identifier included in API requests.
    /// Maximum length of full transaction identifier should be 50 characters.
    /// Only supported characters are A-Z, a-z, 0-9, . , - \ / ( ) :.
    /// </summary>
    public class Transaction
    {
        private static Func<string> generator = () => DateTimeOffset.Now.ToString("yyyyMMddhhmmssfff", CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets or sets a generator function to generate unique identifiers.
        /// </summary>
        /// <value>A generator function to generate unique identifiers.</value>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        public static Func<string> Generator
        {
            get { return generator; }
            set
            {
                ValidateNull(value, nameof(Generator));
                generator = value;
            }
        }

        /// <summary>
        /// Gets or sets the prefix to add to the identifier.
        /// </summary>
        /// <value>The prefix to add to the identifier.</value>
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier of the transaction.
        /// Default is <see cref="DateTimeOffset.Now"/>.
        /// It should not contain ':'.
        /// </summary>
        /// <value>The unique identifier of the transaction.</value>
        public string Value { get; set; } = Generator();

        /// <summary>
        /// Performs an implicit conversion from <see cref="string"/> to <see cref="Transaction"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Transaction(string value)
        {
            var transaction = new Transaction();
            int index;
            if (value != null && (index = value.LastIndexOf(':')) != -1)
            {
                transaction.Prefix = value.Substring(0, index + 1);
                transaction.Value = value.Substring(index + 1);
            }
            else
                transaction.Value = value;

            return transaction;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Transaction"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(Transaction value)
        {
            if (value == null)
                return null;
            return string.IsNullOrEmpty(value.Prefix) ? value.Value : value.Prefix + value.Value;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => this;
    }
}