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
                                    UpdateColors();
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
                listBox1.Items.Add(Program.LClient.OnlineUsers[i].Username);
            }
            
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationService.RemoveBackEntry();
            Skylabs.Networking.SocketClient.SocketMessage sm = new Skylabs.Networking.SocketClient.SocketMessage("GETONLINELIST");
            Program.LClient.writeMessage(sm);
            Update_Online_Users();
            tbNickname.Text = "Your nickname: " + Program.LClient.strUserName;
        }

        private void UpdateColors()
        {
            return;
            if (rtbChat.Document == null)
                return;

            TextRange documentRange = new TextRange(rtbChat.Document.ContentStart, rtbChat.Document.ContentEnd);
            documentRange.ClearAllProperties();

            TextPointer navigator = rtbChat.Document.ContentStart;
            while (navigator.CompareTo(rtbChat.Document.ContentEnd) < 0)
            {
                TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                {
                    //navigator.Parent.
                    CheckWordsInRun((Run)navigator.Parent);


                }
                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);

            }

            for (int i = 0; i < m_tags.Count; i++)
            {
                TextRange range = new TextRange(m_tags[i].StartPosition, m_tags[i].EndPosition);
                switch (m_tags[i].Type)
                {
                    case Networking.LobbyClient.LobbyChatTypes.Global:
                        if (range.Text.Equals("[" + Program.LClient.strUserName + "]:"))
                        {
                            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Blue));
                            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                        }
                        else
                        {
                            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));
                            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                        }
                    break;
                    case Networking.LobbyClient.LobbyChatTypes.Whisper:
                            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Orange));
                            range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
                    break;
                    case Networking.LobbyClient.LobbyChatTypes.System:
                            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Gray));
                            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                    break;
                    case Networking.LobbyClient.LobbyChatTypes.Error:
                            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Red));
                            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                    break;
                }
            }
            m_tags.Clear();
            //SendMessage(rtbChat.Handle, WM_VSCROLL, SB_BOTTOM, 0);
            rtbChat.ScrollToEnd();
        }
        new struct Tag
        {
            public TextPointer StartPosition;
            public TextPointer EndPosition;
            public string Word;
            public Networking.LobbyClient.LobbyChatTypes Type;

        }
        List<Tag> m_tags = new List<Tag>();
        void CheckWordsInRun(Run run)
        {
            string text = run.Text.Trim();
            if (text.Equals(""))
                return;
            int sIndex = 0;
                if (text[0] == '<')
                {
                    Tag t = new Tag();
                    int s = 0;
                    int e = text.IndexOf(':');
                    t.StartPosition = run.ContentStart.GetPositionAtOffset(s, LogicalDirection.Forward);
                    t.EndPosition = run.ContentStart.GetPositionAtOffset(e, LogicalDirection.Backward);
                    t.Word = text.Substring(s,text.Length - e);
                    t.Type = Networking.LobbyClient.LobbyChatTypes.Whisper;
                    m_tags.Add(t);
                    return;
                }
            String[] words = text.Split(new char[1]{' '});
            for(int i=0;i<words.Length;i++)
            {
                string word = words[i];

                for (int u = 0; u < Program.LClient.OnlineUsers.Count; u++)
                {
                    if (word.Equals("[" + Program.LClient.OnlineUsers[u].Username + "]:"))
                    {
                        Tag t = new Tag();
                        int s = text.IndexOf(word,sIndex);
                        sIndex = s + word.Length - 1;
                        t.StartPosition = run.ContentStart.GetPositionAtOffset(s, LogicalDirection.Forward);
                        t.EndPosition = run.ContentStart.GetPositionAtOffset(s + word.Length, LogicalDirection.Backward);
                        t.Word = word;
                        t.Type = Networking.LobbyClient.LobbyChatTypes.Global;
                        m_tags.Add(t);
                    }
                    else if (word.Equals("[" + Program.LClient.OnlineUsers[u].Username + "]"))
                    {
                        Tag t = new Tag();
                        int s = text.IndexOf(word, sIndex);
                        sIndex = s + word.Length - 1;
                        t.StartPosition = run.ContentStart.GetPositionAtOffset(s, LogicalDirection.Forward);
                        t.EndPosition = run.ContentStart.GetPositionAtOffset(s + word.Length, LogicalDirection.Backward);
                        t.Word = word;
                        t.Type = Networking.LobbyClient.LobbyChatTypes.Global;
                        m_tags.Add(t);
                    }
                    else if (word.Equals("!ERROR:"))
                    {
                        Tag t = new Tag();
                        int s = text.IndexOf(word, sIndex);
                        sIndex = s + word.Length - 1;
                        t.StartPosition = run.ContentStart.GetPositionAtOffset(s, LogicalDirection.Forward);
                        t.EndPosition = run.ContentStart.GetPositionAtOffset(s + word.Length, LogicalDirection.Backward);
                        t.Word = word;
                        t.Type = Networking.LobbyClient.LobbyChatTypes.Error;
                        m_tags.Add(t);
                    }
                    else if (word.Equals("#SYSTEM:"))
                    {
                        Tag t = new Tag();
                        int s = text.IndexOf(word, sIndex);
                        sIndex = s + word.Length - 1;
                        t.StartPosition = run.ContentStart.GetPositionAtOffset(s, LogicalDirection.Forward);
                        t.EndPosition = run.ContentStart.GetPositionAtOffset(s + word.Length, LogicalDirection.Backward);
                        t.Word = word;
                        t.Type = Networking.LobbyClient.LobbyChatTypes.System;
                        m_tags.Add(t);
                    }
                }    
            }
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

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (!Program.LClient.isHosting)
            {
                Program.LClient.LastGameInfo = textBox1.Text;
                if (Program.LClient.LastGameInfo.Trim().Equals(""))
                {
                    Program.LClient.LastGameInfo = ".";
                }
                Program.lwMainWindow.NavigationService.Navigate(new LobbyHost(true));
                Program.LClient.isHosting = true;
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (!Program.LClient.isHosting && !Program.LClient.isJoining)
            {
                Program.LClient.isJoining = true;
                int ind = dataGrid1.SelectedIndex;
                try
                {
                    HostedGame hg = Program.LClient.HostedGames[ind];
                    for (int i = 0; i < Program.GamesRepository.Games.Count; i++)
                    {
                        if (Program.GamesRepository.Games[i].Id.ToString().Equals(hg.getStrGUID()))
                        {
                            if (Program.Game == null)
                            {
                                Program.Game = new Game(GameDef.FromO8G(Program.GamesRepository.Games[i].Filename));
                            }
                            else if (Program.Game.Definition.Id != Program.GamesRepository.Games[i].Id)
                            {
                                Program.Game = new Game(GameDef.FromO8G(Program.GamesRepository.Games[i].Filename));
                            }
                            change_join_text("Joining...");
                            intIpTried = 0;
                            port = hg.getIntPort();
                            ips = hg.getStrHost();
                            Join_Game(ips, hg.getIntPort());
                            break;
                        }
                    }
                    
                    
                }
                catch (Exception exe)
                {
                    Program.LClient.isJoining = false;
                }
                
                
            }
        }
        private void Join_Game( String[] host, int port)
        {
            if (Program.Game != null)
            {
                Program.Client = new Networking.Client(IPAddress.Parse(host[intIpTried]), port);
                Program.Client.BeginConnect(ConnectedCallback);
            }
            else
                Program.LClient.isJoining = false;
        }
        
        private void ConnectedCallback(object sender, ConnectedEventArgs e)
        {
            if (e.exception == null)
            {
                change_join_text("Join");
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new NoArgsDelegate(LaunchGame));
            }
            else
            {
                intIpTried++;
                if (intIpTried == ips.Length)
                {
                    Program.LClient.isJoining = false;
                    MessageBox.Show("Unable to join server.");

                    change_join_text("Join");

                }
                else
                {
                    Join_Game(ips, port);
                }
            }
        }
        private void change_join_text(String text)
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

                                    button3.Content = text;
                                }
                            )
                        );
                    }
                )
            );
            thread.Start();
        }
        private void LaunchGame()
        { Program.lwMainWindow.NavigationService.Navigate(new StartGame(true)); }

    }
}