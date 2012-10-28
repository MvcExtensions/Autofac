#region Copyright
// Copyright (c) 2009 - 2011, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>, hazzik <hazzik@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Autofac
{
    using System;
    using global::Autofac.Builder;

    /// <summary>
    /// </summary>
    public static class RegistrationExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="registration"></param>
        /// <typeparam name="TLimit"></typeparam>
        /// <typeparam name="TActivatorData"></typeparam>
        /// <typeparam name="TStyle"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> InstancePerHttpRequest<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration)
        {
            Invariant.IsNotNull(registration, "registration");

            return registration.InstancePerMatchingLifetimeScope(RequestLifetimeScopeProvider.HttpRequestTag);
        }
    }
}