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
using System.Collections.Generic;

namespace Uidai.Aadhaar.Agency
{
    /// <summary>
    /// Represents a collection of configurable properties assigned to agencies used during Aadhaar API request.
    /// </summary>
    public class UserAgency
    {
        /// <summary>
        /// Gets or sets the AUA license key.
        /// </summary>
        /// <value>The AUA license key.</value>
        public string AuaLicenseKey { get; set; }

        /// <summary>
        /// Gets or sets the ASA license key.
        /// </summary>
        /// <value>The ASA license key.</value>
        public string AsaLicenseKey { get; set; }

        /// <summary>
        /// Gets or sets the AUA code.
        /// </summary>
        /// <value>The AUA code.</value>
        public string AuaCode { get; set; }

        /// <summary>
        /// Gets or sets the Sub-AUA code.
        /// </summary>
        /// <value>The Sub-AUA code.</value>
        public string SubAuaCode { get; set; }

        /// <summary>
        /// Gets or sets a collection of host URI assigned to AUA for using UIDAI services.
        /// </summary>
        /// <value>A collection of host URI assigned to AUA for using UIDAI services.</value>
        public Dictionary<string, Uri> Hosts { get; set; }
    }
}