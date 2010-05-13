#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Autofac
{
    /// <summary>
    /// This is copied from Autofac source code, Only the namespace is changed.
    /// Extends registration syntax for common web scenarios.
    /// </summary>
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Share one instance of the component within the context of a single
        /// HTTP request.
        /// </summary>
        /// <typeparam name="TLimit">Registration limit type.</typeparam>
        /// <typeparam name="TStyle">Registration style.</typeparam>
        /// <typeparam name="TActivatorData">Activator data type.</typeparam>
        /// <param name="registration">The registration to configure.</param>
        /// <returns>A registration builder allowing further configuration of the component.</returns>
        public static global::Autofac.Builder.IRegistrationBuilder<TLimit, TActivatorData, TStyle> PerRequestScoped<TLimit, TActivatorData, TStyle>(this global::Autofac.Builder.IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration)
        {
            Invariant.IsNotNull(registration, "registration");

            return registration.InstancePerMatchingLifetimeScope("httpRequest");
        }
    }
}