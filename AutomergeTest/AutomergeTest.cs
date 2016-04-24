using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyersDiff;
using Sources.String;
using System.Collections.Generic;
using System.Linq;

namespace AutomergeTest
{
    [TestClass]
    public class AutomergeTest
    {
        private readonly MyersDiffEngine m_diffEngine = new MyersDiffEngine();

        [TestMethod]
        public void TestAutomerge()
        {
            var source = new StringSource("ABCABBA");
            var target1 = new StringSource("CBABAC");
            var target2 = new StringSource("ABCABBAD");

            var finalControl = new List<Operation>
            {
                new Operation(OperationKind.Delete, 0, source),
                new Operation(OperationKind.Delete, 1, source),
                new Operation(OperationKind.Equal, 2, source),
                new Operation(OperationKind.Insert, 1, target1),
                new Operation(OperationKind.Equal, 3, source),
                new Operation(OperationKind.Equal, 4, source),
                new Operation(OperationKind.Delete, 5, source),
                new Operation(OperationKind.Insert, 7, target2),
                new Operation(OperationKind.Insert, 5, target1)
            };

            var sourceTarget1 = this.m_diffEngine.GetDiff(source, target1);
            var sourceTarget2 = this.m_diffEngine.GetDiff(source, target2);

            Assert.IsNotNull(sourceTarget1);
            Assert.IsNotNull(sourceTarget2);

            var merge = new MergeEngine(ResolveStrategyAction.AcceptFirst, false);
            var final = merge.GetMergeOperations(sourceTarget1.ToList(), sourceTarget2.ToList()).ToList();

            Assert.IsNotNull(final);

            var equal = finalControl.Count == final.Count;
            Assert.IsTrue(equal);

            for (var i = 0; i < finalControl.Count; i++)
            {
                var op = final[i];
                Assert.IsTrue(op != null && Equals(op, finalControl[i]));
            }
        }

        [TestMethod]
        public void TestAutomergeAcceptFirstConflict()
        {
            var source = new StringSource("ABC");
            var target1 = new StringSource("ABD");
            var target2 = new StringSource("ABR");

            var finalControl = new List<Operation>
            {
                new Operation(OperationKind.Equal, 0, source),
                new Operation(OperationKind.Equal, 1, source),
                new Operation(OperationKind.Delete, 2, source),
                new Operation(OperationKind.Insert, 2, target1, true),
            };

            var sourceTarget1 = this.m_diffEngine.GetDiff(source, target1);
            var sourceTarget2 = this.m_diffEngine.GetDiff(source, target2);

            Assert.IsNotNull(sourceTarget1);
            Assert.IsNotNull(sourceTarget2);

            var merge = new MergeEngine(ResolveStrategyAction.AcceptFirst, true);

            merge.ConflictOperationsResolveStrategies.Add(OperationKind.Equal, new ResolveStrategy(ResolveStrategyAction.AcceptFirst, false));
            merge.ConflictOperationsResolveStrategies.Add(OperationKind.Delete, new ResolveStrategy(ResolveStrategyAction.AcceptFirst, false));

            var final = merge.GetMergeOperations(sourceTarget1.ToList(), sourceTarget2.ToList()).ToList();

            Assert.IsNotNull(final);

            var equal = finalControl.Count == final.Count;
            Assert.IsTrue(equal);

            for (var i = 0; i < finalControl.Count; i++)
            {
                var op = final[i];
                Assert.IsTrue(op != null && Equals(op, finalControl[i]));
            }
        }
    }
}