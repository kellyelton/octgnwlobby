﻿#pragma checksum "..\..\..\Launcher\Serve.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "891702FE9FC90679BF9F5A47F412BBC5"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Windows.Controls;
using Octgn.Controls;
using Octgn.Launcher;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Octgn.Launcher {
    
    
    /// <summary>
    /// Serve
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class Serve : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 16 "..\..\..\Launcher\Serve.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Octgn.Launcher.GameSelector gameSelector;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\Launcher\Serve.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox nickBox;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\Launcher\Serve.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox portBox;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\Launcher\Serve.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock natWarning;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\Launcher\Serve.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton v6Box;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\Launcher\Serve.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ItemsControl ipList;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\..\Launcher\Serve.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock noIpLabel;
        
        #line default
        #line hidden
        
        
        #line 92 "..\..\..\Launcher\Serve.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton v4Box;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\..\Launcher\Serve.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ItemsControl ipv4List;
        
        #line default
        #line hidden
        
        
        #line 109 "..\..\..\Launcher\Serve.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock webIPBlock;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\..\Launcher\Serve.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Documents.Run webIPText;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/OCTGNwLobby;component/launcher/serve.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Launcher\Serve.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.gameSelector = ((Octgn.Launcher.GameSelector)(target));
            return;
            case 2:
            this.nickBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.portBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.natWarning = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.v6Box = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 6:
            this.ipList = ((System.Windows.Controls.ItemsControl)(target));
            return;
            case 8:
            this.noIpLabel = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            
            #line 89 "..\..\..\Launcher\Serve.xaml"
            ((System.Windows.Documents.Hyperlink)(target)).Click += new System.Windows.RoutedEventHandler(this.GoToWebSite);
            
            #line default
            #line hidden
            return;
            case 10:
            this.v4Box = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 11:
            this.ipv4List = ((System.Windows.Controls.ItemsControl)(target));
            return;
            case 13:
            this.webIPBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 14:
            this.webIPText = ((System.Windows.Documents.Run)(target));
            return;
            case 15:
            
            #line 110 "..\..\..\Launcher\Serve.xaml"
            ((System.Windows.Documents.Hyperlink)(target)).Click += new System.Windows.RoutedEventHandler(this.CopyIP);
            
            #line default
            #line hidden
            return;
            case 16:
            
            #line 117 "..\..\..\Launcher\Serve.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Start);
            
            #line default
            #line hidden
            return;
            case 17:
            
            #line 118 "..\..\..\Launcher\Serve.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 7:
            
            #line 73 "..\..\..\Launcher\Serve.xaml"
            ((System.Windows.Documents.Hyperlink)(target)).Click += new System.Windows.RoutedEventHandler(this.CopyIP);
            
            #line default
            #line hidden
            break;
            case 12:
            
            #line 103 "..\..\..\Launcher\Serve.xaml"
            ((System.Windows.Documents.Hyperlink)(target)).Click += new System.Windows.RoutedEventHandler(this.CopyIP);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

