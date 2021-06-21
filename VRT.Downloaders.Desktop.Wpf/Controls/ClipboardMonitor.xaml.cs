using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace VRT.Downloaders.Desktop.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for ClipboardMonitor.xaml
    /// </summary>
    public partial class ClipboardMonitor : UserControl, IActivatableView
    {
        public static readonly DependencyProperty MonitorEnabledProperty
           = DependencyProperty.Register(nameof(MonitorEnabled), typeof(bool), typeof(ClipboardMonitor));

        public static readonly DependencyProperty ProcessClipboardTextCommandProperty
            = DependencyProperty.Register(nameof(ProcessClipboardTextCommand), typeof(ICommand), typeof(ClipboardMonitor));

        private IntPtr _handle;
        private IntPtr _clipboardViewerNext;
        private readonly HwndSourceHook _hwndHook;
        private ISubject<string> _clipboardDataSubject;

        public ClipboardMonitor()
        {
            InitializeComponent();
            _clipboardDataSubject = new BehaviorSubject<string>("");            
            _hwndHook = new HwndSourceHook(WndProcHook);
            Loaded += OnControlLoaded;

            this.WhenActivated(disp =>
            {                
                _clipboardDataSubject
                    .Throttle(TimeSpan.FromMilliseconds(500))
                    .ObserveOnDispatcher()
                    .Subscribe(_ => ProcessClipboardData())
                    .DisposeWith(disp);

                this.WhenAnyValue(p => p.MonitorEnabled)
                    .Subscribe(v => EnableClipboardSniffing(v))
                    .DisposeWith(disp)
                    .Discard();
            }).Discard();
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            if (AsHwndSource(VisualParent) is HwndSource newHwndSource)
            {
                _handle = newHwndSource.Handle;
                newHwndSource.AddHook(_hwndHook);
            }
        }

        private void EnableClipboardSniffing(bool enableSniffer)
        {
            if (_handle == IntPtr.Zero)
                return;

            if (enableSniffer)
            {
                _clipboardViewerNext = ClipboardPInvoke.SetClipboardViewer(_handle);
            }
            else
            {
                ClipboardPInvoke.ChangeClipboardChain(_handle, _clipboardViewerNext);
            }
        }

        public bool MonitorEnabled
        {
            get => (bool)GetValue(MonitorEnabledProperty);
            set => SetValue(MonitorEnabledProperty, value);
        }
        public ICommand ProcessClipboardTextCommand
        {
            get => (ICommand)GetValue(ProcessClipboardTextCommandProperty);
            set => SetValue(ProcessClipboardTextCommandProperty, value);
        }

        private static HwndSource AsHwndSource(DependencyObject dependency)
        {
            if (dependency is not Visual visual)
                return null;
            var result = (HwndSource)PresentationSource.FromVisual(visual);
            return result;
        }
        private IntPtr WndProcHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam,
            ref bool handled)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (msg)
            {
                case WM_CHANGECBCHAIN:
                    if (wParam == _clipboardViewerNext)
                        _clipboardViewerNext = lParam;
                    else
                        CallNextClipboardViewer(msg, wParam, lParam);
                    break;

                case WM_DRAWCLIPBOARD:
                    _clipboardDataSubject.OnNext(null);
                    CallNextClipboardViewer(msg, wParam, lParam);
                    break;
            }
            return IntPtr.Zero;
        }

        private void CallNextClipboardViewer(int msg, IntPtr wParam, IntPtr lParam)
        {
            if (_clipboardViewerNext == IntPtr.Zero)
            {
                return;
            }
            ClipboardPInvoke.SendMessage(_clipboardViewerNext, msg, wParam, lParam).Discard();
        }
        private void ProcessClipboardData()
        {
            var iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Text))
            {               
                var text = (string)iData.GetData(DataFormats.UnicodeText);
                OnClipboardTextData(text);
            }
        }
        private void OnClipboardTextData(string text)
        {
            if (ProcessClipboardTextCommand == null || string.IsNullOrWhiteSpace(text))
                return;
            ProcessClipboardTextCommand.Execute(text);
        }
    }
}
