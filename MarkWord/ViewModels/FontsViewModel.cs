using System.Collections.ObjectModel;

namespace MarkWord.ViewModels
{
    using System.Linq;

    public class FontsViewModel : ViewModel
    {
        public ObservableCollection<string> FontsData
        {
            get
            {
                return new ObservableCollection<string>(System.Windows.Media.Fonts.SystemFontFamilies.Select(fontFamily => fontFamily.ToString()));

            }
        }
    }
}
