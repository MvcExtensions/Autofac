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
    using Xunit.Extensions;

    using ContainerBuilder = global::Autofac.ContainerBuilder;
    using CurrentScopeLifetime = global::Autofac.Core.Lifetime.CurrentScopeLifetime;
    using IComponentRegistration = global::Autofac.Core.IComponentRegistration;
    using ILifetimeScope = global::Autofac.ILifetimeScope;
    using InstanceOwnership = global::Autofac.Core.InstanceOwnership;
    using InstanceSharing = global::Autofac.Core.InstanceSharing;
    using KeyedService = global::Autofac.Core.KeyedService;
    using RootScopeLifetime = global::Autofac.Core.Lifetime.RootScopeLifetime;
    using TypedService = global::Autofac.Core.TypedService;

    public class AutofacAdapterTests
    {
        private readonly AutofacAdapter adapter;

        public AutofacAdapterTests()
        {
            adapter = new AutofacAdapter(new ContainerBuilder().Build());
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
                Assert.IsType<CurrentScopeLifetime>(registration.Lifetime);
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

            var registry = adapter.Container.ComponentRegistry;
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

            var instances = adapter.GetServices(typeof(DummyObject));

            Assert.NotEmpty(instances);
        }

        public class DummyObject
        {
        }

        public class DummyObject2
        {
            public DummyObject Dummy { get; set; }
        }
    }
}