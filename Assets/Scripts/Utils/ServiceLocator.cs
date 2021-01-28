﻿using System;
using System.Collections.Generic;
using Functional.Type;
using UnityEngine;

public struct UnavailableServiceError 
{
    public static readonly string MESSAGE = "Unvailable Service";
}

public static class ServiceLocator
{   
    private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static void Register<T>(T serviceInstance)
    {
        Debug.Log("Register instance of type: " + typeof(T));
        services[typeof(T)] = serviceInstance;
    }

    public static bool Resolve<T>(out T registeredService)
    {        
        if (!services.TryGetValue(typeof(T), out object service))
        {
            registeredService = default;
            return false;
        }

        registeredService = (T) service;
        return true;
    }

    public static Either<UnavailableServiceError, T> Resolve<T>()
    {
        if (!services.TryGetValue(typeof(T), out object service))
        {
            return new UnavailableServiceError();
        }

        return (T) service;
    }

    public static void Reset()
    {
        services.Clear();
    }
}
