﻿#pragma checksum "..\..\..\DeckBuilder\DeckBuilderWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F52B22C4D7D092C276B62377C4EF10B1"
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
using Octgn;
using Octgn.Controls;
using Octgn.DeckBuilder;
using Octgn.Play.Gui;
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


namespace Octgn.DeckBuilder {
    
    
    /// <summary>
    /// DeckBuilderWindow
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class DeckBuilderWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 8 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Octgn.DeckBuilder.DeckBuilderWindow self;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem newSubMenu;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem loadSubMenu;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image cardImage;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl searchTabs;
        
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
            System.Uri resourceLocater = new System.Uri("/OCTGNwLobby;component/deckbuilder/deckbuilderwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
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
            this.self = ((Octgn.DeckBuilder.DeckBuilderWindow)(target));
            return;
            case 2:
            
            #line 13 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.LoadDeckCommand);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 14 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.NewDeckCommand);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 15 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.SaveDeck);
            
            #line default
            #line hidden
            
            #line 15 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.IsDeckOpen);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 16 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.SaveDeckAsHandler);
            
            #line default
            #line hidden
            
            #line 16 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.IsDeckOpen);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 17 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.OpenTabCommand);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 18 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CloseTabCommand);
            
            #line default
            #line hidden
            
            #line 18 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CanCloseTab);
            
            #line default
            #line hidden
            return;
            case 8:
            this.newSubMenu = ((System.Windows.Controls.MenuItem)(target));
            
            #line 40 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            this.newSubMenu.Click += new System.Windows.RoutedEventHandler(this.NewClicked);
            
            #line default
            #line hidden
            return;
            case 9:
            this.loadSubMenu = ((System.Windows.Controls.MenuItem)(target));
            
            #line 43 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            this.loadSubMenu.Click += new System.Windows.RoutedEventHandler(this.LoadClicked);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 47 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.CloseClicked);
            
            #line default
            #line hidden
            return;
            case 11:
            this.cardImage = ((System.Windows.Controls.Image)(target));
            return;
            case 14:
            this.searchTabs = ((System.Windows.Controls.TabControl)(target));
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
            case 12:
            
            #line 62 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            ((System.Windows.Controls.Expander)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.SetActiveSection);
            
            #line default
            #line hidden
            break;
            case 13:
            
            #line 84 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            ((System.Windows.Controls.DataGrid)(target)).SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ElementSelected);
            
            #line default
            #line hidden
            
            #line 84 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            ((System.Windows.Controls.DataGrid)(target)).AddHandler(System.Windows.Input.Keyboard.PreviewKeyDownEvent, new System.Windows.Input.KeyEventHandler(this.DeckKeyDownHandler));
            
            #line default
            #line hidden
            
            #line 85 "..\..\..\DeckBuilder\DeckBuilderWindow.xaml"
            ((System.Windows.Controls.DataGrid)(target)).CellEditEnding += new System.EventHandler<System.Windows.Controls.DataGridCellEditEndingEventArgs>(this.ElementEditEnd);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

