#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Autofac
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using global::Autofac;

    /// <summary>
    /// Defines an adapter class which is backed by Autofac <seealso cref="Container">Container</seealso>.
    /// </summary>
    public class AutofacAdapter : ContainerAdapter
    {
        private ILifetimeScopeProvider lifetimeScopeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacAdapter"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public AutofacAdapter(ILifetimeScope container)
        {
            Invariant.IsNotNull(container, "container");

            Container = container;
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public ILifetimeScope Container
        {
            get;
            private set;
        }

        private ILifetimeScope RequestLifetimeScope
        {
            get
            {
                if (lifetimeScopeProvider == null)
                {
                    lifetimeScopeProvider = new RequestLifetimeScopeProvider(Container);
                }

                return lifetimeScopeProvider.GetLifetimeScope();
            }
        }

        /// <summary>
        /// Registers the service and its implementation with the lifetime behavior.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="lifetime">The lifetime of the service.</param>
        /// <returns></returns>
        public override IServiceRegistrar RegisterType(Type serviceType, Type implementationType, LifetimeType lifetime)
        {
            Invariant.IsNotNull(serviceType, "serviceType");
            Invariant.IsNotNull(implementationType, "implementationType");

            var builder = new ContainerBuilder();

            var registration = builder.RegisterType(implementationType).As(serviceType);

            switch (lifetime)
            {
                case LifetimeType.PerRequest:
                    registration.InstancePerHttpRequest();
                    break;
                case LifetimeType.Singleton:
                    registration.SingleInstance();
                    break;
                default:
                    registration.InstancePerDependency();
                    break;
            }

            builder.Update(Container.ComponentRegistry);

            return this;
        }

        /// <summary>
        /// Registers the instance as singleton.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public override IServiceRegistrar RegisterInstance(Type serviceType, object instance)
        {
            Invariant.IsNotNull(serviceType, "serviceType");
            Invariant.IsNotNull(instance, "instance");

            var builder = new ContainerBuilder();
            
            builder.RegisterInstance(instance).As(serviceType).ExternallyOwned();

            builder.Update(Container.ComponentRegistry);

            return this;
        }

        /// <summary>
        /// Injects the matching dependences.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public override void Inject(object instance)
        {
            if (instance != null)
            {
                RequestLifetimeScope.InjectUnsetProperties(instance);
            }
        }

        /// <summary>
        /// Gets the matching instance for the given type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        protected override object DoGetService(Type serviceType)
        {
            return RequestLifetimeScope.Resolve(serviceType);
        }

        /// <summary>
        /// Gets all the instances for the given type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        protected override IEnumerable<object> DoGetServices(Type serviceType)
        {
            Type type = typeof(IEnumerable<>).MakeGenericType(serviceType);

            object instances = RequestLifetimeScope.Resolve(type);

            return ((IEnumerable)instances).Cast<object>();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        protected override void DisposeCore()
        {
            Container.Dispose();
        }
    }
}