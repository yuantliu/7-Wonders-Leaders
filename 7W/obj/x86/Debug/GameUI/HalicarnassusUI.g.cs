﻿#pragma checksum "..\..\..\..\GameUI\HalicarnassusUI.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9AE6AC8FF553F5A8443F4FC6F112C8AA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.2012
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace SevenWonders {
    
    
    /// <summary>
    /// HalicarnassusUI
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class HalicarnassusUI : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\..\GameUI\HalicarnassusUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image cardImage;
        
        #line default
        #line hidden
        
        
        #line 7 "..\..\..\..\GameUI\HalicarnassusUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cardComboBox;
        
        #line default
        #line hidden
        
        
        #line 8 "..\..\..\..\GameUI\HalicarnassusUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button confirmButton;
        
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
            System.Uri resourceLocater = new System.Uri("/7W;component/gameui/halicarnassusui.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\GameUI\HalicarnassusUI.xaml"
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
            
            #line 4 "..\..\..\..\GameUI\HalicarnassusUI.xaml"
            ((SevenWonders.HalicarnassusUI)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.cardImage = ((System.Windows.Controls.Image)(target));
            return;
            case 3:
            this.cardComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 7 "..\..\..\..\GameUI\HalicarnassusUI.xaml"
            this.cardComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.cardComboBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.confirmButton = ((System.Windows.Controls.Button)(target));
            
            #line 8 "..\..\..\..\GameUI\HalicarnassusUI.xaml"
            this.confirmButton.Click += new System.Windows.RoutedEventHandler(this.confirmButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

