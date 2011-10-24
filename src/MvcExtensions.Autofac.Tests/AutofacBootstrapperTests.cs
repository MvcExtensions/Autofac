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
    
    using global::Autofac.Core;

    public class AutofacBootstrapperTests
    {
        [Fact]
        public void Should_be_able_to_create_adapter()
        {
            var buildManager = new Mock<IBuildManager>();
            var bootStrapperTaskRegistery = new Mock<IBootstrapperTasksRegistry>();
            var perRequestTaskRegistery = new Mock<IPerRequestTasksRegistry>();
            buildManager.SetupGet(bm => bm.Assemblies).Returns(new[] { GetType().Assembly });

            var bootstrapper = new AutofacBootstrapper(buildManager.Object, bootStrapperTaskRegistery.Object, perRequestTaskRegistery.Object);

            Assert.IsType<AutofacAdapter>(bootstrapper.Adapter);
        }

        [Fact]
        public void Should_be_able_to_load_modules()
        {
            var buildManager = new Mock<IBuildManager>();
            var bootStrapperTaskRegistery = new Mock<IBootstrapperTasksRegistry>();
            var perRequestTaskRegistery = new Mock<IPerRequestTasksRegistry>();
            buildManager.SetupGet(bm => bm.ConcreteTypes).Returns(new[] { typeof(DummyModule) });

            DummyModule.Configured = false;

            var bootstrapper = new AutofacBootstrapper(buildManager.Object, bootStrapperTaskRegistery.Object, perRequestTaskRegistery.Object);

            Assert.NotNull(bootstrapper.Adapter);

            Assert.True(DummyModule.Configured);
        }

        #region Nested type: DummyModule
        private sealed class DummyModule : IModule
        {
            public static bool Configured
            {
                get;
                set;
            }

            #region IModule Members
            public void Configure(IComponentRegistry componentRegistry)
            {
                Configured = true;
            }
            #endregion
        }
        #endregion
    }
}