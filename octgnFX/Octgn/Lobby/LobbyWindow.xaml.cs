using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace Octgn.Lobby
{
    public partial class LobbyWindow : NavigationWindow
    {
        private static readonly Duration TransitionDuration = new Duration(TimeSpan.FromMilliseconds(500));
        private readonly AnimationTimeline OutAnimation = new DoubleAnimation(0, TransitionDuration);
        private readonly AnimationTimeline InAnimation = new DoubleAnimation(0, 1, TransitionDuration);

        private static readonly object BackTarget = new object();
        public bool isInTransition = false;
        private object transitionTarget;

        public LobbyWindow()
        {
            InitializeComponent();
            okToCloseMainWindow = false;
            NavigationCommands.BrowseBack.InputGestures.Clear();

            OutAnimation.Completed += delegate
            {
                isInTransition = false;
                if (transitionTarget == BackTarget)
                    GoBack();
                else
                    Navigate(transitionTarget);
            };
            OutAnimation.Freeze();

            Navigating += delegate(object sender, NavigatingCancelEventArgs e)
            {
                // FIX (jods): prevent further navigation when a navigation is already in progress
                //						 (e.g. double-click a button in the main menu). This would break the transitions.
                if (isInTransition)
                { e.Cancel = true; return; }
                if (e.Uri != null)
                {
                    try
                    {
                        String strUrlRegex = "(?i)\\b((?:[a-z][\\w-]+:(?:/{1,3}|[a-z0-9%])|www\\d{0,3}[.]|[a-z0-9.\\-]+[.][a-z]{2,4}/)(?:[^\\s()<>]+|\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\))+(?:\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\)|[^\\s`!()\\[\\]{};:'\".,<>?«»“”‘’]))";
                        Regex r = new Regex(strUrlRegex);
                        Match m = r.Match(e.Uri.OriginalString);
                        if (m.Groups.Count >= 2)
                        {
                            Process.Start(new ProcessStartInfo(m.Groups[1].Value));
                        }
                    }
                    catch { }
                    e.Cancel = true;
                    return;
                    //NavigationService.GoBack();
                    //NavigationService.StopLoading();
                }
                if (transitionTarget != null)
                {
                    transitionTarget = null;
                    return;
                }

                var page = Content as Page;
                if (page == null) return;

                e.Cancel = true;
                isInTransition = true;
                if (e.NavigationMode == NavigationMode.Back)
                    transitionTarget = BackTarget;
                else
                    transitionTarget = e.Content;
                page.BeginAnimation(UIElement.OpacityProperty, OutAnimation, HandoffBehavior.SnapshotAndReplace);
            };

            Navigated += delegate
            {
                var page = Content as Page;
                if (page == null) return;

                page.BeginAnimation(UIElement.OpacityProperty, InAnimation);
            };
        }

        public bool okToCloseMainWindow { get; set; }

        private void NavigationWindow_Closed(object sender, EventArgs e)
        {
        }
    }
}