using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SpotifyAPI.Models
{
    public class NavigationSingleton
    {
        private static NavigationSingleton navigation;
        private Frame frame;
        private Page _currentPage;
        private Page _lastPage;

        private NavigationSingleton()
        {

        }
        public static NavigationSingleton GetNavigationService()
        {
            if (navigation == null)
            {
                navigation = new NavigationSingleton();
            }
            return navigation;
        }

        public void NavigateToPage(Page nextPage)
        {
            _lastPage = _currentPage;
            _currentPage = nextPage;
            frame.NavigationService.Navigate(nextPage);
            frame.NavigationService.RemoveBackEntry();
        }
        public void NavigateBack()
        {
            frame.NavigationService.GoBack();
        }
        public void SetCurrentPage(Page currentPage)
        {
            _currentPage = currentPage;
        }

        public static NavigationSingleton CreateNavigationService(Frame frame)
        {
            NavigationSingleton x = GetNavigationService();
            x.frame = frame;
            return x;
        }
    }
}
