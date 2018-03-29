using System.Collections.ObjectModel;

namespace Cinary.Xamarin.Reactive
{
    public class ListGroup : ObservableCollection<ObservableCollection<object>>
    {
        public object Key { get; private set; }

        public ListGroup(object key) => Key = key;
    }
}
