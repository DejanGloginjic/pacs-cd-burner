using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CDBurner.Behavior
{
    public static class ListViewScrollBehavior
    {
        public static readonly DependencyProperty ScrollToTopProperty =
            DependencyProperty.RegisterAttached(
                "ScrollToTop",
                typeof(int),
                typeof(ListViewScrollBehavior),
                new PropertyMetadata(0, OnScrollToTopChanged));

        public static void SetScrollToTop(DependencyObject element, int value)
            => element.SetValue(ScrollToTopProperty, value);

        public static int GetScrollToTop(DependencyObject element)
            => (int)element.GetValue(ScrollToTopProperty);

        private static void OnScrollToTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListView listView)
            {
                listView.Dispatcher.BeginInvoke(() =>
                {
                    if (listView.Items.Count > 0)
                        listView.ScrollIntoView(listView.Items[0]);
                });
            }
        }
    }
}
