using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using Octgn.Definitions;
using Octgn.Networking;
using Octgn.Properties;
using Skylabs.NetShit;

namespace Octgn.Lobby
{
    public sealed partial class Lobby : Page
    {
        private List<String> lsMessageHistory;
        private int intMessHistoryIndex = 0;
        private int intIpTried = 0;
        private String[] ips;
        private int port;
        private Boolean justScrolledToBottom;
        public Boolean okToCloseMainWindow;

        public Lobby()
        {
            eSub();
            justScrolledToBottom = false;
            InitializeComponent();
            okToCloseMainWindow = false;
            if (Settings.Default.LobbySound)
            {
                Uri myUri = new Uri("/OCTGNwLobby;component/Images/audio.png", UriKind.RelativeOrAbsolute);
                imgSound.Source = new BitmapImage(myUri);
            }
            else
            {
                Uri myUri = new Uri("/OCTGNwLobby;component/Images/audiomute.png", UriKind.RelativeOrAbsolute);
                imgSound.Source = new BitmapImage(myUri);
            }

            if (Program.GamesRepository != null)
            {
                foreach (Data.Game g in Program.GamesRepository.Games)
                {
                    comboBox1.Items.Add(g.Name);
                }
            }
            if (Program.Game != null)
                comboBox1.Text = Program.Game.Definition.Name;

            this.rtbChat.SetValue(Paragraph.LineHeightProperty, .5);
            dataGrid1.ItemsSource = Program.LClient.HostedGames;
            lsMessageHistory = new List<string>();
        }

        private void eSub()
        {
            try
            {
                Program.LClient.eLobbyChat += new LobbyClient.LobbyChatDelegate(LClient_eLobbyChat);
                Program.LClient.eUserEvent += new LobbyClient.UserDelegate(LClient_eUserEvent);
                Program.LClient.eLogEvent += new LobbyClient.ConnectionDelegate(LClient_eConnection);
                Program.LClient.eGameHost += new LobbyClient.GameHostDelegate(LClient_eGameHost);
                Program.LClient.eUserStatusChanged += new LobbyClient.UserStatusChangedDelegate(LClient_eUserStatusChanged);
                Program.lwLobbyWindow.Activated += delegate(object sender, EventArgs e)
                {
                    if (Program.lwLobbyWindow.TaskbarItemInfo == null)
                        Program.lwLobbyWindow.TaskbarItemInfo = new TaskbarItemInfo();
                    if (Program.lwLobbyWindow.TaskbarItemInfo.ProgressState == TaskbarItemProgressState.Indeterminate)
                        Program.lwLobbyWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
                };
            }
            catch (Exception e) { };
        }

        private void LClient_eUserStatusChanged(LobbyClient.User user, PlayerStatus status)
        {
            System.Threading.Thread thread = new System.Threading.Thread
             (
                 new System.Threading.ThreadStart
                 (
                     delegate()
                     {
                         rtbChat.Dispatcher.Invoke
                         (
                             System.Windows.Threading.DispatcherPriority.Normal,
                             new Action
                             (
                                 delegate()
                                 {
                                     Update_Online_Users();
                                 }
                             )
                         );
                     }
                 )
             );

            thread.Start();
        }

        private void eUnSub()
        {
            try
            {
                Program.LClient.eLobbyChat -= LClient_eLobbyChat;
                Program.LClient.eUserEvent -= LClient_eUserEvent;
                Program.LClient.eLogEvent -= LClient_eConnection;
                Program.LClient.eGameHost -= LClient_eGameHost;
                Program.LClient.eUserStatusChanged -= LClient_eUserStatusChanged;
                NavigationService.Navigating -= NavigationService_Navigating;
                var navWnd = Application.Current.MainWindow as Octgn.Launcher.LauncherWindow;
            }
            catch (Exception e) { };
        }

        private DataGridRow GetRow(DataGrid dg, int index)
        {
            DataGridRow row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // may be virtualized, bring into view and try again
                dg.ScrollIntoView(dg.Items[index]);
                row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        private void LClient_eGameHost(HostedGame game, bool unHosting, bool isGameListItem)
        {
            System.Threading.Thread thread = new System.Threading.Thread
            (
                new System.Threading.ThreadStart
                (
                    delegate()
                    {
                        rtbChat.Dispatcher.Invoke
                        (
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action
                            (
                                delegate()
                                {
                                    if (unHosting)
                                    {
                                        for (int i = 0; i < Program.LClient.HostedGames.Count; i++)
                                        {
                                            if (Program.LClient.HostedGames[i].intUGameNum == game.intUGameNum)
                                                Program.LClient.HostedGames.RemoveAt(i);
                                        }
                                        return;
                                    }
                                    if (isGameListItem)
                                    {
                                        Program.LClient.HostedGames.Add(game);
                                    }
                                    else
                                    {
                                        if (game.strHostName == Program.LClient.strUserName)
                                        {
                                            IPHostEntry host = Dns.GetHostEntry(game.getStrHost()[0]);

                                            // Addres of the host.
                                            IPAddress[] addressList = host.AddressList;

                                            ips = new String[addressList.Length];
                                            int i = 0;
                                            foreach (IPAddress ip in addressList)
                                            {
                                                ips[i] = ip.ToString();
                                                i++;
                                            }
                                            Join_Game(ips, game.getIntPort());
                                            //Program.Client = new Networking.Client(addresslist[0], game.getIntPort());
                                            //Program.Client.Connect();
                                            //Program.LClient.isHosting = true;
                                        }
                                        Run r = new Run("#SYSTEM: ");
                                        Brush b = Brushes.Red;
                                        r.ToolTip = DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString();
                                        r.Foreground = b;
                                        r.Cursor = Cursors.Hand;
                                        r.Background = Brushes.White;
                                        Paragraph p = new Paragraph();
                                        p.Inlines.Add(new Bold(r));
                                        r = new Run(game.strHostName + " is hosting a " + game.strGameName + " ");
                                        p.Inlines.Add(r);
                                        r = getGameRun(game.strHostName, game);
                                        p.Inlines.Add(r);
                                        r = new Run(": " + game.strName);
                                        p.Inlines.Add(r);
                                        rtbChat.Document.Blocks.Add(p);
                                        rtbChat.ScrollToEnd();
                                        if (Settings.Default.LobbySound)
                                        {
                                            System.Media.SoundPlayer sp = new System.Media.SoundPlayer(Properties.Resources.click);
                                            sp.Play();
                                        }

                                        Program.LClient.HostedGames.Add(game);
                                    }
                                }
                            )
                        );
                    }
                )
            );
            thread.Start();
        }

        private Run getUserRun(String user, string fulltext)
        {
            Run r = new Run(fulltext);
            r.ToolTip = DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "\nClick to whisper " + user;
            r.Cursor = Cursors.Hand;
            r.Background = Brushes.White;
            r.MouseEnter += delegate(object sender, MouseEventArgs e)
            {
                r.Background = new RadialGradientBrush(Colors.DarkGray, Colors.WhiteSmoke);
            };
            r.MouseLeave += delegate(object sender, MouseEventArgs e)
            {
                r.Background = Brushes.White;
            };
            r.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(delegate(object sender, MouseButtonEventArgs e)
            {
                tbMess.Text = "/w " + user + " ";
                tbMess.SelectionStart = tbMess.Text.Length - 1;
                tbMess.Focus();
                tbMess.Focus();
            }));
            return r;
        }

        private Run getGameRun(String user, HostedGame game)
        {
            Run r = new Run("game");
            r.ToolTip = "Click to join " + user + "'s game";
            r.Cursor = Cursors.Hand;
            r.Foreground = Brushes.Blue;
            r.Background = Brushes.White;
            r.MouseEnter += delegate(object sender, MouseEventArgs e)
            {
                r.Background = new RadialGradientBrush(Colors.DarkGray, Colors.WhiteSmoke);
            };
            r.MouseLeave += delegate(object sender, MouseEventArgs e)
            {
                r.Background = Brushes.White;
            };
            r.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(delegate(object sender, MouseButtonEventArgs e)
            {
                if (user.Equals(Program.LClient.strUserName))
                    return;
                if (!Program.LClient.isHosting && !Program.LClient.isJoining)
                {
                    if ((Application.Current.MainWindow as Play.PlayWindow) != null)
                        Application.Current.MainWindow.Close();
                    else if ((Application.Current.MainWindow as DeckBuilder.DeckBuilderWindow) != null)
                        Application.Current.MainWindow.Close();
                    Program.LClient.isJoining = true;
                    if (SelectGame(game.getStrGUID()))
                    {
                        intIpTried = 0;
                        port = game.getIntPort();
                        ips = game.getStrHost();

                        IPHostEntry host = Dns.GetHostEntry(ips[0]);

                        // Addres of the host.
                        IPAddress[] addressList = host.AddressList;

                        ips[0] = addressList[0].ToString();

                        Join_Game(ips, game.getIntPort());
                    }
                    else
                    {
                        //change_join_text("Join");
                        MessageBox.Show("You do not have the correct game installed.");
                        Program.LClient.isJoining = false;
                    }
                }
                else
                {
                    if (Program.LClient.isHosting)
                    {
                        MessageBox.Show("Please stop hosting first.");
                    }
                    if (Program.LClient.isJoining)
                    {
                        MessageBox.Show("Please stop joining first.");
                    }
                }
            }));
            return r;
        }

        private void LClient_eConnection(string ConEvent)
        {
            System.Threading.Thread thread = new System.Threading.Thread
            (
                new System.Threading.ThreadStart
                (
                    delegate()
                    {
                        rtbChat.Dispatcher.Invoke
                        (
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action
                            (
                                delegate()
                                {
                                    if (ConEvent.Equals("DC"))
                                    {
                                        LClient_eLobbyChat(LobbyClient.LobbyChatTypes.System, "SYSTEM", "Disconnected from server! /reconnect to reconnect. /login email password to log back in.", false);
                                        //Leave_Lobby();
                                    }
                                    else if (ConEvent.Equals("CON"))
                                    {
                                        LClient_eLobbyChat(LobbyClient.LobbyChatTypes.System, "SYSTEM", "Connected to server!", false);
                                    }
                                }
                            )
                        );
                    }
                )
            );
            thread.Start();
        }

        private void LClient_eUserEvent(LobbyClient.User user, bool Connected)
        {
            System.Threading.Thread thread = new System.Threading.Thread
            (
                new System.Threading.ThreadStart
                (
                    delegate()
                    {
                        rtbChat.Dispatcher.Invoke
                        (
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action
                            (
                                delegate()
                                {
                                    if (rtbChat == null)
                                        return;
                                    if (Connected)
                                    {
                                        if (!user.Username.Equals(""))
                                        {
                                            LClient_eLobbyChat(LobbyClient.LobbyChatTypes.System, "SYSTEM", user.Username + " joined the lobby.", false);
                                            if (Settings.Default.LobbySound)
                                            {
                                                System.Media.SoundPlayer sp = new System.Media.SoundPlayer(Properties.Resources.logon);
                                                sp.Play();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!user.Username.Equals(""))
                                        {
                                            LClient_eLobbyChat(LobbyClient.LobbyChatTypes.System, "SYSTEM", user.Username + " left the lobby.", false);
                                            if (Settings.Default.LobbySound)
                                            {
                                                System.Media.SoundPlayer sp = new System.Media.SoundPlayer(Properties.Resources.logoff);
                                                sp.Play();
                                            }
                                        }
                                    }
                                    Update_Online_Users();
                                }
                            )
                        );
                    }
                )
            );

            thread.Start();
        }

        private void LClient_eLobbyChat(LobbyClient.LobbyChatTypes type, string user, string chat, bool inXaml)
        {
            rtbChat.Dispatcher.Invoke
            (
                System.Windows.Threading.DispatcherPriority.Normal,
                new Action
                (
                    delegate()
                    {
                        if (rtbChat == null)
                            return;
                        bool rtbatbottom = false;
                        //check to see if the richtextbox is scrolled to the bottom.
                        //----------------------------------------------------------------------------------
                        double dVer = rtbChat.VerticalOffset;

                        //get the vertical size of the scrollable content area
                        double dViewport = rtbChat.ViewportHeight;

                        //get the vertical size of the visible content area
                        double dExtent = rtbChat.ExtentHeight;

                        if (dVer != 0)
                        {
                            if (dVer + dViewport == dExtent)
                            {
                                rtbatbottom = true;
                                justScrolledToBottom = false;
                            }
                            else
                            {
                                if (!justScrolledToBottom)
                                {
                                    Paragraph pa = new Paragraph();
                                    Run ru = new Run("------------------------------");
                                    ru.Foreground = Brushes.Red;
                                    pa.Inlines.Add(new Bold(ru));
                                    rtbChat.Document.Blocks.Add(pa);
                                    justScrolledToBottom = true;
                                }
                            }
                        }
                        //----------------------------------------------------------------------------------

                        Paragraph p = new Paragraph();
                        Run r;
                        Brush b;
                        switch (type)
                        {
                            case LobbyClient.LobbyChatTypes.Global:
                                r = getUserRun(user, "[" + user + "]: ");
                                if (user.Length > 5)
                                {
                                    if (user.Substring(0, 5).ToLower().Equals("<irc>"))
                                        b = System.Windows.Media.Brushes.DarkGray;
                                    else
                                        b = System.Windows.Media.Brushes.Black;
                                }
                                else
                                    b = System.Windows.Media.Brushes.Black;
                                if (user.Equals(Program.LClient.strUserName))
                                    b = Brushes.Blue;

                                r.Foreground = b;

                                p.Inlines.Add(new Bold(r));
                                break;
                            case LobbyClient.LobbyChatTypes.System:
                                r = new Run("#" + user + ": ");
                                b = Brushes.Red;
                                r.ToolTip = DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString();
                                if (user.Equals("SUPPORT"))
                                    r.Foreground = Brushes.White;
                                else
                                {
                                    Program.TraceWarning("#" + user + ": " + chat);
                                    r.Foreground = b;
                                }
                                r.Cursor = Cursors.Hand;
                                //r.Background = Brushes.White;

                                p.Inlines.Add(new Bold(r));
                                if (user.Equals("SUPPORT"))
                                {
                                    try
                                    {
                                        rtbChat.Document.Blocks.Add(p);
                                        //Put xaml into rtb-------------------------------------------------------------
                                        StringReader stringReader = new StringReader(chat);
                                        System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(stringReader);
                                        FlowDocument d = XamlReader.Load(xmlReader) as FlowDocument;
                                        Section s = new Section();
                                        s.BorderBrush = Brushes.Black;
                                        s.BorderThickness = new Thickness(1);
                                        s.Blocks.Add(p);
                                        GradientStopCollection gsc = new GradientStopCollection();
                                        gsc.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#e8c1c0"), 0));
                                        gsc.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#c42123"), .4));
                                        gsc.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#ea3a3c"), 1));
                                        s.Background = new LinearGradientBrush(gsc, (double)90.0);

                                        while (d.Blocks.Count > 0)
                                        {
                                            Block block = d.Blocks.FirstBlock;
                                            //block.BorderBrush = Brushes.Red;
                                            //block.BorderThickness = new Thickness(1);
                                            block.Foreground = Brushes.White;
                                            s.Blocks.Add(block);
                                            //richTextBox1.Document.Blocks.Add(block);
                                        }
                                        rtbChat.Document.Blocks.Add(s);
                                        //-------------------------------------------------------------------------------------------------------------
                                        if (rtbatbottom)
                                        {
                                            rtbChat.ScrollToEnd();
                                        }
                                        if (Settings.Default.LobbySound && !Program.lwLobbyWindow.IsActive)
                                        {
                                            System.Media.SoundPlayer sp = new System.Media.SoundPlayer(Properties.Resources.click);
                                            sp.Play();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        if (chat != null)
                                            ErrorLog.WriteError(e, chat, false);
                                        else
                                            ErrorLog.WriteError(e, "null chat", false);
                                    }
                                    return;
                                }
                                break;
                            case LobbyClient.LobbyChatTypes.Whisper:
                                String[] w = user.Split(new char[1] { ':' });
                                string u = "";
                                if (!w[0].Equals(Program.LClient.strUserName))
                                    u = w[0];
                                else if (!w[1].Equals(Program.LClient.strUserName))
                                    u = w[1];
                                else
                                    u = w[0];
                                r = getUserRun(u, "<" + w[0] + ">" + w[1] + ": ");
                                Program.TraceWarning("#WHISPER FROM " + u + " : " + chat);
                                r.ToolTip = DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "\nClick here to whisper back";
                                b = Brushes.Orange;
                                r.Foreground = b;
                                p.Inlines.Add(new Italic(r));
                                break;
                            case LobbyClient.LobbyChatTypes.Error:
                                r = new Run("!" + user + ": ");
                                r.ToolTip = DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString();
                                b = Brushes.Red;
                                r.Foreground = b;
                                r.Cursor = Cursors.Hand;
                                r.Background = Brushes.White;
                                p.Inlines.Add(new Bold(r));
                                break;
                        }
                        if (chat.Contains("\n"))
                        {
                            String[] lines = chat.Split(new char[1] { '\n' });
                            foreach (String line in lines)
                            {
                                String[] words = line.Split(new char[1] { ' ' });
                                foreach (String word in words)
                                {
                                    Inline inn = StringToRun(word, type);

                                    if (inn != null)
                                        p.Inlines.Add(inn);
                                    p.Inlines.Add(new Run(" "));
                                }
                                p.Inlines.Add(new Run("\n"));
                            }
                        }
                        else
                        {
                            String[] words = chat.Split(new char[1] { ' ' });
                            foreach (String word in words)
                            {
                                Inline inn = StringToRun(word, type);

                                if (inn != null)
                                    p.Inlines.Add(inn);
                                p.Inlines.Add(new Run(" "));
                            }
                        }
                        rtbChat.Document.Blocks.Add(p);

                        if (rtbatbottom)
                        {
                            rtbChat.ScrollToEnd();
                        }
                        if (Settings.Default.LobbySound && !Program.lwLobbyWindow.IsActive)
                        {
                            System.Media.SoundPlayer sp = new System.Media.SoundPlayer(Properties.Resources.click);
                            sp.Play();
                        }
                    }
                )
            );
        }

        private void NavigationService_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
        }

        public Inline StringToRun(String s, LobbyClient.LobbyChatTypes type)
        {
            Brush b;
            Inline ret = null;
            String strUrlRegex = "(?i)\\b((?:[a-z][\\w-]+:(?:/{1,3}|[a-z0-9%])|www\\d{0,3}[.]|[a-z0-9.\\-]+[.][a-z]{2,4}/)(?:[^\\s()<>]+|\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\))+(?:\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\)|[^\\s`!()\\[\\]{};:'\".,<>?������]))";
            Regex reg = new Regex(strUrlRegex);
            s = s.Trim();
            if (type == LobbyClient.LobbyChatTypes.System || type == LobbyClient.LobbyChatTypes.Error)
                b = Brushes.Gray;
            else
                b = Brushes.Black;
            Inline r = new Run(s);
            if (reg.IsMatch(s))
            {
                b = Brushes.LightBlue;
                Hyperlink h = new Hyperlink(r);
                h.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(h_RequestNavigate);
                try
                {
                    h.NavigateUri = new Uri(s);
                }
                catch (UriFormatException e)
                {
                    s = "http://" + s;
                    try
                    {
                        h.NavigateUri = new Uri(s);
                    }
                    catch (Exception ex)
                    {
                        r.Foreground = b;
                        System.Windows.Documents.Underline ul = new Underline(r);
                    }
                }
                ret = h;
            }
            else
            {
                if (s.Equals(Program.LClient.strUserName))
                {
                    b = Brushes.Blue;
                    ret = new Bold(r);
                }
                else
                {
                    Boolean fUser = false;
                    foreach (LobbyClient.User u in Program.LClient.OnlineUsers)
                    {
                        if (u.Username == s)
                        {
                            b = Brushes.LightGreen;
                            ret = new Bold(r);
                            ret.ToolTip = "Click to whisper";
                            r.Cursor = Cursors.Hand;
                            r.Background = Brushes.White;
                            r.MouseEnter += delegate(object sender, MouseEventArgs e)
                            {
                                r.Background = new RadialGradientBrush(Colors.DarkGray, Colors.WhiteSmoke);
                            };
                            r.MouseLeave += delegate(object sender, MouseEventArgs e)
                            {
                                r.Background = Brushes.White;
                            };
                            r.MouseUp += delegate(object sender, MouseButtonEventArgs e)
                            {
                                tbMess.Text = "/w " + s + " ";
                                tbMess.Focus();
                            };
                            fUser = true;
                            break;
                        }
                    }
                    if (!fUser)
                    {
                        ret = new Run(s);
                    }
                }
            }
            ret.Foreground = b;
            return ret;
        }

        private void h_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Hyperlink hl = (Hyperlink)sender;
            string navigateUri = hl.NavigateUri.ToString();
            try
            {
                Process.Start(new ProcessStartInfo(navigateUri));
            }
            catch { }
            e.Handled = true;
        }

        private delegate void NoArgsDelegate();

        private void richTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //rtbChat.AppendText(new TextRange(rtbMess.Document.ContentStart, rtbMess.Document.ContentEnd).Text);
            //Program.LClient.Chat(new TextRange(rtbMess.Document.ContentStart, rtbMess.Document.ContentEnd).Text);
            Program.LClient.Chat(tbMess.Text);
            tbMess.Text = "";
            intMessHistoryIndex = 0;
            // rtbChat.re
        }

        private void Update_Online_Users()
        {
            if (listBox1 == null)
                return;
            listBox1.Items.Clear();
            Program.LClient.OnlineUsers.Sort(delegate(LobbyClient.User p1, LobbyClient.User p2)
            {
                return p1.Username.CompareTo(p2.Username);
            });
            for (int i = 0; i < Program.LClient.OnlineUsers.Count; i++)
            {
                if (!listBox1.Items.Contains(Program.LClient.OnlineUsers[i].Username.ToString()))
                {
                    if (Program.LClient.OnlineUsers[i].Status == PlayerStatus.Available)
                        listBox1.Items.Add(Program.LClient.OnlineUsers[i].Username);
                    else
                        listBox1.Items.Add(Program.LClient.OnlineUsers[i].Username + "<" + Program.LClient.OnlineUsers[i].Status.ToString() + ">");
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationService.RemoveBackEntry();
            SocketMessage sm = new SocketMessage("GETONLINELIST");
            Program.LClient.writeMessage(sm);
            Update_Online_Users();
            tbNickname.Text = "Your nickname: " + Program.LClient.strUserName;
            rtbChat.ScrollToEnd();
            NavigationService.Navigating += new System.Windows.Navigation.NavigatingCancelEventHandler(NavigationService_Navigating);
        }

        private bool isOnlineUserName(String user)
        {
            for (int u = 0; u < Program.LClient.OnlineUsers.Count; u++)
            {
                if (Program.LClient.OnlineUsers[u].Username.Equals(user))
                    return true;
            }
            return false;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Leave_Lobby();
        }

        private void tbMess_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!tbMess.Text.Trim().Equals(""))
                {
                    String temp = Program.LClient.Chat(tbMess.Text);
                    if (!temp.Equals(""))
                        lsMessageHistory.Add(temp);
                    tbMess.Text = "";
                    intMessHistoryIndex = 0;
                }
            }
            else if (e.Key == Key.Down)
            {
                intMessHistoryIndex++;
                if (intMessHistoryIndex >= lsMessageHistory.Count)
                    intMessHistoryIndex = 0;
                try
                {
                    tbMess.Text = lsMessageHistory[intMessHistoryIndex];
                }
                catch (Exception ex)
                {
                    tbMess.Text = "";
                }
            }
            else if (e.Key == Key.Up)
            {
                intMessHistoryIndex--;
                if (intMessHistoryIndex <= 0)
                    intMessHistoryIndex = lsMessageHistory.Count - 1;

                try
                {
                    tbMess.Text = lsMessageHistory[intMessHistoryIndex];
                }
                catch (Exception ex)
                {
                    tbMess.Text = "";
                }
            }
        }

        private void Leave_Lobby()
        {
            try
            {
                eUnSub();
                Program.LClient.Close("Exit Lobby", true);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            try
            {
                Program.lwLobbyWindow.Close();
            }
            catch (Exception ex)
            {
            }
            Program.lwLobbyWindow = null;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Leave_Lobby();
        }

        private Boolean SelectGame(String GUID)
        {
            try
            {
                //HostedGame hg = Program.LClient.HostedGames[ind];
                for (int i = 0; i < Program.GamesRepository.Games.Count; i++)
                {
                    if (Program.GamesRepository.Games[i].Id.ToString().Equals(GUID))
                    {
                        if (Program.Game == null)
                        {
                            Program.Game = new Game(GameDef.FromO8G(Program.GamesRepository.Games[i].Filename));
                        }
                        else if (Program.Game.Definition.Id != Program.GamesRepository.Games[i].Id)
                        {
                            Program.Game = new Game(GameDef.FromO8G(Program.GamesRepository.Games[i].Filename));
                        }
                        return true;
                    }
                }
            }
            catch (Exception exe)
            {
            }
            return false;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (!Program.LClient.isHosting && !Program.LClient.isJoining)
            {
                if ((Application.Current.MainWindow as Play.PlayWindow) != null)
                    Application.Current.MainWindow.Close();
                else if ((Application.Current.MainWindow as DeckBuilder.DeckBuilderWindow) != null)
                    Application.Current.MainWindow.Close();
                int ind = dataGrid1.SelectedIndex;
                if (ind < 0)
                    return;
                Program.LClient.isJoining = true;
                HostedGame hg = null;
                try
                {
                    hg = Program.LClient.HostedGames[ind];
                }
                catch (ArgumentOutOfRangeException oor)
                {
                    MessageBox.Show("That game no longer exists.");
                    return;
                }
                if (SelectGame(hg.getStrGUID()))
                {
                    if (hg.strHostName.Equals(Program.LClient.strUserName))
                    {
                        Program.LClient.isJoining = false;
                        return;
                    }
                    intIpTried = 0;
                    port = hg.getIntPort();
                    ips = hg.getStrHost();
                    IPHostEntry host = null;
                    try
                    {
                        host = Dns.GetHostEntry(ips[0]);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("DNS Error. Please try restarting any routers or modems you have. If that doesn't work restart your computer.");
                        Program.LClient.isJoining = false;
                    }

                    if (host != null)
                    {
                        try
                        {
                            IPAddress[] addressList = host.AddressList;

                            ips[0] = addressList[0].ToString();

                            Join_Game(ips, hg.getIntPort());
                        }
                        catch (ArgumentOutOfRangeException oor)
                        {
                            MessageBox.Show("Error connecting.");
                        }
                    }
                }
                else
                {
                    //change_join_text("Join");
                    MessageBox.Show("You do not have the correct game installed.");
                    Program.LClient.isJoining = false;
                }
            }
            else
            {
                if (Program.LClient.isHosting)
                {
                    MessageBox.Show("Please stop hosting first.");
                }
                if (Program.LClient.isJoining)
                {
                    MessageBox.Show("Please stop joining first.");
                }
            }
        }

        private void Join_Game(String[] host, int port)
        {
            //change_join_text("Joining...");
            try
            {
                IPEndPoint ripe = Program.LClient.sock.Client.RemoteEndPoint as IPEndPoint;
                Program.Client = new Networking.Client(ripe.Address, port);
                Program.Client.BeginConnect(ConnectedCallback);
            }
            catch (IndexOutOfRangeException e)
            {
                ConnectedCallback(this, new ConnectedEventArgs(e));
            }
            catch (ObjectDisposedException ode)
            {
                MessageBox.Show("You have to be connected to the lobby to join or host a game.");
                ConnectedCallback(this, new ConnectedEventArgs(ode));
            }
        }

        private void ConnectedCallback(object sender, ConnectedEventArgs e)
        {
            if (e.exception == null)
            {
                //change_join_text("Join");
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new NoArgsDelegate(LaunchGame));
            }

            else
            {
                //intIpTried++;
                // if (intIpTried >= ips.Length)
                //{
                //intIpTried = 0;
                Program.LClient.isJoining = false;
                MessageBox.Show("Unable to join server.");
                if (Program.LClient.isHosting)
                {
                    Program.LClient.isHosting = false;
                    Program.LClient.unHost_Game();
                }
                //change_join_text("Join");

                //}
                //else
                //{
                //    Join_Game(ips, port);
                //}
            }
        }

        private void LaunchGame()
        {
            if (Program.LClient.isHosting)
                Program.LClient.ChangePlayerStatus(PlayerStatus.Hosting);
            else if (Program.LClient.isJoining)
                Program.LClient.ChangePlayerStatus(PlayerStatus.Playing);
            //Octgn.Launcher.LauncherWindow lw = (Octgn.Launcher.LauncherWindow)Application.Current.MainWindow;
            Program.lwMainWindow.NavigationService.Navigate(new Octgn.Lobby.StartGame());
            Program.lwMainWindow.Activate();
        }

        private void btnHost_Click(object sender, RoutedEventArgs e)
        {
            if (Program.LClient.isHosting)
            {
                MessageBox.Show("Please stop hosting first.");
            }
            if (Program.LClient.isJoining)
            {
                MessageBox.Show("Please stop joining/playing a game first.");
            }

            if (!Program.LClient.isHosting && !Program.LClient.isJoining)
            {
                if ((Application.Current.MainWindow as Play.PlayWindow) != null)
                    Application.Current.MainWindow.Close();
                else if ((Application.Current.MainWindow as DeckBuilder.DeckBuilderWindow) != null)
                    Application.Current.MainWindow.Close();

                Program.LClient.isHosting = true;
                if (String.IsNullOrEmpty(comboBox1.Text))
                {
                    MessageBox.Show("Pick a game.");
                    Program.LClient.isHosting = false;
                    return;
                }
                Program.Game = null;
                foreach (Data.Game g in Program.GamesRepository.Games)
                {
                    if (g.Name.Equals(comboBox1.Text))
                    {
                        SelectGame(g.Id.ToString());
                        break;
                    }
                }
                if (Program.Game == null)
                {
                    Program.LClient.isHosting = false;
                    return;
                }
                if (!Program.Game.Definition.CheckVersion())
                {
                    Program.LClient.isHosting = false;
                    return;
                }

                Program.LClient.LastGameInfo = textBox1.Text;
                if (Program.LClient.LastGameInfo.Trim().Equals(""))
                {
                    Program.LClient.LastGameInfo = ".";
                }
                Program.LClient.Host_Game(Program.Game.Definition);
            }
        }

        private void rtbChat_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!Program.lwLobbyWindow.IsActive)
                {
                    if (Program.lwLobbyWindow.TaskbarItemInfo == null)
                        Program.lwLobbyWindow.TaskbarItemInfo = new TaskbarItemInfo();
                    Program.lwLobbyWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
                }
            }
            catch { }
        }

        private void image1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Settings.Default.LobbySound)
            {
                Settings.Default.LobbySound = false;
                Uri myUri = new Uri("/OCTGNwLobby;component/Images/audiomute.png", UriKind.RelativeOrAbsolute);
                imgSound.Source = new BitmapImage(myUri);
                Settings.Default.Save();
            }
            else
            {
                Uri myUri = new Uri("/OCTGNwLobby;component/Images/audio.png", UriKind.RelativeOrAbsolute);
                imgSound.Source = new BitmapImage(myUri);

                Settings.Default.LobbySound = true;
                Settings.Default.Save();
            }
        }

        private void rtbChat_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
        }
    }
}