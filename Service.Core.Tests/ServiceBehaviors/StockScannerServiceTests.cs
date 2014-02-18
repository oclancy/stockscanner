using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockService;
using StockService.Core;

namespace Service.Core.Tests.ServiceBehaviors
{
    [TestClass]
    public class StockScannerServiceTests
    {
        [TestMethod]
        public void GetDividends()
        {
            var service = new StockScannerService();

            var res = service.GetDividends(new Sector() { Industries = new List<Industry>() { { new Industry() { IndustryId = 1 } } } });
        }
    }
}
