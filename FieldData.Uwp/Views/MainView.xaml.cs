using DataFactory;
using DataFactory.Model;
using FieldData.Core.ViewModels;
using MvvmCross.Base;
using MvvmCross.Platforms.Uap;
using MvvmCross.Platforms.Uap.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace FieldData.Uwp.Views
{
    public sealed partial class MainView : MvxWindowsPage
    {
        #region Fields

        private WriteableBitmap _Field;
        private WriteableBitmap _Image;
        private DispatcherTimer _Timer;

        #endregion Fields

        #region Constructors

        public MainView()
        {
            this.InitializeComponent();
        }

        ~MainView()
        {
            if (_Timer == null) return;
            _Timer.Stop();
            _Timer.Tick -= OnTick;
            _Timer.DisposeIfDisposable();
            _Timer = null;
        }

        #endregion Constructors

        #region Lifecycle

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            if (ViewModel != null)
            {
                var vm = (MainViewModel)ViewModel;
                vm.OpenFileAction = async () => await OpenFileAction();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Task.Run(Init);
        }

        #endregion Lifecycle

        #region Actions

        private async Task Init()
        {
            //  get the field
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
            {
                try
                {
                    _Field = await BitmapFactory.FromContent(new Uri("ms-appx:///assets/field.png"));
                    _Image = BitmapFactory.New((int)MainGrid.ActualWidth, (int)MainGrid.ActualHeight);
                    MainImage.Source = _Image;
                    _Timer = new DispatcherTimer();
                    _Timer.Tick += OnTick;
                    _Timer.Interval = TimeSpan.FromMilliseconds(20);
                    _Timer.Start();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{nameof(MainView)}.{nameof(Init)} - Exception: {ex}");
                }
            });
        }

        private async Task OpenFileAction()
        {
            try
            {
                var picker = new FileOpenPicker();
                picker.ViewMode = PickerViewMode.List;
                picker.FileTypeFilter.Add(".json");
                picker.FileTypeFilter.Add(".txt");

                var file = await picker.PickSingleFileAsync();
                if (file == null)
                {
                    Debug.WriteLine($"{nameof(MainView)}.{nameof(OpenFileAction)} - Action cancelled.");
                    return;
                }
                //  read contents of the file
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        var s = sr.ReadToEnd();
                        ((MainViewModel)ViewModel).OpenFileCommand.Execute(s);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{GetType().Name}.{nameof(OpenFileAction)} - Exception: {ex}");
            }
        }

        #endregion Actions

        #region Render

        private void OnTick(object sender, object e)
        {
            var vm = (MainViewModel)ViewModel;
            var data = vm.Data?.ToList();

            //  render the field
            var bmp = BitmapFactory.New(360, 160);
            using (var context = bmp.GetBitmapContext())
            {
                bmp.Blit(new Rect(0, 0, 360, 160), _Field, new Rect(0, 0, _Field.PixelWidth, _Field.PixelHeight));

                //  draw activities
                if ((vm.Field != null) && (vm.Field.Activities != null) && (vm.Field.Activities.Count > 0))
                {
                    foreach (var activity in vm.Field.Activities)
                    {
                        var p = (activity.State == ActivityState.Ready) ? Colors.Orange : (activity.State == ActivityState.Waiting) ? Colors.LightGreen : Colors.Red;
                        _Image.DrawRectangle((int)(activity.Bounds.X0), (int)(activity.Bounds.Y0), (int)((activity.Bounds.X1 - activity.Bounds.X0)), (int)((activity.Bounds.Y1 - activity.Bounds.Y0)), p);
                        //  queue point
                        _Image.FillEllipse((int)(activity.Bounds.X0) - 3, (int)(activity.Bounds.Y0) - 3, 6, 6, Colors.Black);
                        //  collect point
                        _Image.FillEllipse((int)(activity.Bounds.X0) - 3, (int)(activity.Bounds.Y0) - 3, 6, 6, Colors.Red);
                    }
                }

                if ((data != null) && (data.Count() > 0))
                {
                    foreach (var d in data)
                    {
                        if (d == null) continue;
                        _Image.SetPixel((int)d.X, (int)d.Y, Colors.Black);
                    }
                }
                data = null;
            }
            //  blit to the view
            using (var context = _Image.GetBitmapContext())
            {
                _Image.Blit(new Rect(0, 0, _Image.PixelWidth, _Image.PixelHeight), bmp, new Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight));
            }
        }

        #endregion Render
    }
}
