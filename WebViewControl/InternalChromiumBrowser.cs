using System.Windows;
using System.Windows.Input;
using CefSharp.Enums;
using CefSharp.Wpf;

namespace WebViewControl {

    internal class InternalChromiumBrowser : ChromiumWebBrowser {

        public InternalChromiumBrowser() {
            DragOver += OnDragOver;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            e.Handled = false; // let the mouse event be fired
        }

        internal void CreateBrowser() {
            CreateOffscreenBrowser(new Size(200, 200));
        }

        #region Drag Drop Cursors
        // TODO temporary until this pull request gets into cefsharp
        // https://github.com/cefsharp/CefSharp/pull/2691

        private static DragDropEffects GetDragEffects(DragOperationsMask mask) {
            if ((mask & DragOperationsMask.Every) == DragOperationsMask.Every) {
                return DragDropEffects.Scroll | DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
            }
            if ((mask & DragOperationsMask.Copy) == DragOperationsMask.Copy) {
                return DragDropEffects.Copy;
            }
            if ((mask & DragOperationsMask.Move) == DragOperationsMask.Move) {
                return DragDropEffects.Move;
            }
            if ((mask & DragOperationsMask.Link) == DragOperationsMask.Link) {
                return DragDropEffects.Link;
            }
            return DragDropEffects.None;
        }

        private DragDropEffects currentDragDropEffects;

        protected override void UpdateDragCursor(DragOperationsMask operation) {
            currentDragDropEffects = GetDragEffects(operation);
            base.UpdateDragCursor(operation);
        }

        private void OnDragOver(object sender, DragEventArgs e) {
            e.Effects = currentDragDropEffects;
            e.Handled = true;
        }

        #endregion
    }
}
