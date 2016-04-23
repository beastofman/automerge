using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyersDiff;
using Sources.String;
using System.Collections.Generic;
using System.Linq;

namespace MyersDiffTest
{
    [TestClass]
    public class MyersDiffUnitTest
    {
        private readonly MyersDiffEngine m_diffEngine = new MyersDiffEngine();

        [TestMethod]
        public void TestBothEmptySources()
        {
            var diffEngine = new MyersDiffEngine();
            var operations = diffEngine.GetDiff(null, null);

            Assert.IsNull(operations);
        }

        [TestMethod]
        public void TestDiff()
        {
            var source1 = new StringSource("ABCABBA");
            var source2 = new StringSource("CBABAC");

            var operationsControl = new List<Operation>
            {
                new Operation(OperationKind.Delete, 0, source1),
                new Operation(OperationKind.Delete, 1, source1),
                new Operation(OperationKind.Equal, 2, source1),
                new Operation(OperationKind.Insert, 1, source2),
                new Operation(OperationKind.Equal, 3, source1),
                new Operation(OperationKind.Equal, 4, source1),
                new Operation(OperationKind.Delete, 5, source1),
                new Operation(OperationKind.Equal, 6, source1),
                new Operation(OperationKind.Insert, 5, source2)
            };

            var operations = this.m_diffEngine.GetDiff(source1, source2).ToList();
            Assert.IsNotNull(operations);

            var equal = operationsControl.Count == operations.Count;
            Assert.IsTrue(equal);

            for (var i = 0; i < operationsControl.Count; i++)
            {
                var op = operations[i];
                Assert.IsTrue(op != null && Equals(op, operationsControl[i]));
            }
        }

        [TestMethod]
        public void TestSingleEmptySource()
        {
            var source1 = new StringSource("A");
            var operationsControl = new List<Operation>
            {
                new Operation(OperationKind.Insert, 0, source1)
            };

            var operations = this.m_diffEngine.GetDiff(source1, null).ToList();
            Assert.IsNotNull(operations);

            var equal = operationsControl.Count == operations.Count;
            Assert.IsTrue(equal);

            for (var i = 0; i < operationsControl.Count; i++)
            {
                var op = operations[i];
                equal = op != null && Equals(op, operationsControl[i]);
                Assert.IsTrue(equal);
            }
        }
    }
}