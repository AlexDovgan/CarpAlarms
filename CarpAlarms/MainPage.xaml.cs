using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace CarpAlarms
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            
            RedRod.DataContext = new RodViewModel("RedRod", new SolidColorBrush(Colors.Red));
            YellowRod.DataContext = new RodViewModel("YellowRod", new SolidColorBrush(Colors.Yellow));
            GreenRod.DataContext = new RodViewModel("GreenRod", new SolidColorBrush(Colors.Green));
            BlueRod.DataContext = new RodViewModel("BlueRod", new SolidColorBrush(Colors.Blue));

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("rod"))
            {
                string rodName= NavigationContext.QueryString["rod"];
                object sp=this.FindName(rodName);
                if(sp!=null && sp is StackPanel)
                {
                    RodViewModel rvm = (RodViewModel)((StackPanel)sp).DataContext;
                    rvm.OnRodExpired();

                }

                // etc ...
            }
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }
    }
}
