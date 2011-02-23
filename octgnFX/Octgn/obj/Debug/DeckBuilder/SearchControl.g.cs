﻿#pragma checksum "..\..\..\DeckBuilder\SearchControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F2707DB35687AA0FEBF5459FD63CB94E"
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
using Octgn.Data;
using Octgn.DeckBuilder;
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
    /// SearchControl
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class SearchControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 7 "..\..\..\DeckBuilder\SearchControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Octgn.DeckBuilder.SearchControl self;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\DeckBuilder\SearchControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ItemsControl filtersList;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\DeckBuilder\SearchControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ItemsControl filterList;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\DeckBuilder\SearchControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid resultsGrid;
        
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
            System.Uri resourceLocater = new System.Uri("/OCTGNwLobby;component/deckbuilder/searchcontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\DeckBuilder\SearchControl.xaml"
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
            this.self = ((Octgn.DeckBuilder.SearchControl)(target));
            return;
            case 2:
            this.filtersList = ((System.Windows.Controls.ItemsControl)(target));
            return;
            case 4:
            this.filterList = ((System.Windows.Controls.ItemsControl)(target));
            return;
            case 5:
            
            #line 64 "..\..\..\DeckBuilder\SearchControl.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.RefreshSearch);
            
            #line default
            #line hidden
            return;
            case 6:
            this.resultsGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 74 "..\..\..\DeckBuilder\SearchControl.xaml"
            this.resultsGrid.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ResultCardSelected);
            
            #line default
            #line hidden
            
            #line 75 "..\..\..\DeckBuilder\SearchControl.xaml"
            this.resultsGrid.AddHandler(System.Windows.Input.Keyboard.PreviewKeyDownEvent, new System.Windows.Input.KeyEventHandler(this.ResultKeyDownHandler));
            
            #line default
            #line hidden
            
            #line 75 "..\..\..\DeckBuilder\SearchControl.xaml"
            this.resultsGrid.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.ResultDoubleClick);
            
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
            case 3:
            
            #line 35 "..\..\..\DeckBuilder\SearchControl.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.AddFilter);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

