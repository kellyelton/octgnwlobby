﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace Octgn.Launcher
{
    public partial class LauncherWindow : NavigationWindow
    {
        public Boolean OkToClose { get; set; }

        private static readonly Duration TransitionDuration = new Duration(TimeSpan.FromMilliseconds(500));
        private readonly AnimationTimeline OutAnimation = new DoubleAnimation(0, TransitionDuration);
        private readonly AnimationTimeline InAnimation = new DoubleAnimation(0, 1, TransitionDuration);

        private static readonly object BackTarget = new object();
        private bool isInTransition = false;
        private object transitionTarget;

        public LauncherWindow()
        {
            InitializeComponent();
            OkToClose = false;
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
            //TODO Lobby added this

            Navigating += delegate(object sender, NavigatingCancelEventArgs e)
            {
                // FIX (jods): prevent further navigation when a navigation is already in progress
                //						 (e.g. double-click a button in the main menu). This would break the transitions.
                if (isInTransition)
                { e.Cancel = true; return; }

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

        private void NavigationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Program.lwLobbyWindow != null)
            {
                if (Program.lwLobbyWindow.IsVisible == true)
                    e.Cancel = true;
            }
        }

        private void NavigationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Program.lwMainWindow = this as Launcher.LauncherWindow;
            Application.Current.MainWindow = Program.lwMainWindow;
        }
    }
}