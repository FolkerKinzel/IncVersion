using Microsoft.VisualStudio.TestTools.UnitTesting;
using IncVersion;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace IncVersion.Tests
{
    [TestClass()]
    public class AssemblyFileVersionIncrementerTests
    {
        public TestContext TestContext { get; set; }


        [TestMethod()]
        public void IncrementAssemblyFileVersionTest()
        {
            string fileName = Path.Combine(Directory.GetParent(TestContext.TestRunDirectory).FullName, "Test.csproj");

            var incrementer = new AssemblyFileVersionIncrementer();

          

            incrementer.IncrementAssemblyFileVersion(fileName);

        }
    }
}
