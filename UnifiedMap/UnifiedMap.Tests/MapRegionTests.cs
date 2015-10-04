using System.Collections;
using fivenine.UnifiedMaps;
using FluentAssertions;
using NUnit.Framework;

namespace UnifiedMap.Tests
{
    [TestFixture]
    public class MapRegionTests
    {
        private class CoordinateIncludeCases : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new object[] {new MapRegion(-10, 10, 100, -10), new Position(5, 90)};
                yield return new object[] {new MapRegion(-100, 10, 10, -10), new Position(5, -90)};
                yield return new object[] {new MapRegion(-10, 100, 10, -10), new Position(90, 5)};
                yield return new object[] {new MapRegion(-10, 10, 10, -100), new Position(-90, 5)};
            }
        }

        private class CoordinateExcludeCases : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new object[] {new MapRegion(-10, 10, 10, -10), new Position(20, 0)};
                yield return new object[] {new MapRegion(-10, 10, 10, -10), new Position(-20, 0)};
                yield return new object[] {new MapRegion(-10, 10, 10, -10), new Position(0, 20)};
                yield return new object[] {new MapRegion(-10, 10, 10, -10), new Position(0, -20)};
                yield return new object[] {new MapRegion(-10, 10, 10, -10), new Position(20, -20)};
                yield return new object[] {new MapRegion(-10, 10, 10, -10), new Position(-20, 20)};
            }
        }

        [TestCase(-10, 20, 10, -20)]
        [TestCase(10, 20, -10, -20)]
        [TestCase(-10, -20, 10, 20)]
        [TestCase(10, -20, -10, 20)]
        public void Constructor_must_rearange_given_coordinates_if_required(double minX, double maxY, double maxX, double minY)
        {
            var region = new MapRegion(minX, maxY, maxX, minY);

            region.MinX.Should().Be(-10);
            region.MaxX.Should().Be(10);
            region.MinY.Should().Be(-20);
            region.MaxY.Should().Be(20);
        }

        [Test]
        public void Clone_must_create_an_exact_copy()
        {
            var region = new MapRegion(10, 20, 30, 40);
            var copy = region.Clone();

            copy.Should().NotBeSameAs(region);
            copy.Should().Be(region);
        }

        [TestCaseSource(typeof(CoordinateIncludeCases))]
        public void Include_with_included_coordinate_must_not_extend_the_region(MapRegion originalRegion, Position position)
        {
            var region = originalRegion.Clone();
            region.Include(position);

            region.Should().Be(originalRegion);
        }

        [TestCaseSource(typeof(CoordinateExcludeCases))]
        public void Include_with_excluded_coordinate_must_extend_the_region(MapRegion originalRegion, Position position)
        {
            var region = originalRegion.Clone();
            region.Include(position);

            region.Should().NotBe(originalRegion);

            region.Contains(position).Should().BeTrue("the new region should containt the included position now");
            originalRegion.Contains(position).Should().BeFalse("the old region should not include the position");
        }
    }
}
