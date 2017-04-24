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
using System.Text;

namespace Uidai.Aadhaar.Helper
{
    /// <summary>
    /// Provides a mechanism to format Aadhaar number.
    /// To format an Aadhaar number with spaces, use 'A' as the format specifier.
    /// </summary>
    public class AadhaarNumberFormatter : ICustomFormatter, IFormatProvider
    {
        /// <summary>
        /// Converts the value of a specified object to an equivalent string representation using specified format and culture-specific formatting information.
        /// </summary>
        /// <param name="format">A format string containing formatting specifications.</param>
        /// <param name="arg">An object to format.</param>
        /// <param name="formatProvider">An object that supplies format information about the current instance.</param>
        /// <returns>The string representation of the value of <paramref name="arg"/>, formatted as specified by <paramref name="format"/> and <paramref name="formatProvider"/>.</returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                HandleOtherFormats(format, arg);
            else if (format.Length > 1)
                format = format.Substring(0, 1).ToUpper();

            var aadhaarNumber = arg as string;
            if (aadhaarNumber == null || format.ToUpperInvariant() != "A")
                return HandleOtherFormats(format, arg);

            var builder = new StringBuilder(aadhaarNumber, 14);
            builder.Insert(4, " ");
            builder.Insert(9, " ");
            return builder.ToString();
        }

        /// <summary>
        /// Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="formatType">An object that specifies the type of format object to return.</param>
        /// <returns>An instance of the object specified by <paramref name="formatType"/>, if the <see cref="IFormatProvider"/> implementation can supply that type of object; otherwise, null.</returns>
        public object GetFormat(Type formatType) => formatType == typeof(ICustomFormatter) ? this : null;

        private static string HandleOtherFormats(string format, object arg)
        {
            var formattable = arg as IFormattable;
            if (formattable != null)
                return formattable.ToString(format, CultureInfo.CurrentCulture);
            return arg?.ToString() ?? string.Empty;
        }
    }
}