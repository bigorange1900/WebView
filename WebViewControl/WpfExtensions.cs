using System;
using System.Windows;
using System.Windows.Threading;

namespace WebViewControl {

    internal static class WpfExtensions {

        public static void RunAsyncInUIThread(this Dispatcher dispatcher, Action action, DispatcherPriority? priority = null) {
            if (priority == null && Application.Current.Dispatcher.CheckAccess()) {
                action();
            } else if (!dispatcher.HasShutdownStarted) {
                if (priority != null) {
                    dispatcher.BeginInvoke(action, priority.Value);
                } else {
                    dispatcher.BeginInvoke(action);
                }
            }
        }
    }
}
