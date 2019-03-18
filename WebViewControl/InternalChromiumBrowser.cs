using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using CefSharp.Enums;
using CefSharp.Wpf;
using Microsoft.Win32.SafeHandles;

namespace WebViewControl {

    internal class InternalChromiumBrowser : ChromiumWebBrowser {

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

        internal static class DragCursorProvider {
            [DllImport("kernel32.dll")]
            private static extern IntPtr LoadLibrary(string dllToLoad);

            [DllImport("user32.dll")]
            private static extern IntPtr LoadCursor(IntPtr hInstance, ushort lpCursorName);

            private static readonly Dictionary<DragDropEffects, Cursor> DragCursors;

            static DragCursorProvider() {
                var library = LoadLibrary("ole32.dll");
                    DragCursors = new Dictionary<DragDropEffects, Cursor>() {
                    { DragDropEffects.None, GetCursorFromLib(library, 1) },
                    { DragDropEffects.Move, GetCursorFromLib(library, 2) },
                    { DragDropEffects.Copy, GetCursorFromLib(library, 3) },
                    { DragDropEffects.Link, GetCursorFromLib(library, 4) }
                    // TODO: support black cursors
                };
            }

            private static Cursor GetCursorFromLib(IntPtr library, ushort cursorIndex) {
                var cursorHandle = LoadCursor(library, cursorIndex);
                return CursorInteropHelper.Create(new SafeFileHandle(cursorHandle, false));
            }

            private static DragDropEffects GetDragEffects(DragOperationsMask mask) {
                if ((mask & DragOperationsMask.Every) == DragOperationsMask.Every) {
                    return DragDropEffects.All;
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

            /// <summary>
            /// Get the Windows cursor for the drag effect specified.
            /// </summary>
            /// <param name="operation"></param>
            /// <returns>The drop cursor based on the specified drag operation effect</returns>
            public static Cursor GetCursor(DragOperationsMask operation) {
                var effects = GetDragEffects(operation);
                if (DragCursors.TryGetValue(effects, out var cursor)) {
                    return cursor;
                }
                return Cursors.Arrow;
            }
        }

        protected override void UpdateDragCursor(DragOperationsMask operation) {
            var dragCursor = DragCursorProvider.GetCursor(operation);
            Dispatcher.RunAsyncInUIThread((Action) (() => Mouse.SetCursor(dragCursor)));
            base.UpdateDragCursor(operation);
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
            /// prevent showing default cursor, the appropriate cursor will be set by <see cref=UpdateDragCursor />
            e.Handled = true;
            base.OnGiveFeedback(e);
        }

        #endregion
    }
}
