using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockService.Core;

namespace Service.Core.Tests.Data
{
    [TestClass]
    public class CsvReadLineTests
    {
        [TestMethod]
        public void CsvReadLine()
        {
            var line = "one,two,three";

            var parts = LseDataReader.ReadCsvLine(line);

            Assert.AreEqual(3, parts.Count());
        }

        [TestMethod]
        public void CsvReadLine2()
        {
            var line = ",,one,two,three";

            var parts = LseDataReader.ReadCsvLine(line);

            Assert.AreEqual(5, parts.Count());
        }

        [TestMethod]
        public void CsvReadLine3()
        {
            var line = "\"one\",two,three";

            var parts = LseDataReader.ReadCsvLine(line);

            Assert.AreEqual(3, parts.Count());
        }

        [TestMethod]
        public void CsvReadLine4()
        {
            var line = "\"one,two,two\",two,three";

            var parts = LseDataReader.ReadCsvLine(line);

            Assert.AreEqual(3, parts.Count());
        }

        [TestMethod]
        public void CsvReadLine5()
        {
            var line = "f,g,h\"d,f,g\"sss,";

            var parts = LseDataReader.ReadCsvLine(line);

            Assert.AreEqual(4, parts.Count());
        }
    }
}
