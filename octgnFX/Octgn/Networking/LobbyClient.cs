﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Skylabs.Networking;
using System.Windows;
using System.Security.Cryptography;
using System.Collections;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Octgn.Properties;

namespace Octgn.Networking
{
    public class LobbyClient : SocketClient
    {
        public delegate void ConnectionDelegate(String ConEvent);
        public delegate void GameHostDelegate(HostedGame game, Boolean unHosting, Boolean isGameListItem);
        public delegate void ErrorDelegate(String error);
        public delegate void UserDelegate(User user, bool Connected);
        public delegate void LobbyChatDelegate(LobbyChatTypes type,String user, String chat);
        private delegate void NoArgsDelegate();
        private bool loggedIn = false;
        private string strLastWhisperFrom = "";
        public event UserDelegate eUserEvent;
        public event ConnectionDelegate eLogEvent;
        public event LobbyChatDelegate eLobbyChat;
        public event GameHostDelegate eGameHost;
        public event ErrorDelegate eError;
        public List<User> OnlineUsers;
        public String strUserName = "";
        public Boolean isHosting { get; set; }
        public Boolean isJoining { get; set; }
        public String LastGameInfo { get; set; }
        //public HostedGameBox HostedGames = new HostedGameBox();
        public ObservableCollection<HostedGame> HostedGames = new ObservableCollection<HostedGame>();
        public enum LobbyChatTypes { Global,Whisper,System, Error};
        public LobbyClient(String host, Int32 port)
        {
            strHost = host;
            intPort = port;
            OnlineUsers = new List<User>(0);
            regEvents();
            isHosting = false;
            LastGameInfo = "";
        }
        private void regEvents()
        {
            Program.lwMainWindow.Closed += new EventHandler(lwMainWindow_Closed);
            Program.lwLobbyWindow.Closed += new EventHandler(lwLobbyWindow_Closed);
        }
        private void unregEvents()
        {
            if (Program.lwMainWindow != null)
                Program.lwMainWindow.Closed -= lwMainWindow_Closed;
            if(Program.lwLobbyWindow != null)
                Program.lwLobbyWindow.Closed -= lwLobbyWindow_Closed;
        }
        void lwMainWindow_Closed(object sender, EventArgs e)
        {

            Program.lwLobbyWindow.Close();
        }

        void lwLobbyWindow_Closed(object sender, EventArgs e)
        {
            this.Close("Lobby Closed.", true);

        }
        //
        // Summary:
        //     Connects to the server and starts the run() loop.
        //
        public void Start()
        {
            this.Connect(strHost, intPort);
        }
        public void Login(String email, String password)
        {
            Encoding enc = Encoding.ASCII;
            byte[] buffer = new byte[0];
            String pass = "";
            try
            {
                buffer = enc.GetBytes(password);
            }
            catch (Exception e)
            {
                
            }
            SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();

            try
            {
                pass = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
            }
            catch (Exception e)
            {

            }
            cryptoTransformSHA1.Dispose();
            pass = pass.ToLower();
            SocketMessage sm = new SocketMessage("LOG");
            sm.Arguments.Add(email);
            sm.Arguments.Add(pass);
            this.writeMessage(sm);  
        }
        public void Register(String email, String username, String password)
        {
            SocketMessage sm = new SocketMessage("REG");
            sm.Arguments.Add(username);
            sm.Arguments.Add(email);
            sm.Arguments.Add(password);
            this.writeMessage(sm);
        }
        public String Chat(String text)
        {
            SocketMessage sm = new SocketMessage("LOBCHAT");
            text = text.Trim();
            if (text.Length == 0)
                return text;
            try
            {
                if (text.ToLower().Substring(0, 3).Equals("/r "))
                {
                    if (strLastWhisperFrom.Equals(""))
                        return "";
                    else
                        text = "/w " + strLastWhisperFrom + " " + text.Substring(3);
                }
            }
            catch (ArgumentOutOfRangeException ae)
            {
                
            }
            catch (Exception e)
            {
                return text;
            }
            sm.Arguments.Add(text);
            writeMessage(sm);
            return text;
        }

        public void Host_Game(String ip, int port, Definitions.GameDef def)
        {
            SocketMessage sm = new SocketMessage("HOST");
            sm.Arguments.Add(ip);
            sm.Arguments.Add(port.ToString());
            sm.Arguments.Add(def.Name);
            sm.Arguments.Add(def.Id.ToString());
            sm.Arguments.Add(def.Version.ToString());
            sm.Arguments.Add(LastGameInfo);
            this.isHosting = true;
            writeMessage(sm);
            
        }
        public void unHost_Game()
        {
            SocketMessage sm = new SocketMessage("UNHOST");
            this.isHosting = false;
            writeMessage(sm);
        }
        override
        public void handleError(String error) 
        {
            try
            {

                eError.Invoke(error);
            }
            catch (Exception e) { }
        }

        public override void handleInput(SocketMessage input) 
        {
            String head = input.Header;
            String a = "";
            String[] up ;
            List<String> args = input.Arguments;
            String sPing = new String(new char[1] { (char)1 });
            switch (head)
            {
                case "LOGSUCCESS":
                    loggedIn = true;
                    try
                    {
                        strUserName = args[0];
                        Settings.Default.NickName = Program.LClient.strUserName;
                        Settings.Default.Save();
                    }
                    catch (Exception e)
                    {
                        
                    }
                    eLogEvent.Invoke("LSUCC");
                break;
                case "LOGERROR":
                    MessageBox.Show("E-Mail or password is incorrect.", "Error");
                    eLogEvent.Invoke("LERR");
                break;
                case "REGERR0":
                    MessageBox.Show("Lobby server error. Please try again later.");
                    eLogEvent.Invoke("RERR");
                break;
                case "REGERR1":
                    MessageBox.Show("Please enter a valid e-mail address.");
                    eLogEvent.Invoke("RERR");
                break;
                case "REGERR2":
                    MessageBox.Show("Password must be at least 6 characters long.");
                    eLogEvent.Invoke("RERR");
                break;
                case "REGERR3":
                    MessageBox.Show("Nickname already exists.");
                    eLogEvent.Invoke("RERR");
                break;
                case "REGERR4":
                    MessageBox.Show("E-Mail already registered.");
                    eLogEvent.Invoke("RERR");
                break;
                case "REGSUCCESS":
                    MessageBox.Show("Account successfully registered.");
                    eLogEvent.Invoke("RSUCC");
                break;
                default:
                if (head.Equals(sPing))
                {
                    if (Program.lwLobbyWindow == null)
                        this.Close("Program failed.", true);
                    else
                    {
                        if (!Program.lwLobbyWindow.IsActive)
                            this.Close("Program failed.", true);
                    }
                }
                if (loggedIn)
                {
                    switch (head)
                    {
                        case "USERONLINE":
                            //User[] temp = new User[args.Count];
                            try
                            {
                                a = args[0];
                                up = a.Split(new char[1] { ':' });
                                Boolean userAlreadyOnList = false;
                                for (int i = 0; i < OnlineUsers.Count; i++)
                                {
                                    if (OnlineUsers[i].Username.Equals(up[1]))
                                    {
                                        userAlreadyOnList = true;
                                        break;
                                    }
                                }
                                if(!userAlreadyOnList)
                                    OnlineUsers.Add(new User(up[0], up[1]));
                                eUserEvent.Invoke(new User(up[0], up[1]), true);
                            }
                            catch (Exception e)
                            {
                                break;
                            }
                            break;
                        case "USEROFFLINE":
                            try
                            {
                                a = args[0];
                                up = a.Split(new char[1] { ':' });
                                for (int i = 0; i < OnlineUsers.Count; i++)
                                {
                                    if (OnlineUsers[i].Email.Equals(up[0]))
                                    {
                                        eUserEvent.Invoke(OnlineUsers[i], false);
                                        OnlineUsers.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {

                                break;
                            }
                            break;
                        case "ONLINELIST":
                            try
                            {
                                OnlineUsers = new List<User>(0);
                                for (int i = 0; i < args.Count; i++)
                                {
                                    a = (String)args[i];
                                    up = a.Split(new char[1] { ':' });
                                    if (up.Length == 2)
                                        OnlineUsers.Add(new User(up[0], up[1]));
                                }
                                eUserEvent.Invoke(new User("", ""), true);
                                SocketMessage sm = new SocketMessage("GAMELIST");
                                writeMessage(sm);
                            }
                            catch (Exception e)
                            { }
                            break;
                        case "LOBCHAT":
                            try
                            {
                                eLobbyChat.Invoke(LobbyChatTypes.Global, (String)args[0], (String)args[1]);
                            }
                            catch (Exception e)
                            { }
                            break;
                        case "LOBW":
                            try
                            {
                                String[] s = args[0].Split(new char[1] { ':' });
                                strLastWhisperFrom = s[0];
                                eLobbyChat.Invoke(LobbyChatTypes.Whisper, (String)args[0], (String)args[1]);
                            }
                            catch (Exception e) { };
                            break;
                        case "CHATERROR":
                            try
                            {
                                eLobbyChat.Invoke(LobbyChatTypes.Error, "ERROR", (String)args[0]);
                            }
                            catch (Exception e) { };
                            break;
                        case "CHATINFO":
                            try
                            {
                                eLobbyChat.Invoke(LobbyChatTypes.System, "SYSTEM", (String)args[0]);
                            }
                            catch (Exception e) { };
                            break;
                        case "HOST":
                            try
                            {
                                String t = (string)args[0];
                                String[] ips = t.Split(new char[1] { '?' });
                                HostedGame hg = new HostedGame((string)args[5],(string)args[3],(string)args[2],(string)args[4],"",ips,int.Parse((string)args[1]));
                                hg.strHostName = (string)args[6];
                                hg.intUGameNum = int.Parse((string)args[7]);
                                eGameHost.Invoke(hg, false, false);
                            }
                            catch (Exception e) { };
                            break;
                        case "UNHOST":
                            try
                            {
                                HostedGame hg = new HostedGame();
                                hg.intUGameNum = int.Parse((String)args[0]);
                                eGameHost.Invoke(hg, true, false);
                            }
                            catch (Exception e) { };
                            break;
                        case "GAMELIST":
                            try
                            {
                                String t = (string)args[1];
                                String[] ips = t.Split(new char[1] { '?' });
                                HostedGame hg = new HostedGame((string)args[7], (string)args[4], (string)args[3], (string)args[5], "", ips, int.Parse((string)args[2]));
                                hg.strHostName = (string)args[6];
                                hg.intUGameNum = int.Parse((string)args[0]);
                                eGameHost.Invoke(hg, false, true);
                            }
                            catch (Exception e) { };
                            break;
                        default:
#if (DEBUG)
                            MessageBox.Show(input.getMessage(), "Input from sock");
#endif
                            break;
                    }
                }
                else
                {
                    #if (DEBUG)
                        MessageBox.Show(input.getMessage() , "Input from sock");
                    #endif
                }
                break;
            }
                
        }
        override
        public void handleConnect(String host, Int32 port) 
        {
            try
            {
            eLogEvent.Invoke("CON");
            }
            catch (Exception e) { };
        }
        override
        public void handleDisconnect(String reason, String host, int port) 
        {
            try
            {
                eLogEvent.Invoke("DC");

            }
            catch (Exception e) { }
            unregEvents();
        }
        public class User
        {
            public String Email;
            public String Username;
            public User()
            {
                Email = "";
                Username = "";
            }
            public User(String email, String username)
            {
                Email = email;
                Username = username;
            }
        }
    }
}
