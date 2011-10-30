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