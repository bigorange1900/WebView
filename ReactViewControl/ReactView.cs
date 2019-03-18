﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WebViewControl;

namespace ReactViewControl {

    public partial class ReactView : UserControl, IReactView, IViewModule {

        private static ReactViewRender cachedView;

        private static ReactViewRender CreateReactViewInstance() {
            var result = cachedView;
            cachedView = null;
            Application.Current.Dispatcher.RunAsyncInUIThread(() => {
                if (cachedView == null && !Application.Current.Dispatcher.HasShutdownStarted) {
                    cachedView = new ReactViewRender(true);
                }
            }, 
            DispatcherPriority.Background);
            return result ?? new ReactViewRender(true);
        }

        private readonly ReactViewRender view;

        public ReactView(bool usePreloadedWebView = true) {
            if (usePreloadedWebView) {
                view = CreateReactViewInstance();
            } else {
                view = new ReactViewRender(false);
            }
            SetResourceReference(StyleProperty, typeof(ReactView)); // force styles to be inherited, must be called after view is created otherwise view might be null
            Content = view;
            Dispatcher.RunAsyncInUIThread(() => {
                if (!view.IsDisposing) {
                    if (EnableHotReload) {
                        view.EnableHotReload(Source);
                    }
                    view.LoadComponent(this);
                }
            },
            DispatcherPriority.Send);

            FocusManager.SetIsFocusScope(this, true);
            FocusManager.SetFocusedElement(this, view.FocusableElement);
        }

        ~ReactView() {
            Dispose();
        }

        public void Dispose() {
            view.Dispose();
            GC.SuppressFinalize(this);
        }

        public ResourceUrl DefaultStyleSheet { get => view.DefaultStyleSheet; set => view.DefaultStyleSheet = value; }

        public IViewModule[] Plugins { get => view.Plugins; set => view.Plugins = value; }

        public T WithPlugin<T>() {
            return view.WithPlugin<T>();
        }

        public bool EnableDebugMode { get => view.EnableDebugMode; set => view.EnableDebugMode = value; }

        public bool EnableHotReload { get; set; }

        public bool IsReady => view.IsReady;

        public event Action Ready {
            add { view.Ready += value; }
            remove { view.Ready -= value; }
        }

        public event Action<UnhandledAsyncExceptionEventArgs> UnhandledAsyncException {
            add { view.UnhandledAsyncException += value; }
            remove { view.UnhandledAsyncException -= value; }
        }

        public event Func<string, Stream> CustomResourceRequested {
            add { view.CustomResourceRequested += value; }
            remove { view.CustomResourceRequested -= value; }
        }

        public void ShowDeveloperTools() {
            view.ShowDeveloperTools();
        }

        public void CloseDeveloperTools() {
            view.CloseDeveloperTools();
        }

        string IViewModule.JavascriptSource => JavascriptSource;

        protected virtual string JavascriptSource => null;

        string IViewModule.NativeObjectName => NativeObjectName;

        protected virtual string NativeObjectName => null;

        protected virtual string ModuleName => null;

        string IViewModule.Name => ModuleName;

        string IViewModule.Source => Source;

        protected virtual string Source => null; // used for hot reload

        object IViewModule.CreateNativeObject() => CreateNativeObject();

        protected virtual object CreateNativeObject() {
            return null;
        }

        void IViewModule.Bind(IExecutionEngine engine) {
            throw new Exception("Cannot bind ReactView");
        }

        IExecutionEngine IViewModule.ExecutionEngine => ExecutionEngine;

        protected IExecutionEngine ExecutionEngine => view; // ease access in generated code
    }
}