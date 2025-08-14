using System.Windows.Controls;

namespace SmartICAVI.Animation
{
    public static class ExtensionMethods
    {
        public static void NavigateToExample(this Frame exampleContainer, Page page)
        {
            var navigation = new NavigationAnimator();
            navigation.Navigate(exampleContainer, page);
        }
    }
}
