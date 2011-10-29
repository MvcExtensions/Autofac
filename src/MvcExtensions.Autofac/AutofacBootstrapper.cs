#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Autofac
{
    using System;
    using System.Linq;
    using System.Web;

    using global::Autofac;
    using global::Autofac.Core;

    /// <summary>
    /// Defines a <seealso cref="Bootstrapper">Bootstrapper</seealso> which is backed by <seealso cref="AutofacAdapter"/>.
    /// </summary>
    public class AutofacBootstrapper : Bootstrapper
    {
        private static readonly Type moduleType = typeof(IModule);

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
        /// Creates the container adapter.
        /// </summary>
        /// <returns></returns>
        protected override ContainerAdapter CreateAdapter()
        {
            var builder = new ContainerBuilder();

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

            ILifetimeScope container = ((AutofacAdapter)Adapter).Container;

            builder.Update(container.ComponentRegistry);
        }
    }
}