﻿#pragma checksum "..\..\..\Views\Rezerwacje.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "D9A7774B139C00FC84D4B2D1C4EA38244D6B2913"
//------------------------------------------------------------------------------
// <auto-generated>
//     Ten kod został wygenerowany przez narzędzie.
//     Wersja wykonawcza:4.0.30319.42000
//
//     Zmiany w tym pliku mogą spowodować nieprawidłowe zachowanie i zostaną utracone, jeśli
//     kod zostanie ponownie wygenerowany.
// </auto-generated>
//------------------------------------------------------------------------------

using FirmaTransportowa.Views;
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


namespace FirmaTransportowa.Views {
    
    
    /// <summary>
    /// Rezerwacje
    /// </summary>
    public partial class Rezerwacje : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 15 "..\..\..\Views\Rezerwacje.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox PrywatneBox;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\Views\Rezerwacje.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ZakonczoneBox;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\Views\Rezerwacje.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox PozostałeBox;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\Views\Rezerwacje.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox Zakonczone_i_PrywatneBox;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\Views\Rezerwacje.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox idFilter;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\Views\Rezerwacje.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox personFilter;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\Views\Rezerwacje.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox dataStartFilter;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\Views\Rezerwacje.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox dataEndFilter;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\Views\Rezerwacje.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox dataReservationFilter;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\Views\Rezerwacje.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox carFilter;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\Views\Rezerwacje.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView ListViewReservations;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/FirmaTransportowa;component/views/rezerwacje.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\Rezerwacje.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.PrywatneBox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 15 "..\..\..\Views\Rezerwacje.xaml"
            this.PrywatneBox.Click += new System.Windows.RoutedEventHandler(this.PrywatneBox_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ZakonczoneBox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 16 "..\..\..\Views\Rezerwacje.xaml"
            this.ZakonczoneBox.Click += new System.Windows.RoutedEventHandler(this.ZakonczoneBox_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.PozostałeBox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 17 "..\..\..\Views\Rezerwacje.xaml"
            this.PozostałeBox.Click += new System.Windows.RoutedEventHandler(this.PozostałeBox_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Zakonczone_i_PrywatneBox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 18 "..\..\..\Views\Rezerwacje.xaml"
            this.Zakonczone_i_PrywatneBox.Click += new System.Windows.RoutedEventHandler(this.Zakonczone_i_PrywatneBox_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.idFilter = ((System.Windows.Controls.TextBox)(target));
            
            #line 21 "..\..\..\Views\Rezerwacje.xaml"
            this.idFilter.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.idFilter_TextChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.personFilter = ((System.Windows.Controls.TextBox)(target));
            
            #line 22 "..\..\..\Views\Rezerwacje.xaml"
            this.personFilter.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.personFilter_TextChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.dataStartFilter = ((System.Windows.Controls.TextBox)(target));
            
            #line 23 "..\..\..\Views\Rezerwacje.xaml"
            this.dataStartFilter.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.dataStartFilter_TextChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this.dataEndFilter = ((System.Windows.Controls.TextBox)(target));
            
            #line 24 "..\..\..\Views\Rezerwacje.xaml"
            this.dataEndFilter.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.dataEndFilter_TextChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.dataReservationFilter = ((System.Windows.Controls.TextBox)(target));
            
            #line 25 "..\..\..\Views\Rezerwacje.xaml"
            this.dataReservationFilter.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.dataReservationFilter_TextChanged);
            
            #line default
            #line hidden
            return;
            case 10:
            this.carFilter = ((System.Windows.Controls.TextBox)(target));
            
            #line 26 "..\..\..\Views\Rezerwacje.xaml"
            this.carFilter.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.carFilter_TextChanged);
            
            #line default
            #line hidden
            return;
            case 11:
            this.ListViewReservations = ((System.Windows.Controls.ListView)(target));
            return;
            case 12:
            
            #line 34 "..\..\..\Views\Rezerwacje.xaml"
            ((System.Windows.Controls.GridViewColumnHeader)(target)).Click += new System.Windows.RoutedEventHandler(this.GridViewColumnHeader_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            
            #line 39 "..\..\..\Views\Rezerwacje.xaml"
            ((System.Windows.Controls.GridViewColumnHeader)(target)).Click += new System.Windows.RoutedEventHandler(this.GridViewColumnHeader_Click);
            
            #line default
            #line hidden
            return;
            case 14:
            
            #line 44 "..\..\..\Views\Rezerwacje.xaml"
            ((System.Windows.Controls.GridViewColumnHeader)(target)).Click += new System.Windows.RoutedEventHandler(this.GridViewColumnHeader_Click);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 49 "..\..\..\Views\Rezerwacje.xaml"
            ((System.Windows.Controls.GridViewColumnHeader)(target)).Click += new System.Windows.RoutedEventHandler(this.GridViewColumnHeader_Click);
            
            #line default
            #line hidden
            return;
            case 16:
            
            #line 54 "..\..\..\Views\Rezerwacje.xaml"
            ((System.Windows.Controls.GridViewColumnHeader)(target)).Click += new System.Windows.RoutedEventHandler(this.GridViewColumnHeader_Click);
            
            #line default
            #line hidden
            return;
            case 17:
            
            #line 59 "..\..\..\Views\Rezerwacje.xaml"
            ((System.Windows.Controls.GridViewColumnHeader)(target)).Click += new System.Windows.RoutedEventHandler(this.GridViewColumnHeader_Click);
            
            #line default
            #line hidden
            return;
            case 18:
            
            #line 66 "..\..\..\Views\Rezerwacje.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Modyfikuj_Rezerwacje);
            
            #line default
            #line hidden
            return;
            case 19:
            
            #line 67 "..\..\..\Views\Rezerwacje.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Zakoncz_Rezerwacje);
            
            #line default
            #line hidden
            return;
            case 20:
            
            #line 70 "..\..\..\Views\Rezerwacje.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Generuj_Raport_Rezerwacje);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

