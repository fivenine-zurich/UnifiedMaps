using System.Collections;
using fivenine.UnifiedMaps.Droid;
using NUnit.Framework;
using Xamarin.Forms;

namespace UnifiedMaps.Droid.Tests
{
    [TestFixture]
    public class GoogleMapsExtensionsTests
    {
        private class ColorCases : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new object[] {Color.Red, 0};
                yield return new object[] {Color.Green, 120};
                yield return new object[] {Color.Blue, 240};
                yield return new object[] {Color.Yellow, 60};
                yield return new object[] {Color.FromHex("#007FFF"), 210}; // Azure
                yield return new object[] {Color.FromHex("#00FFFF"), 180}; // Cyan
                yield return new object[] {Color.FromHex("#FF00FF"), 300}; // Magenta
                yield return new object[] {Color.FromHex("#FF7F00"), 29};  // Orange (pecission issue, should be 30)
                yield return new object[] {Color.FromHex("#FF007F"), 330}; // Rose
                yield return new object[] {Color.FromHex("#7F00FF"), 269}; // Violet (pecission issue, should be 270)
            }
        }

        [TestCaseSource(typeof (ColorCases))]
        public void ToMarkerHue_returns_the_matching_android_hue_value(Color color, int expectedHue)
        {
            Assert.AreEqual(expectedHue, (int) color.ToMarkerHue());
        }
    }
}