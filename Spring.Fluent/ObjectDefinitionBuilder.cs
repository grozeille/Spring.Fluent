#region License

/*
 * Copyright © 2002-2005 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using Spring.Objects.Factory.Support;
using Spring.Context.Support;
using Spring.Objects.Factory.Config;

#endregion

namespace Spring.Fluent
{
    public class ObjectDefinitionBuilder<T> : IObjectDefinitionBuilder
    {
        private ObjectDefinitionBuilder builder;

        private string name;

        private FluentStaticApplicationContext context;

        private IObjectDefinitionFactory factory;

        public ObjectDefinitionBuilder(FluentStaticApplicationContext context, IObjectDefinitionFactory factory, string name)
        {
            this.name = name;
            this.factory = factory;
            this.context = context;
            this.builder = ObjectDefinitionBuilder.RootObjectDefinition(factory, typeof(T));
            this.OnBeforeInitializationObjects = new List<ObjectPostProcessor>();
            this.OnAfterInitializationObjects = new List<ObjectPostProcessor>();
        }

        private ObjectDefinitionBuilder(FluentStaticApplicationContext context, IObjectDefinitionFactory factory)
        {
            this.factory = factory;
            this.context = context;
            this.builder = ObjectDefinitionBuilder.RootObjectDefinition(factory, typeof(T));
            this.OnBeforeInitializationObjects = new List<ObjectPostProcessor>();
            this.OnAfterInitializationObjects = new List<ObjectPostProcessor>();
        }

        public string Name { get { return this.name; } }
        public AbstractObjectDefinition ObjectDefinition { get { return this.builder.ObjectDefinition; } }
        public AbstractObjectDefinition RawObjectDefinition { get { return this.builder.RawObjectDefinition; } }
        public AbstractApplicationContext ApplicationContext { get { return this.context; } }

        public IList<ObjectPostProcessor> OnBeforeInitializationObjects { get; private set; }

        public IList<ObjectPostProcessor> OnAfterInitializationObjects { get; private set; }

        public ObjectDefinitionBuilder<T> AddConstructorArgs(params object[] args)
        {
            foreach (var arg in args)
                this.builder.AddConstructorArg(arg);
            return this;
        }

        #region Wrapper methods
        public ObjectDefinitionBuilder<T> AddPostProcessAfterInitialization(Func<T, T> afterCreationHandler)
        {
            var obj = new ObjectPostProcessor((instance, objectName) =>
            {
                if (objectName.Equals(this.Name) && instance is T)
                    return afterCreationHandler((T)instance);

                return instance;
            }, null);
            this.OnAfterInitializationObjects.Add(obj);

            return this;
        }

        public ObjectDefinitionBuilder<T> AddPostProcessBeforeInitialization(Func<T, T> beforeCreationHandler)
        {
            var obj = new ObjectPostProcessor((instance, objectName) =>
            {
                if (objectName.Equals(this.Name) && instance is T)
                    return beforeCreationHandler((T)instance);

                return null;
            }, null);
            this.OnBeforeInitializationObjects.Add(obj);
            return this;
        }

        public ObjectDefinitionBuilder<T> AddConstructorArg(object value)
        {
            this.builder.AddConstructorArg(value);
            return this;
        }
        public ObjectDefinitionBuilder<T> AddConstructorArgReference(string objectName)
        {
            this.builder.AddConstructorArgReference(objectName);
            return this;
        }
        public ObjectDefinitionBuilder<T> AddDependsOn(string objectName)
        {
            this.builder.AddDependsOn(objectName);
            return this;
        }
        public ObjectDefinitionBuilder<T> AddPropertyReference(Expression<Func<T, object>> property, string objectName)
        {
            this.builder.AddPropertyReference(this.GetPropertyName(property), objectName);
            return this;
        }
        public ObjectDefinitionBuilder<T> AddPropertyValue(Expression<Func<T, object>> property, object value)
        {
            this.builder.AddPropertyValue(this.GetPropertyName(property), value);
            return this;
        }
        public ObjectDefinitionBuilder<T1> AddPropertyInnerObject<T1>(Expression<Func<T, object>> property)
        {
            var innerObject = new ObjectDefinitionBuilder<T1>(this.context, this.factory);
            innerObject.name = ObjectDefinitionReaderUtils.GenerateObjectName(innerObject.builder.RawObjectDefinition, this.context);
            this.context.RegisterObjectDefinitionBuilder(innerObject);
            this.builder.AddPropertyReference(this.GetPropertyName(property), innerObject.Name);
            return innerObject;
        }

        private string GetPropertyName(Expression<Func<T, object>> property)
        {
            var propertyInfo = (property.Body as MemberExpression).Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }

            var propertyName = propertyInfo.Name;

            return propertyName;
        }

        private string GeMethodName(Expression<Func<T, object>> method)
        {
            var methodInfo = (method.Body as MemberExpression).Member as MethodInfo;
            if (methodInfo == null)
            {
                throw new ArgumentException("The lambda expression 'method' should point to a valid Method");
            }

            var methodName = methodInfo.Name;

            return methodName;
        }

        public ObjectDefinitionBuilder<T> SetAbstract(bool flag)
        {
            builder.SetAbstract(flag);
            return this;
        }
        public ObjectDefinitionBuilder<T> SetAutowireMode(AutoWiringMode autowireMode)
        {
            builder.SetAutowireMode(autowireMode);
            return this;
        }
        public ObjectDefinitionBuilder<T> SetDependencyCheck(DependencyCheckingMode dependencyCheck)
        {
            builder.SetDependencyCheck(dependencyCheck);
            return this;
        }
        public ObjectDefinitionBuilder<T> SetDestroyMethodName(Expression<Func<T, object>> method)
        {
            this.builder.SetDestroyMethodName(this.GeMethodName(method));
            return this;
        }
        public ObjectDefinitionBuilder<T> SetFactoryMethod(Expression<Func<T, object>> method)
        {
            this.builder.SetFactoryMethod(this.GeMethodName(method));
            return this;
        }
        public ObjectDefinitionBuilder<T> SetFactoryObject(string factoryObject, string factoryMethod)
        {
            builder.SetFactoryObject(factoryObject, factoryMethod);
            return this;
        }
        public ObjectDefinitionBuilder<T> SetInitMethodName(Expression<Func<T, object>> method)
        {
            this.builder.SetInitMethodName(this.GeMethodName(method));
            return this;
        }
        public ObjectDefinitionBuilder<T> SetLazyInit(bool lazy)
        {
            builder.SetLazyInit(lazy);
            return this;
        }
        public ObjectDefinitionBuilder<T> SetResourceDescription(string resourceDescription)
        {
            builder.SetResourceDescription(resourceDescription);
            return this;
        }
        public ObjectDefinitionBuilder<T> SetSingleton(bool singleton)
        {
            builder.SetSingleton(singleton);
            return this;
        }
        #endregion

        public ObjectDefinitionBuilder<T> SetParent(ObjectDefinitionBuilder<T> parent)
        {
            this.builder.ObjectDefinition.ParentName = parent.Name;
            return this;
        }
    }
}
