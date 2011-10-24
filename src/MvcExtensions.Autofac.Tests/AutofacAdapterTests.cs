#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Autofac.Tests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Web;
    using Moq;
    
    using Xunit;
    using Xunit.Extensions;
    
    using global::Autofac;
    using global::Autofac.Core;
    using global::Autofac.Core.Lifetime;

    public class AutofacAdapterTests
    {
        private readonly AutofacAdapter adapter;

        public AutofacAdapterTests()
        {
            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(x => x.Items).Returns(new Hashtable());
            var builder = new ContainerBuilder();
            builder.Register(c => httpContextMock.Object).As<HttpContextBase>().InstancePerDependency();
            adapter = new AutofacAdapter(builder.Build());
        }

        [Fact]
        public void Dispose_should_also_dispose_container()
        {
            var lifetimeScope = new Mock<ILifetimeScope>();
            var localAdapter = new AutofacAdapter(lifetimeScope.Object);

            lifetimeScope.Setup(c => c.Dispose());

            localAdapter.Dispose();

            lifetimeScope.VerifyAll();
        }

        [Theory]
        [InlineData(LifetimeType.Transient)]
        [InlineData(LifetimeType.Singleton)]
        [InlineData(LifetimeType.PerRequest)]
        public void Should_be_able_to_register_with_lifetime_type(LifetimeType lifetime)
        {
            adapter.RegisterType(null, typeof(DummyObject), typeof(DummyObject), lifetime);

            IComponentRegistration registration;

            adapter.Container.ComponentRegistry.TryGetRegistration(new TypedService(typeof(DummyObject)), out registration);

            Assert.NotNull(registration);

            if (lifetime == LifetimeType.PerRequest)
            {
                Assert.Equal(registration.Sharing, InstanceSharing.Shared);
                Assert.IsType<MatchingScopeLifetime>(registration.Lifetime);
            }
            else if (lifetime == LifetimeType.Singleton)
            {
                Assert.Equal(registration.Sharing, InstanceSharing.Shared);
                Assert.IsType<RootScopeLifetime>(registration.Lifetime);
            }
            else
            {
                Assert.Equal(registration.Sharing, InstanceSharing.None);
                Assert.IsType<CurrentScopeLifetime>(registration.Lifetime);
            }
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("")]
        public void Should_be_able_to_register_instance(string key)
        {
            adapter.RegisterInstance(key, typeof(DummyObject), new DummyObject());

            IComponentRegistry registry = adapter.Container.ComponentRegistry;
            IComponentRegistration registration;

            if (string.IsNullOrEmpty(key))
            {
                registry.TryGetRegistration(new TypedService(typeof(DummyObject)), out registration);
            }
            else
            {
                registry.TryGetRegistration(new KeyedService(key, typeof(DummyObject)), out registration);
            }

            Assert.NotNull(registration);
            Assert.Equal(registration.Ownership, InstanceOwnership.ExternallyOwned);
        }

        [Fact]
        public void Should_be_able_to_inject()
        {
            adapter.RegisterAsTransient<DummyObject>();

            var dummy = new DummyObject2();

            adapter.Inject(dummy);

            Assert.NotNull(dummy.Dummy);
        }

        [Fact]
        public void Should_be_able_to_get_instance_by_type()
        {
            adapter.RegisterAsSingleton<DummyObject>();

            Assert.NotNull(adapter.GetService(typeof(DummyObject)));
        }

        [Fact]
        public void Should_be_able_to_get_instance_by_type_and_key()
        {
            adapter.RegisterType("foo", typeof(DummyObject), typeof(DummyObject), LifetimeType.Singleton);

            Assert.NotNull(adapter.GetService<DummyObject>("foo"));
        }

        [Fact]
        public void Should_be_able_to_get_all_instances()
        {
            adapter.RegisterAsTransient<DummyObject>();

            IEnumerable<object> instances = adapter.GetServices(typeof(DummyObject));

            Assert.NotEmpty(instances);
        }

        #region Nested type: DummyObject
        public class DummyObject
        {
        }
        #endregion

        #region Nested type: DummyObject2
        public class DummyObject2
        {
            public DummyObject Dummy
            {
                get;
                set;
            }
        }
        #endregion
    }
}