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

    using ILifetimeScope = global::Autofac.ILifetimeScope;

    /// <summary>
    /// Defines a <see cref="HttpApplication"/> which is uses <seealso cref="AutofacBootstrapper"/>.
    /// </summary>
    public class AutofacMvcApplication : ExtendedMvcApplication
    {
        private static readonly Type httpContextKey = typeof(AutofacAdapter);

        private AutofacAdapter CurrentAdapter
        {
            get
            {
                return Context.Items.Contains(httpContextKey) ? (AutofacAdapter)Context.Items[httpContextKey] : null;
            }

            set
            {
                Context.Items[httpContextKey] = value;
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
        /// Executes after the registered <see cref="IPerRequestTask"/> disposes.
        /// </summary>
        protected override void OnPerRequestTasksDisposed()
        {
            AutofacAdapter adapter = CurrentAdapter;

            if (adapter != null)
            {
                adapter.Dispose();
            }
        }

        private IServiceLocator GetLocator()
        {
            AutofacAdapter currentLocator = CurrentAdapter;

            if (currentLocator == null)
            {
                ILifetimeScope scope = ((AutofacAdapter)Bootstrapper.ServiceLocator).Container.BeginLifetimeScope(WebLifetime.Request);
                currentLocator = new AutofacAdapter(scope);

                CurrentAdapter = currentLocator;
            }

            return currentLocator;
        }
    }
}