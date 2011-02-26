using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Octgn.Properties;

namespace Octgn.Launcher
{
	public sealed partial class LobbyHost : Page
	{
		private bool isStarting = false;
        public LobbyHost()
		{

		}

        private void Start(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (isStarting)
            {
                Program.LClient.isHosting = false;
                return; // prevents double-click and such
            }
            if (gameSelector.Game == null)
            {
                Program.LClient.isHosting = false;
                return;
            }

            Program.Game = gameSelector.Game;
            if (!Program.Game.Definition.CheckVersion())
            {
                Program.LClient.isHosting = false;
                return;
            }

            isStarting = true;

            Program.LClient.Host_Game(Program.Game.Definition);
            //NavigationService.GoBack();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(Program.LClient != null)
                Program.LClient.isHosting =  false;

        }
	}
}