using System;
using System.Collections.Generic;
using Lykke.AlgoStore.Core.Utils;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class ExtensionTests
    {
        #region Data Generation

        public class AccuracyTestData
        {
            public double Number { get; set; }
            public int Accuracy { get; set; }
        }

        private static readonly Random Rnd = new Random();
        private static IEnumerable<object[]> RandomAccuracyData
        {
            get
            {
                var result = new List<object[]>();

                for (int i = 0; i < 20; i++)
                {
                    int num1 = Rnd.Next();
                    if (num1.ToString().EndsWith('0'))
                        num1 = num1 + 1;
                    int acc = Rnd.Next(1, 9);
                    double res = (double)num1 / Math.Pow(10, acc);

                    result.Add(new object[] { new AccuracyTestData{Accuracy = acc, Number = res} });
                }
                return result;
            }
        }
        #endregion

        [TestCaseSource("RandomAccuracyData")]
        public void GetAccuracy_Test(AccuracyTestData data)
        {
            var res = data.Number.GetAccuracy();
            Assert.AreEqual(res, data.Accuracy);
        }
    }
}
