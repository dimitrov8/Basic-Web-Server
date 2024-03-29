﻿namespace BasicWebServer.Server.Common;

using System.Reflection;

public class ServiceCollection : IServiceCollection
{
    private readonly Dictionary<Type, Type> services;

    public ServiceCollection()
    {
        this.services = new Dictionary<Type, Type>();
    }

    public IServiceCollection Add<TService, TImplementation>() where TService : class where TImplementation : TService
    {
        this.services[typeof(TService)] = typeof(TImplementation);

        return this;
    }

    public IServiceCollection Add<TService>() where TService : class
    {
        return this.Add<TService, TService>();
    }

    public object CreateInstance(Type serviceType)
    {
        if (this.services.ContainsKey(serviceType))
        {
            serviceType = this.services[serviceType];
        }
        else if (serviceType.IsInterface)
        {
            throw new InvalidOperationException($"Service {serviceType.FullName} is not registered.");
        }

        ConstructorInfo[] constructors = serviceType.GetConstructors();

        if (constructors.Length > 1)
        {
            throw new InvalidOperationException("Multiple constructors are not supported.");
        }

        var constructor = constructors[0];
        ParameterInfo[] parameters = constructor.GetParameters();
        object[] parameterValues = new object[parameters.Length];

        for (int i = 0; i < parameterValues.Length; i++)
        {
            var parameterType = parameters[i].ParameterType;
            object parameterValue = this.CreateInstance(parameterType);

            parameterValues[i] = parameterValue;
        }

        return constructor.Invoke(parameterValues);
    }

    public TService Get<TService>() where TService : class
    {
        var serviceType = typeof(TService);

        if (!this.services.ContainsKey(serviceType))
        {
            return null;
        }

        var service = this.services[serviceType];

        return (TService)this.CreateInstance(service);
    }
}