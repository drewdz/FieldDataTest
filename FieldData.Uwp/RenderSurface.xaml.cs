using FieldData.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FieldData.Uwp
{
    public sealed partial class RenderSurface : UserControl
    {
        #region Fields

        private WriteableBitmap _Field;

        #endregion Fields

        #region Constructors

        public RenderSurface()
        {
            this.InitializeComponent();
            CompositionTarget.Rendering += OnRendering;
            DataContextChanged += OnDataContextChanged;
        }

        ~RenderSurface()
        {
            ViewModel.PropertyChanged -= OnPropertyChanged;
            CompositionTarget.Rendering -= OnRendering;
            DataContextChanged -= OnDataContextChanged;
        }

        #endregion Constructors

        #region Properties

        private MainViewModel ViewModel
        {
            get { return (DataContext == null) ? null : (MainViewModel)DataContext; }
        }

        #endregion Properties

        #region Initialize

        private async Task Init()
        {
            //  get the field
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
            {
                try
                {
                    _Field = await BitmapFactory.FromContent(new Uri("ms-appx:///assets/field.png"));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{nameof(RenderSurface)}.{nameof(Init)}~ - Exception: {ex}");
                }
            });
        }

        #endregion Initialize

        #region Event Handlers

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            ViewModel.PropertyChanged += OnPropertyChanged;
            Task.Run(Init);
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }

        private void OnRendering(object sender, object e)
        {
            Debug.WriteLine("rendering");

        }

        #endregion Event Handlers
    }
}
