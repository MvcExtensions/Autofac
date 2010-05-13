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
    using System.Diagnostics;
    using System.Linq;

    using Microsoft.Practices.ServiceLocation;

    using ContainerBuilder = global::Autofac.ContainerBuilder;
    using IContainer = global::Autofac.IContainer;
    using RegisterExtension = global::Autofac.RegistrationExtensions;
    using ResolveExtension = global::Autofac.ResolutionExtensions;

    /// <summary>
    /// Defines an adapter class which with backed by Autofac <seealso cref="Container">Container</seealso>.
    /// </summary>
    public class AutofacAdapter : ServiceLocatorImplBase, IServiceRegistrar, IServiceInjector, IDisposable
    {
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacAdapter"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public AutofacAdapter(IContainer container)
        {
            Invariant.IsNotNull(container, "container");

            Container = container;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="AutofacAdapter"/> is reclaimed by garbage collection.
        /// </summary>
        [DebuggerStepThrough]
        ~AutofacAdapter()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public IContainer Container
        {
            get;
            private set;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Registers the service and its implementation with the lifetime behavior.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="lifetime">The lifetime of the service.</param>
        /// <returns></returns>
        public virtual IServiceRegistrar RegisterType(string key, Type serviceType, Type implementationType, LifetimeType lifetime)
        {
            Invariant.IsNotNull(serviceType, "serviceType");
            Invariant.IsNotNull(implementationType, "implementationType");

            ContainerBuilder builder = new ContainerBuilder();

            var registration = RegisterExtension.RegisterType(builder, implementationType).As(serviceType);

            if (!string.IsNullOrEmpty(key))
            {
                registration = registration.Named(key, serviceType);
            }

            if (lifetime == LifetimeType.PerRequest)
            {
                registration.PerRequestScoped();
            }
            else if (lifetime == LifetimeType.Singleton)
            {
                registration.SingleInstance();
            }
            else
            {
                registration.InstancePerDependency();
            }

            builder.Update(Container);

            return this;
        }

        /// <summary>
        /// Registers the instance as singleton.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public virtual IServiceRegistrar RegisterInstance(string key, Type serviceType, object instance)
        {
            Invariant.IsNotNull(serviceType, "serviceType");
            Invariant.IsNotNull(instance, "instance");

            ContainerBuilder builder = new ContainerBuilder();

            if (string.IsNullOrEmpty(key))
            {
                RegisterExtension.RegisterInstance(builder, instance).As(serviceType).ExternallyOwned();
            }
            else
            {
                RegisterExtension.RegisterInstance(builder, instance).Named(key, serviceType).As(serviceType).ExternallyOwned();
            }

            builder.Update(Container);

            return this;
        }

        /// <summary>
        /// Injects the matching dependences.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public virtual void Inject(object instance)
        {
            if (instance != null)
            {
                ResolveExtension.InjectUnsetProperties(Container, instance);
            }
        }

        /// <summary>
        /// Gets the matching instance for the given type and key.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            return key != null ? ResolveExtension.Resolve(Container, key, serviceType) : ResolveExtension.Resolve(Container, serviceType);
        }

        /// <summary>
        /// Gets all the instances for the given type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            Type type = typeof(IEnumerable<>).MakeGenericType(serviceType);

            object instances = ResolveExtension.Resolve(Container, type);

            return ((IEnumerable)instances).Cast<object>();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        [DebuggerStepThrough]
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                Container.Dispose();
            }

            disposed = true;
        }
    }
}