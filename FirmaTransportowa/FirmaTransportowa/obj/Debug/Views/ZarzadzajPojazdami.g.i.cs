﻿#pragma checksum "..\..\..\Views\ZarzadzajPojazdami.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "85AC46B81B9EE2E6ECF067CDDAD8C16D6147A97FFDBC8D9AA3A12B2962493DF6"
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
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
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
    /// ZarzadzajPojazdami
    /// </summary>
    public partial class ZarzadzajPojazdami : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 16 "..\..\..\Views\ZarzadzajPojazdami.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox idFilter;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\Views\ZarzadzajPojazdami.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox registrationFiler;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\Views\ZarzadzajPojazdami.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox carSupervisorFilter;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\Views\ZarzadzajPojazdami.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView carList;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\Views\ZarzadzajPojazdami.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CarStatistics;
        
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
            System.Uri resourceLocater = new System.Uri("/FirmaTransportowa;component/views/zarzadzajpojazdami.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\ZarzadzajPojazdami.xaml"
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
            this.idFilter = ((System.Windows.Controls.TextBox)(target));
            
            #line 16 "..\..\..\Views\ZarzadzajPojazdami.xaml"
            this.idFilter.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.idFilter_TextChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.registrationFiler = ((System.Windows.Controls.TextBox)(target));
            
            #line 17 "..\..\..\Views\ZarzadzajPojazdami.xaml"
            this.registrationFiler.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.registrationFiler_TextChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.carSupervisorFilter = ((System.Windows.Controls.TextBox)(target));
            
            #line 18 "..\..\..\Views\ZarzadzajPojazdami.xaml"
            this.carSupervisorFilter.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.carSupervisorFilter_TextChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.carList = ((System.Windows.Controls.ListView)(target));
            return;
            case 5:
            
            #line 25 "..\..\..\Views\ZarzadzajPojazdami.xaml"
            ((System.Windows.Controls.GridViewColumnHeader)(target)).Click += new System.Windows.RoutedEventHandler(this.GridViewColumnHeader_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 30 "..\..\..\Views\ZarzadzajPojazdami.xaml"
            ((System.Windows.Controls.GridViewColumnHeader)(target)).Click += new System.Windows.RoutedEventHandler(this.GridViewColumnHeader_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 35 "..\..\..\Views\ZarzadzajPojazdami.xaml"
            ((System.Windows.Controls.GridViewColumnHeader)(target)).Click += new System.Windows.RoutedEventHandler(this.GridViewColumnHeader_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 42 "..\..\..\Views\ZarzadzajPojazdami.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Dodaj_Pojazd);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 43 "..\..\..\Views\ZarzadzajPojazdami.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Usun_Pojazd);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 44 "..\..\..\Views\ZarzadzajPojazdami.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Zmiana_Opiekuna);
            
            #line default
            #line hidden
            return;
            case 11:
            this.CarStatistics = ((System.Windows.Controls.Button)(target));
            
            #line 45 "..\..\..\Views\ZarzadzajPojazdami.xaml"
            this.CarStatistics.Click += new System.Windows.RoutedEventHandler(this.CarStatistics_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            
            #line 48 "..\..\..\Views\ZarzadzajPojazdami.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Generuj_Raport);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

