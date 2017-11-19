using NUnit.Framework;
using fivenine.UnifiedMaps;
using System.Collections.Generic;
using FluentAssertions;

namespace UnifiedMap.Tests
{
    [TestFixture]
    public class PositionTests
    {
        private static IEnumerable<TestCaseData> AddCases()
        {
            yield return new TestCaseData(new Position(1, 1), new Position(5, 5), 629060.759879635);
            yield return new TestCaseData(new Position(47.384636158315, 8.53182792663574), new Position(47.38707687595, 8.57465744), 3239.46);
        }

        [Test, TestCaseSource("AddCases")]
        public void GetDistanceTo_returns_the_expected_results(Position a, Position b, double expected)
        {
            var distance = a.GetDistanceTo(b);
            var delta = distance - expected;
            delta.Should().BeLessOrEqualTo(1e-8);
        }

        [Test]
        public void Equals_returns_true_for_the_same_positions() {
            Position a = new Position(1, 2);
            Position b = new Position(1, 2);

            a.Equals(b).Should().BeTrue();
        }

        [Test]
        public void Equals_returns_false_for_different_positions()
        {
            Position a = new Position(1, 2);
            Position b = new Position(2, 1);

            a.Equals(b).Should().BeFalse();
        }
    }
}
