using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace fivenine.UnifiedMaps.Windows
{
    public sealed class InfoWindow : Control
    {
        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof (string), typeof (InfoWindow), new PropertyMetadata(string.Empty));

        public string Snippet
        {
            get { return (string) GetValue(SnippetProperty); }
            set { SetValue(SnippetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Snippet.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SnippetProperty =
            DependencyProperty.Register("Snippet", typeof (string), typeof (InfoWindow), new PropertyMetadata(string.Empty));

        public ICommand SelectedCommand
        {
            get { return (ICommand) GetValue(SelectedCommandProperty); }
            set { SetValue(SelectedCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedCommandProperty =
            DependencyProperty.Register("SelectedCommand", typeof (ICommand), typeof (InfoWindow), new PropertyMetadata(null));

        public InfoWindow()
        {
            this.DefaultStyleKey = typeof (InfoWindow);
        }
    }
}
