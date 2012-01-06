using System;
using System.Windows;
using System.Windows.Controls;

namespace Octgn.Launcher
{
    public sealed partial class MainMenu : Page
    {
        private Boolean canOpenDeckEditor = true;

        public MainMenu()
        {
            InitializeComponent();
            versionText.Text = string.Format("Version {0}", OctgnApp.OctgnVersion.ToString(4));
            tbRelease.Text = "Release " + Octgn.Properties.Settings.Default.currevision;
            MessageBoxResult br = MessageBox.Show(
                "Octgn w/ Lobby is dead. A bunch of rabid dogs ate the source code.\n Visit http://www.skylabsonline.com/octgn to download Octgn 3.0(with built in lobby), or click 'Yes' to go there automatically. That's about it.","OMG new Octgn",MessageBoxButton.YesNo);
            if(br == MessageBoxResult.Yes)
            {
                OpenLink("http://www.skylabsonline.com/octgn");          
            }
            Application.Current.MainWindow.Close();
                Application.Current.Shutdown(0);
        }
        public void OpenLink(string sUrl)
        {
            try
            {
                System.Diagnostics.Process.Start(sUrl);
            }
            catch (Exception exc1)
            {
                // System.ComponentModel.Win32Exception is a known exception that occurs when Firefox is default browser.  
                // It actually opens the browser but STILL throws this exception so we can just ignore it.  If not this exception,
                // then attempt to open the URL in IE instead.
                if (exc1.GetType().ToString() != "System.ComponentModel.Win32Exception")
                {
                    // sometimes throws exception so we have to just ignore
                    // this is a common .NET bug that no one online really has a great reason for so now we just need to try to open
                    // the URL using IE if we can.
                    try
                    {
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("IExplore.exe", sUrl);
                        System.Diagnostics.Process.Start(startInfo);
                        startInfo = null;
                    }
                    catch (Exception exc2)
                    {
                        // still nothing we can do so just show the error to the user here.
                    }
                }
            }
        }
        private void StartGame(object sender, RoutedEventArgs e)
        { NavigationService.Navigate(new Serve()); }

        private void JoinGame(object sender, RoutedEventArgs e)
        { NavigationService.Navigate(new Join()); }

        private void ManageGames(object sender, RoutedEventArgs e)
        { if (canOpenDeckEditor)NavigationService.Navigate(new GameManager()); }

        private void EditDecks(object sender, RoutedEventArgs e)
        {
            if (canOpenDeckEditor)
            {
                if (Program.GamesRepository.Games.Count == 0)
                {
                    MessageBox.Show("You have no game installed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                canOpenDeckEditor = false;
                Program.lwDeck = new DeckBuilder.DeckBuilderWindow();
                Program.lwDeck.Show();
                Program.lwDeck.Closing += new System.ComponentModel.CancelEventHandler(lwDeck_Closing);
                Program.lwMainWindow.Hide();

                Application.Current.MainWindow = Program.lwDeck;

                //launcherWnd.Close();
            }
        }

        private void lwDeck_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            canOpenDeckEditor = true;
        }

        private void QuitClicked(object sender, RoutedEventArgs e)
        {
            if (Program.lwLobbyWindow == null)
            {
                Application.Current.MainWindow.Close();
                Application.Current.Shutdown(0);
            }
            else
            {
                if (Program.lwLobbyWindow.IsVisible == false)
                {
                    Program.lwLobbyWindow = new Octgn.Lobby.LobbyWindow();
                    Program.lwLobbyWindow.Show();
                    Program.lwLobbyWindow.Navigate(new Octgn.Lobby.ServerConnect());
                }
                else
                    MessageBox.Show("You must exit the lobby system first.");
            }
        }

        #region Hint texts

        private void EnterBtn(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            hintText.Text = (string)btn.Tag;
        }

        private void LeaveBtn(object sender, RoutedEventArgs e)
        { hintText.Text = ""; }

        #endregion Hint texts

        private void LobbyClicked(object sender, RoutedEventArgs e)
        {
            if (Program.lwLobbyWindow == null)
            {
                Program.lwLobbyWindow = new Octgn.Lobby.LobbyWindow();
                Program.lwLobbyWindow.Show();
                Program.lwLobbyWindow.Navigate(new Octgn.Lobby.ServerConnect());
            }
            else if (Program.lwLobbyWindow.IsVisible == false)
            {
                Program.lwLobbyWindow = new Octgn.Lobby.LobbyWindow();
                Program.lwLobbyWindow.Show();
                Program.lwLobbyWindow.Navigate(new Octgn.Lobby.ServerConnect());
            }
        }
    }
}