using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Reflection;

namespace Octgn.Launcher
{
	public sealed partial class MainMenu : Page
	{
		public MainMenu()
		{
			InitializeComponent();
			versionText.Text = string.Format("Version {0}", OctgnApp.OctgnVersion.ToString(4));
            Program.lwMainWindow = (Launcher.LauncherWindow)Application.Current.MainWindow;
		}

		private void StartGame(object sender, RoutedEventArgs e)
		{ NavigationService.Navigate(new Serve(false)); }

		private void JoinGame(object sender, RoutedEventArgs e)
		{ NavigationService.Navigate(new Join()); }

		private void ManageGames(object sender, RoutedEventArgs e)
		{ NavigationService.Navigate(new GameManager()); }

		private void EditDecks(object sender, RoutedEventArgs e)
		{
			if (Program.GamesRepository.Games.Count == 0)
			{
				MessageBox.Show("You have no game installed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			var launcherWnd = Application.Current.MainWindow;
			var deckWnd = new DeckBuilder.DeckBuilderWindow();
			deckWnd.Show();
			Application.Current.MainWindow = deckWnd;
            Program.lwMainWindow.isOkToClose = true;
			launcherWnd.Close();			
		}

		private void QuitClicked(object sender, RoutedEventArgs e)
		{
            if (Program.lwLobbyWindow == null)
            {
                Program.lwMainWindow.isOkToClose = true;
                Application.Current.MainWindow.Close();
                Application.Current.Shutdown(0);
            }
            else
            {
                if (Program.lwLobbyWindow.IsVisible == false)
                {
                    Program.lwLobbyWindow = new LauncherWindow();
                    Program.lwLobbyWindow.Show();
                    Program.lwLobbyWindow.Navigate(new ServerConnect());
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

		#endregion

        private void LobbyClicked(object sender, RoutedEventArgs e)
        {
            if (Program.lwLobbyWindow == null)
            {
                Program.lwLobbyWindow = new LauncherWindow();
                Program.lwLobbyWindow.Show();
                Program.lwLobbyWindow.Navigate(new ServerConnect());
            }
            else if (Program.lwLobbyWindow.IsVisible == false)
            {
                Program.lwLobbyWindow = new LauncherWindow();
                Program.lwLobbyWindow.Show();
                Program.lwLobbyWindow.Navigate(new ServerConnect());
            }
        }
	}
}