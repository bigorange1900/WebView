﻿using NUnit.Framework;

namespace Tests.ReactView {

    [Ignore("Needs browser setup")]
    public class InnerViewTests : ReactViewTestBase {

        protected override void InitializeView() {
            TargetView.AutoShowInnerView = true;
            base.InitializeView();
        }

        [Test(Description = "Tests inner view load")]
        public void InnerViewIsLoaded() {
            var innerViewLoaded = false;
            var innerView = new InnerViewModule();
            innerView.Loaded += () => innerViewLoaded = true;

            TargetView.AttachInnerView(innerView, "test");

            WaitFor(() => innerViewLoaded, "inner view module load");
            Assert.IsTrue(innerViewLoaded);
        }

        [Test(Description = "Tests inner view load")]
        public void InnerViewMethodIsExecuted() {
            var methodCalled = false;
            var innerView = new InnerViewModule();
            innerView.MethodCalled += () => methodCalled = true;

            TargetView.AttachInnerView(innerView, "test");
            innerView.TestMethod();

            WaitFor(() => methodCalled, "method called");
            Assert.IsTrue(methodCalled);
        }
    }
}
