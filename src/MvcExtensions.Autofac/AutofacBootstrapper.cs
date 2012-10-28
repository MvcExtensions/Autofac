#region Copyright
// Copyright (c) 2009 - 2011, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>, hazzik <hazzik@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Autofac
{
    using System;
    using System.Linq;
    using System.Web;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using global::Autofac;
    using global::Autofac.Core;

    /// <summary>
    /// Defines a <seealso cref="Bootstrapper">Bootstrapper</seealso> which is backed by <seealso cref="AutofacAdapter"/>.
    /// </summary>
    public class AutofacBootstrapper : Bootstrapper
    {
        private static readonly Type moduleType = typeof(IModule);
        private static bool startWasCalled;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacBootstrapper"/> class.
        /// </summary>
        /// <param name="buildManager">The build manager.</param>
        /// <param name="bootstrapperTasks">The bootstrapper tasks.</param>
        /// <param name="perRequestTasks">The per request tasks.</param>
        public AutofacBootstrapper(IBuildManager buildManager, IBootstrapperTasksRegistry bootstrapperTasks, IPerRequestTasksRegistry perRequestTasks)
            : base(buildManager, bootstrapperTasks, perRequestTasks)
        {
        }

        /// <summary>
        /// Starts this bootstrapper
        /// </summary>
        public static void Run()
        {
            // Guard against multiple calls. All Start calls are made on the same thread, so no lock needed here.
            if (startWasCalled)
            {
                return;
            }

            startWasCalled = true;
            Current = new AutofacBootstrapper(BuildManagerWrapper.Current, BootstrapperTasksRegistry.Current, PerRequestTasksRegistry.Current);
            DynamicModuleUtility.RegisterModule(typeof(Module));
            DynamicModuleUtility.RegisterModule(typeof(PerWebRequestLifestyleModule));
        }

        /// <summary>
        /// Creates the container adapter.
        /// </summary>
        /// <returns></returns>
        protected override ContainerAdapter CreateAdapter()
        {
            var builder = new ContainerBuilder();

            builder.Register(c => new HttpContextWrapper(HttpContext.Current)).As<HttpContextBase>().InstancePerDependency();

            var adapter = new AutofacAdapter(builder.Build());

            return adapter;
        }

        /// <summary>
        /// Loads the container specific modules.
        /// </summary>
        protected override void LoadModules()
        {
            var builder = new ContainerBuilder();

            BuildManager.ConcreteTypes
                .Where(type => moduleType.IsAssignableFrom(type) && type.HasDefaultConstructor())
                .Select(Activator.CreateInstance)
                .Cast<IModule>()
                .Each(builder.RegisterModule);

            var container = ((AutofacAdapter)Adapter).Container;

            builder.Update(container.ComponentRegistry);
        }
    }
}