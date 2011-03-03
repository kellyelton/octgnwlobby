using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Xml;
using System.Windows.Media.Animation;
using Octgn.Networking;
using System.Net;
using Octgn.Definitions;
using Octgn.Properties;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Skylabs.NetShit;
namespace Octgn.Launcher
{
    public sealed partial class Lobby : Page
    {

        private List<String> lsMessageHistory;
        private int intMessHistoryIndex = 0;
        private int intIpTried = 0;
        private String[] ips;
        private int port;

        public Lobby()
        {
            eSub();
            InitializeComponent();

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
                Program.LClient.eLobbyChat += new Networking.LobbyClient.LobbyChatDelegate(LClient_eLobbyChat);
                Program.LClient.eUserEvent += new Networking.LobbyClient.UserDelegate(LClient_eUserEvent);
                Program.LClient.eLogEvent += new Networking.LobbyClient.ConnectionDelegate(LClient_eConnection);
                Program.LClient.eGameHost += new Networking.LobbyClient.GameHostDelegate(LClient_eGameHost);
            }
            catch (Exception e) { };
        }

        private void eUnSub()
        {
            try
            {
                Program.LClient.eLobbyChat -= LClient_eLobbyChat;
                Program.LClient.eUserEvent -= LClient_eUserEvent;
                Program.LClient.eLogEvent -= LClient_eConnection;
                Program.LClient.eGameHost -= LClient_eGameHost;
            }
            catch (Exception e) { };
        }
        DataGridRow GetRow(DataGrid dg, int index)
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
        void LClient_eGameHost(Networking.HostedGame game, bool unHosting, bool isGameListItem)
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

                                            IPAddress[] addresslist = Dns.GetHostAddresses(game.getStrHost()[0]);
                                            ips = new String[addresslist.Length];
                                            int i = 0;
                                            foreach (IPAddress ip in addresslist)
                                            {
                                                ips[i] = ip.ToString();
                                                i++;
                                            }
                                            Join_Game(ips, game.getIntPort());
                                            //Program.Client = new Networking.Client(addresslist[0], game.getIntPort());
                                            //Program.Client.Connect();
                                            //Program.LClient.isHosting = true;
                                        }
                                        else
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
        void LClient_eConnection(string ConEvent)
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
                                        LClient_eLobbyChat(LobbyClient.LobbyChatTypes.System, "SYSTEM", "Disconnected from server!");
                                        //Leave_Lobby();

                                    }
                                }
                            )
                        );
                    }
                )
            );
            thread.Start();
        }

        void LClient_eUserEvent(Networking.LobbyClient.User user, bool Connected)
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
                                            LClient_eLobbyChat(LobbyClient.LobbyChatTypes.System, "SYSTEM", user.Username + " joined the lobby.");
                                        }
                                    }
                                    else
                                    {
                                        if (!user.Username.Equals(""))
                                        {
                                            LClient_eLobbyChat(LobbyClient.LobbyChatTypes.System, "SYSTEM", user.Username + " left the lobby.");
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

        void LClient_eLobbyChat(Octgn.Networking.LobbyClient.LobbyChatTypes type,string user, string chat)
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
                        Paragraph p = new Paragraph();
                        Run r;
                        Brush b;
                        switch (type)
                        {
                            case Networking.LobbyClient.LobbyChatTypes.Global:
                                r = new Run("[" + user + "]: ");
                                b = System.Windows.Media.Brushes.Black;
                                if (user.Equals(Program.LClient.strUserName))
                                    b = Brushes.Blue;
                                r.Foreground = b;
                                p.Inlines.Add(new Bold(r));
                            break;
                            case Networking.LobbyClient.LobbyChatTypes.System:
                                r = new Run("#" + user + ": ");
                                b = Brushes.Red;
                                r.Foreground = b;
                                p.Inlines.Add(new Bold(r));

                            break;
                            case Networking.LobbyClient.LobbyChatTypes.Whisper:
                                String[] w = user.Split(new char[1] { ':' });
                                r = new Run("<" + w[0] + ">" +  w[1] + ": ");
                                b = Brushes.Orange;
                                r.Foreground = b;
                                p.Inlines.Add(new Italic(r));
                            break;
                            case Networking.LobbyClient.LobbyChatTypes.Error:
                                 r = new Run("!" + user + ": ");
                                b = Brushes.Red;
                                r.Foreground = b;
                                p.Inlines.Add(new Bold(r));
                            break;
                        }
                        String[] words = chat.Split(new char[1] { ' ' });
                        foreach (String word in words)
                        {
                            Inline inn = StringToRun(word, type);
                            if (inn != null)
                                p.Inlines.Add(inn);
                            p.Inlines.Add(new Run(" "));
                        }
                        rtbChat.Document.Blocks.Add(p);
                        //UpdateColors();
                        rtbChat.ScrollToEnd();
                    }
                )
            );
            
        }
        public Inline StringToRun(String s, Octgn.Networking.LobbyClient.LobbyChatTypes type)
        {
            Brush b;
            Inline ret = null;
            String strUrlRegex = "(?i)\\b((?:[a-z][\\w-]+:(?:/{1,3}|[a-z0-9%])|www\\d{0,3}[.]|[a-z0-9.\\-]+[.][a-z]{2,4}/)(?:[^\\s()<>]+|\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\))+(?:\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\)|[^\\s`!()\\[\\]{};:'\".,<>?«»“”‘’]))";
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
                    foreach (Octgn.Networking.LobbyClient.User u in Program.LClient.OnlineUsers)
                    {
                        if (u.Username == s)
                        {
                            b = Brushes.LightGreen;
                            ret = new Bold(r);
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

        void h_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {

            Hyperlink hl = (Hyperlink)sender;
            string navigateUri = hl.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri));
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
           // rtbChat.re
        }
        private void Update_Online_Users()
        {
            if (listBox1 == null)
                return;
            listBox1.Items.Clear();
            for (int i = 0; i < Program.LClient.OnlineUsers.Count; i++)
            {
                if(!listBox1.Items.Contains(Program.LClient.OnlineUsers[i].Username.ToString()))
                    listBox1.Items.Add(Program.LClient.OnlineUsers[i].Username);
            }
            
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationService.RemoveBackEntry();
            SocketMessage sm = new SocketMessage("GETONLINELIST");
            Program.LClient.writeMessage(sm);
            Update_Online_Users();
            tbNickname.Text = "Your nickname: " + Program.LClient.strUserName;
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
                    if(!temp.Equals(""))
                        lsMessageHistory.Add(temp);
                    tbMess.Text = "";
                }
            }
            else if (e.Key == Key.Up)
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
            else if (e.Key == Key.Down)
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
                Program.lwLobbyWindow.isOkToClose = true;
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
                Program.LClient.isJoining = true;
                int ind = dataGrid1.SelectedIndex;
                HostedGame hg = Program.LClient.HostedGames[ind];
                if(SelectGame(hg.getStrGUID()))
                {
                            intIpTried = 0;
                            port = hg.getIntPort();
                            ips = hg.getStrHost();
                            IPAddress[] addresslist = Dns.GetHostAddresses(ips[0]);

                            ips[0] = addresslist[0].ToString();

                            Join_Game(ips, hg.getIntPort());
                }
                else
                {
                    //change_join_text("Join");
                    MessageBox.Show("You do not have the correct game installed.");
                        Program.LClient.isJoining = false;
                }
                
                
            }
        }
        private void Join_Game( String[] host, int port)
        {
                //change_join_text("Joining...");
            try
            {
                Program.Client = new Networking.Client(IPAddress.Parse(host[intIpTried]), port);
                Program.Client.BeginConnect(ConnectedCallback);
            }
            catch (IndexOutOfRangeException e)
            {
                ConnectedCallback(this, new ConnectedEventArgs(e));
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
                intIpTried++;
                if (intIpTried >= ips.Length)
                {
                    Program.LClient.isJoining = false;
                    MessageBox.Show("Unable to join server.");
                    if (Program.LClient.isHosting)
                    {
                        Program.LClient.isHosting = false;
                        Program.LClient.unHost_Game();
                    }
                    //change_join_text("Join");

                }
                else
                {
                    Join_Game(ips, port);
                }
            }
        }
        private void LaunchGame()
        { Program.lwMainWindow.NavigationService.Navigate(new StartGame(true)); }

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

    }
}