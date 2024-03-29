﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Octgn.Data;
using Octgn.Lobby;

namespace Octgn.DeckBuilder
{
    public partial class DeckBuilderWindow : Window, INotifyPropertyChanged
    {
        private string deckFilename = null;
        private bool unsaved = false;

        private Deck _deck;

        public Deck Deck
        {
            get { return _deck; }
            set
            {
                if (_deck == value) return;
                _deck = value;
                unsaved = false;
                ActiveSection = value.Sections.FirstOrDefault();
                OnPropertyChanged("Deck");
            }
        }

        private Data.Game _game;

        private Data.Game Game
        {
            get { return _game; }
            set
            {
                if (_game == value) return;

                if (_game != null)
                    _game.CloseDatabase();

                _game = value;
                ActiveSection = null;

                if (value != null)
                {
                    value.OpenDatabase(true);
                    cardImage.Source = new BitmapImage(value.GetCardBackUri());

                    Searches.Clear();
                    AddSearchTab();
                }
            }
        }

        private Deck.Section _section;

        public Deck.Section ActiveSection
        {
            get { return _section; }
            set
            {
                if (_section == value) return;
                _section = value;
                OnPropertyChanged("ActiveSection");
            }
        }

        public ObservableCollection<SearchControl> Searches
        { get; private set; }

        public DeckBuilderWindow()
        {
            Searches = new ObservableCollection<SearchControl>();
            InitializeComponent();
            // If there's only one game in the repository, create a deck of the correct kind
            if (Program.GamesRepository.Games.Count == 1)
            {
                Game = Program.GamesRepository.Games[0];
                Deck = new Deck(Game);
                deckFilename = null;
            }
        }

        #region Search tabs

        private void OpenTabCommand(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            AddSearchTab();
            CommandManager.InvalidateRequerySuggested();
        }

        private void CloseTabCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var search = (SearchControl)((FrameworkElement)e.OriginalSource).DataContext;
            Searches.Remove(search);
            CommandManager.InvalidateRequerySuggested();
        }

        private void CanCloseTab(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Searches.Count > 1;
        }

        private void AddSearchTab()
        {
            var ctrl = new SearchControl(Game) { SearchIndex = Searches.Count == 0 ? 1 : Searches.Max(x => x.SearchIndex) + 1 };
            ctrl.CardAdded += AddResultCard;
            ctrl.CardRemoved += RemoveResultCard;
            ctrl.CardSelected += CardSelected;
            Searches.Add(ctrl);
            searchTabs.SelectedIndex = Searches.Count - 1;
        }

        #endregion Search tabs

        private void NewDeckCommand(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (Game == null)
            {
                if (Program.GamesRepository.Games.Count == 1)
                    Game = Program.GamesRepository.Games[0];
                else
                {
                    MessageBox.Show("You have to select a game before you can use this command.", "Error", MessageBoxButton.OK);
                    return;
                }
            }
            Deck = new Deck(Game);
            deckFilename = null;
        }

        private void NewClicked(object sender, RoutedEventArgs e)
        {
            Game = (Data.Game)((MenuItem)e.OriginalSource).DataContext;
            CommandManager.InvalidateRequerySuggested();
            Deck = new Deck(Game);
            deckFilename = null;
        }

        private void SaveDeck(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Save();
        }

        private void SaveDeckAsHandler(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            SaveAs();
        }

        private void Save()
        {
            if (deckFilename == null)
            {
                SaveAs();
                return;
            }
            try
            {
                Deck.Save(deckFilename);
                unsaved = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while trying to save the deck:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAs()
        {
            var sfd = new Microsoft.Win32.SaveFileDialog
            {
                AddExtension = true,
                Filter = "OCTGN decks|*.o8d",
                InitialDirectory = Game.DefaultDecksPath
            };
            if (!sfd.ShowDialog().GetValueOrDefault()) return;
            try
            {
                Deck.Save(sfd.FileName);
                unsaved = false;
                deckFilename = sfd.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while trying to save the deck:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDeckCommand(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            LoadDeck(Game);
        }

        private void LoadClicked(object sender, RoutedEventArgs e)
        {
            var game = (Data.Game)((MenuItem)e.OriginalSource).DataContext;
            LoadDeck(game);
        }

        private void LoadDeck(Data.Game game)
        {
            if (unsaved)
            {
                var result = MessageBox.Show("This deck contains unsaved modifications. Save?", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Save();
                        break;
                    case MessageBoxResult.No:
                        break;
                    default:
                        return;
                }
            }

            // Show the dialog to choose the file
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "OCTGN deck files (*.o8d) | *.o8d",
                InitialDirectory = game != null ? game.DefaultDecksPath : null
            };
            if (ofd.ShowDialog() != true) return;
            // Try to load the file contents
            Deck newDeck;
            try
            {
                newDeck = Deck.Load(ofd.FileName, Program.GamesRepository);
            }
            catch (DeckException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("OCTGN couldn't load the deck.\r\nDetails:\r\n\r\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Game = Program.GamesRepository.Games.First(g => g.Id == newDeck.GameId);
            Deck = newDeck;
            deckFilename = ofd.FileName;
            CommandManager.InvalidateRequerySuggested();
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        { Close(); }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            if (unsaved)
            {
                var result = MessageBox.Show("This deck contains unsaved modifications. Save?", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Save();
                        break;
                    case MessageBoxResult.No:
                        break;
                    default:
                        e.Cancel = true;
                        return;
                }
            }

            Game = null;	// Close DB if required
            Program.lwMainWindow.Show();
            if ((Program.lwMainWindow.Content as Launcher.MainMenu) == null)
                Program.lwMainWindow.Navigate(new Launcher.MainMenu());
            Application.Current.MainWindow = Program.lwMainWindow;
        }

        private void CardSelected(object sender, SearchCardImageEventArgs e)
        {
            try
            {
                cardImage.Source = new BitmapImage(e.Image != null ?
                    CardModel.GetPictureUri(Game, e.SetId, e.Image) :
                    Game.GetCardBackUri());
            }
            catch (Exception er)
            {
                ErrorLog.WriteError(er, "DeckBuilderWindowError: " + Game.GetCardBackUri() + ":" + e.SetId.ToString() + ":" + e.Image + ":" + Game.Name, false);
            }
        }

        private void ElementSelected(object sender, SelectionChangedEventArgs e)
        {
            var grid = (DataGrid)sender;
            var element = (Data.Deck.Element)grid.SelectedItem;

            // Don't hide the picture if the selected element was removed
            // with a keyboard shortcut from the results grid
            if (element == null && !grid.IsFocused) return;

            cardImage.Source = new BitmapImage(element != null ?
                new Uri(element.Card.Picture) :
                Game.GetCardBackUri());
        }

        private void IsDeckOpen(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Deck != null;
            e.Handled = true;
        }

        private void AddResultCard(object sender, SearchCardIdEventArgs e)
        {
            unsaved = true;
            var element = ActiveSection.Cards.FirstOrDefault(c => c.Card.Id == e.CardId);
            if (element != null)
                element.Quantity += 1;
            else
                ActiveSection.Cards.Add(new Deck.Element { Card = Game.GetCardById(e.CardId), Quantity = 1 });
        }

        private void RemoveResultCard(object sender, SearchCardIdEventArgs e)
        {
            unsaved = true;
            var element = ActiveSection.Cards.FirstOrDefault(c => c.Card.Id == e.CardId);
            if (element != null)
            {
                element.Quantity -= 1;
                if (element.Quantity == 0)
                    ActiveSection.Cards.Remove(element);
            }
        }

        private void DeckKeyDownHandler(object sender, KeyEventArgs e)
        {
            var grid = (DataGrid)sender;
            var element = (Deck.Element)grid.SelectedItem;
            if (element == null) return;

            switch (e.Key)
            {
                case Key.Insert:
                case Key.Add:
                    unsaved = true;
                    element.Quantity += 1;
                    e.Handled = true;
                    break;

                case Key.Delete:
                case Key.Subtract:
                    unsaved = true;
                    element.Quantity -= 1;
                    e.Handled = true;
                    break;
            }
        }

        private void ElementEditEnd(object sender, DataGridCellEditEndingEventArgs e)
        {
            unsaved = true;
        }

        private void SetActiveSection(object sender, RoutedEventArgs e)
        {
            ActiveSection = (Deck.Section)((FrameworkElement)sender).DataContext;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members

        private void PreventExpanderBehavior(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }

    public class ActiveSectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2) return Binding.DoNothing;
            return values[0] == values[1] ? FontWeights.Bold : FontWeights.Normal;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        { throw new NotImplementedException(); }
    }
}