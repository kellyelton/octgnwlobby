using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Octgn.Properties;
using System.Collections;
using System.Reflection;
using System.Threading;

namespace Octgn.Launcher
{
	public partial class GameManager : Page
	{
		public GameManager()
    {
			InitializeComponent();
			Loaded += delegate
			{
				// FIX: those two lines make sure any BitmapDecoder, which may lock the set file
				// (e.g. after playing a game or browsing a deck), releases it
				GC.Collect(GC.MaxGeneration);
				GC.WaitForPendingFinalizers();
			};
		}

		protected void InstallGame(object sender, RoutedEventArgs e)
		{
      if (!Settings.Default.DontShowInstallNotice)
        new InstallNoticeDialog { Owner = Application.Current.MainWindow }.ShowDialog();

			// Get the definition file
			Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
			ofd.Filter = "Game definition files (*.o8g)|*.o8g";
			if (ofd.ShowDialog() != true) return;			

			// Open the archive
			Definitions.GameDef game = Definitions.GameDef.FromO8G(ofd.FileName);
			if (!game.CheckVersion()) return;

			// Check if the game already exists
			if (Program.GamesRepository.Games.Any(g => g.Id == game.Id))
				if (MessageBox.Show("This game already exists.\r\nDo you want to overwrite it?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) != MessageBoxResult.Yes)
					return;
				
			var gameData = new Data.Game()
			{
				Id = game.Id, Name = game.Name, 
        Filename = ofd.FileName, Version = game.Version,
				CardWidth = game.CardDefinition.Width, CardHeight = game.CardDefinition.Height,
				CardBack = game.CardDefinition.back,
				DeckSections = game.DeckDefinition.Sections.Keys,
        SharedDeckSections = game.SharedDeckDefinition == null ? null : game.SharedDeckDefinition.Sections.Keys
			};
			Program.GamesRepository.InstallGame(gameData, game.CardDefinition.Properties.Values);
		}

		private void InstallCards(object sender, RoutedEventArgs e)
		{
      if (!Settings.Default.DontShowInstallNotice)
        new InstallNoticeDialog { Owner = Application.Current.MainWindow }.ShowDialog();

      // Open the DB
      Data.Game gameDb = (Data.Game)gamesList.SelectedItem;
      if (gameDb == null)
      {
        MessageBox.Show("Select a game in the list first");
        return;
      }

      // Get the patch file
      var ofd = new Microsoft.Win32.OpenFileDialog
      {
        Filter = "Cards set definition files (*.o8s)|*.o8s",
        Multiselect = true
      };
			if (ofd.ShowDialog() != true) return;

      var wnd = new InstallSetsProgressDialog { Owner = Application.Current.MainWindow };
      ThreadPool.QueueUserWorkItem(_ =>
        {
          int current = 0, max = ofd.FileNames.Length;
          wnd.UpdateProgress(current, max, null, false);
          foreach (string setName in ofd.FileNames)
          {
            ++current;
            string shortName = System.IO.Path.GetFileName(setName);
            try
            {
              gameDb.InstallSet(setName);
              wnd.UpdateProgress(current, max, string.Format("'{0}' installed.", shortName), false);
            }
            catch (Exception ex)
            {
              wnd.UpdateProgress(current, max, string.Format("'{0}' an error occured during installation:", shortName), true);
              wnd.UpdateProgress(current, max, ex.Message, true);
            }
          }
        });
      wnd.ShowDialog();
		}

		private void DeleteSet(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			var game = (Data.Game)gamesList.SelectedItem;
			if (game == null) return;
			Data.Set set = (Data.Set)setsList.SelectedItem;
			if (set == null) return;
			game.DeleteSet(set);			
		}

    private void PatchSets(object sender, RoutedEventArgs e)
    {
      new PatchDialog { Owner = Application.Current.MainWindow }.ShowDialog();
    }
	}

  public class DispatchedCollectionView : ListCollectionView
  {
    public DispatchedCollectionView(IList enumerable) : base(enumerable)
    {
      typeof(CollectionView).GetMethod("SetFlag", BindingFlags.Instance | BindingFlags.NonPublic)
                            .Invoke(this, new object[] { 0x100, true });  // 0x100 = IsMultiThreadCollectionChangeAllowed
    }
  }
}