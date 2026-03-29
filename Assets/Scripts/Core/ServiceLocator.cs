// Assets/Scripts/Core/ServiceLocator.cs
using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> services = new();

    public static void Register<T>(T service)
    {
        services[typeof(T)] = service;
    }

    public static T Get<T>()
    {
        if (services.TryGetValue(typeof(T), out var service))
            return (T)service;
        throw new InvalidOperationException($"Service {typeof(T)} not registered");
    }

    public static bool TryGet<T>(out T service)
    {
        if (services.TryGetValue(typeof(T), out var s))
        {
            service = (T)s;
            return true;
        }
        service = default;
        return false;
    }

    public static void Clear()
    {
        services.Clear();
    }
}