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
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Uidai.Aadhaar.Resident;
using Xunit;

namespace Uidai.AadhaarTests.Resident
{
    public class BestFingerInfoTest
    {
        [Fact]
        public void ToXmlTest()
        {
            var bestFingerInfo = Data.BestFingerInfo;
            var xml = XElement.Parse(File.ReadAllText(Data.BestFingerInfoXml)).Elements().ToArray();

            // Set: All
            Assert.True(XNode.DeepEquals(xml[0], bestFingerInfo.ToXml("Rbd")));

            // Remove: Biometrics
            bestFingerInfo.Fingers.Clear();
            Assert.Throws<ArgumentException>(nameof(BestFingerInfo.Fingers), () => bestFingerInfo.ToXml("Rbd"));
        }
    }
}