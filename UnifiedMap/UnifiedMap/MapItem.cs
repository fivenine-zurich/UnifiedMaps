using Xamarin.Forms;

namespace fivenine.UnifiedMaps
{
    /// <summary>
    /// Defineas a map item.
    /// </summary>
    public abstract class MapItem : BindableObject
    {
        internal object Id { get; set; }
    }
}