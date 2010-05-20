#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Autofac
{
    using System;
    using System.Web;

    using Microsoft.Practices.ServiceLocation;
    using ContainerBuilder = global::Autofac.ContainerBuilder;
    using RegisterExtensions = global::Autofac.RegistrationExtensions;

    /// <summary>
    /// Defines a <see cref="HttpApplication"/> which is uses <seealso cref="AutofacBootstrapper"/>.
    /// </summary>
    public class AutofacMvcApplication : ExtendedMvcApplication
    {
        private static readonly Type httpContextKey = typeof(AutofacAdapter);

        private AutofacAdapter ApplicationAdapter
        {
            get
            {
                return (AutofacAdapter)Bootstrapper.Adapter;
            }
        }

        private IDisposable CurrentAdapter
        {
            get
            {
                HttpContextBase context = ApplicationAdapter.GetInstance<HttpContextBase>();

                return context.Items.Contains(httpContextKey) ? (IDisposable)context.Items[httpContextKey] : null;
            }

            set
            {
                ApplicationAdapter.GetInstance<HttpContextBase>().Items[httpContextKey] = value;
            }
        }

        /// <summary>
        /// Creates the bootstrapper.
        /// </summary>
        /// <returns></returns>
        protected override IBootstrapper CreateBootstrapper()
        {
            return new AutofacBootstrapper(BuildManagerWrapper.Current);
        }

        /// <summary>
        /// Executes when the application starts.
        /// </summary>
        protected override void OnStart()
        {
            ServiceLocator.SetLocatorProvider(GetLocator);
        }

        /// <summary>
        /// Executes after the registered <see cref="PerRequestTask"/> disposes.
        /// </summary>
        protected override void OnPerRequestTasksDisposed()
        {
            IDisposable adapter = CurrentAdapter;

            if (adapter != null)
            {
                adapter.Dispose();
            }
        }

        private IServiceLocator GetLocator()
        {
            Func<AutofacAdapter> createNew = () =>
                                                 {
                                                     AutofacAdapter adapter = new AutofacAdapter(ApplicationAdapter.Container.BeginLifetimeScope(WebLifetime.Request));

                                                     ContainerBuilder builder = new ContainerBuilder();

                                                     RegisterExtensions.RegisterInstance(builder, adapter).As<IServiceRegistrar>();
                                                     RegisterExtensions.RegisterInstance(builder, adapter).As<IServiceLocator>();
                                                     RegisterExtensions.RegisterInstance(builder, adapter).As<IServiceInjector>();
                                                     RegisterExtensions.RegisterInstance(builder, adapter).As<ContainerAdapter>();

                                                     builder.Update(adapter.Container.ComponentRegistry);

                                                     return adapter;
                                                 };

            return (CurrentAdapter ?? (CurrentAdapter = createNew())) as IServiceLocator;
        }
    }
}