#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Autofac
{    
    using System.Web;

    using RegisterExtensions = global::Autofac.RegistrationExtensions;

    /// <summary>
    /// Defines a <see cref="HttpApplication"/> which is uses <seealso cref="AutofacBootstrapper"/>.
    /// </summary>
    public class AutofacMvcApplication : ExtendedMvcApplication
    {
        /// <summary>
        /// Creates the bootstrapper.
        /// </summary>
        /// <returns></returns>
        protected override IBootstrapper CreateBootstrapper()
        {
            return new AutofacBootstrapper(BuildManagerWrapper.Current, BootstrapperTasksRegistry.Current, PerRequestTasksRegistry.Current);
        }        
    }
}