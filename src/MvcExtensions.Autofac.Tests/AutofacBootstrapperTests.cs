#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Autofac.Tests
{
    using Moq;
    using Xunit;

    using IComponentRegistry = global::Autofac.Core.IComponentRegistry;
    using IModule = global::Autofac.Core.IModule;

    public class AutofacBootstrapperTests
    {
        [Fact]
        public void Should_be_able_to_create_service_locator()
        {
            var buildManager = new Mock<IBuildManager>();
            buildManager.SetupGet(bm => bm.Assemblies).Returns(new[] { GetType().Assembly });

            var bootstrapper = new AutofacBootstrapper(buildManager.Object);

            Assert.IsType<AutofacAdapter>(bootstrapper.ServiceLocator);
        }

        [Fact]
        public void Should_be_able_to_load_modules()
        {
            var buildManager = new Mock<IBuildManager>();
            buildManager.SetupGet(bm => bm.ConcreteTypes).Returns(new[] { typeof(DummyModule) });

            DummyModule.Configured = false;

            var bootstrapper = new AutofacBootstrapper(buildManager.Object);

            Assert.NotNull(bootstrapper.ServiceLocator);

            Assert.True(DummyModule.Configured);
        }

        private sealed class DummyModule : IModule
        {
            public static bool Configured { get; set; }

            public void Configure(IComponentRegistry componentRegistry)
            {
                Configured = true;
            }
        }
    }
}