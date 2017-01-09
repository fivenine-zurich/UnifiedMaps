// ReSharper disable once CheckNamespace

using fivenine.UnifiedMaps.iOS;
using Foundation;

namespace fivenine
{
    [Preserve(AllMembers = true)]
    public static class UnifiedMap
    {
        public static void Init()
        {
            var renderer = new UnifiedMapRenderer();
        }
    }
}
