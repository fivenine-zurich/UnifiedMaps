using Xamarin.Forms;
using WinRt = Windows.UI;

namespace fivenine.UnifiedMaps.Windows
{
    public static class WinRtExtensions
    {
        public static WinRt.Color ToWinRT(this Color color)
        {
            return WinRt.Color.FromArgb(
                (byte) (color.A*255),
                (byte) (color.R*255),
                (byte) (color.G*255),
                (byte) (color.B*255));
        }
    }
}
