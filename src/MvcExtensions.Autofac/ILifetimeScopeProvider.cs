namespace MvcExtensions.Autofac
{
    using global::Autofac;

    internal interface ILifetimeScopeProvider
    {
        ILifetimeScope GetLifetimeScope();

        void EndLifetimeScope();
    }
}