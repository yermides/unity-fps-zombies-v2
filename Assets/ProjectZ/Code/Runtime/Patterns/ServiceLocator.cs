using System;
using System.Collections.Generic;

namespace ProjectZ.Code.Runtime.Patterns
{
    public class ServiceLocator
    {
        private static ServiceLocator _instance;
        public static ServiceLocator Instance => _instance ??= new ServiceLocator();
        
        private readonly Dictionary<Type, object> _registeredServices;

        private ServiceLocator()
        {
            _registeredServices = new Dictionary<Type, object>();
        }

        public T GetService<T>()
        {
            // throw error if not found
            if (!_registeredServices.TryGetValue(typeof(T), out var registeredService))
            {
                throw new Exception("Service not registerer!");
            }

            return (T)registeredService;
        }

        public void RegisterService<T>(T service)
        {
            if (_registeredServices.TryGetValue(typeof(T), out var registeredService))
            {
                throw new Exception("Service already registered, you will overwrite it!");
            }

            _registeredServices.Add(typeof(T), service);
        }

        public void DeregisterService<T>()
        {
            _registeredServices.Remove(typeof(T));
        }

        private bool Exists<T>()
        {
            return _registeredServices.TryGetValue(typeof(T), out _);
        }
    }
}