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

using System.Xml.Linq;

namespace Uidai.Aadhaar.Security
{
    /// <summary>
    /// Provides an interface to sign XML.
    /// </summary>
    public interface ISigner
    {
        /// <summary>
        /// Computes digital signature and adds it to an XML element.
        /// </summary>
        /// <param name="xml">The XML on which the signature is to be computed.</param>
        /// <returns>The Signature element of the XML.</returns>
        XElement ComputeSignature(XElement xml);
    }
}