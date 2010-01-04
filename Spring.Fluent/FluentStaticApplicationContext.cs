using System;
using System.Collections.Generic;
using System.Text;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;
using Spring.Context.Support;
using Spring.Context;
using Spring.Objects.Factory;

namespace Spring.Fluent
{
    /// <summary>
    /// StaticApplicationContext with addional methods with a Fluent way
    /// </summary>
    public class FluentStaticApplicationContext : StaticApplicationContext
    {
        private IObjectDefinitionFactory objectDefinitionFactory;

        private List<IObjectDefinitionBuilder> builderList = new List<IObjectDefinitionBuilder>();

        private VariablePlaceholderConfigurer variablePlaceHolderConfigurer;

        private List<IVariableSource> variableSources = new List<IVariableSource>();

        public VariablePlaceholderConfigurer VariablePlaceholderConfigurer
        {
            get
            {
                return this.variablePlaceHolderConfigurer;
            }
        }

        #region constructors
        public FluentStaticApplicationContext()
            : base()
        {
            this.objectDefinitionFactory = new DefaultObjectDefinitionFactory();
        }

        public FluentStaticApplicationContext(IApplicationContext parentContext)
            : base(parentContext)
        {
            this.objectDefinitionFactory = new DefaultObjectDefinitionFactory();
        }

        public FluentStaticApplicationContext(string name, IApplicationContext parentContext)
            : base(name, parentContext)
        {
            this.objectDefinitionFactory = new DefaultObjectDefinitionFactory();
        }
        #endregion

        protected override void OnPreRefresh()
        {
            foreach (var b in builderList)
            {
                this.RegisterObjectDefinition(b.Name, b.ObjectDefinition);
                foreach (var after in b.OnAfterInitializationObjects)
                {
                    this.ObjectFactory.AddObjectPostProcessor(after);
                }
                foreach (var before in b.OnBeforeInitializationObjects)
                {
                    this.ObjectFactory.AddObjectPostProcessor(before);
                }
            }
            base.OnPreRefresh();
        }

        public T GetObject<T>(string name)
        {
            var obj = this.GetObject(name);

            if (obj is IFactoryObject)
            {
                return (T)((IFactoryObject)obj).GetObject();
            }

            return (T)obj;
        }

        private void RefreshParent(AbstractApplicationContext context)
        {
            var parent = context.ParentContext as AbstractApplicationContext;
            if (parent != null)
                this.RefreshParent(parent);

            context.Refresh();
        }


        public void RefreshAll()
        {
            this.RefreshParent(this);
        }

        public T GetObject<T>()
        {
            var names = this.GetObjectNamesForType(typeof(T));
            if (names.Length == 0)
                throw new UnsatisfiedDependencyException(string.Format("No object definition for type {0}", typeof(T)));
            if (names.Length > 1)
                throw new UnsatisfiedDependencyException(string.Format("More than one object definition for type {0}", typeof(T)));

            return (T)this.GetObject(names[0], typeof(T));
        }

        public ObjectDefinitionBuilder<T> RegisterObject<T>(string name)
        {
            var definitionBuilder = new ObjectDefinitionBuilder<T>(this, this.objectDefinitionFactory, name);
            builderList.Add(definitionBuilder);
            return definitionBuilder;
        }

		public ObjectDefinitionBuilder<T> RegisterObject<T>(string name, string parentName)
        {
            var definitionBuilder = new ObjectDefinitionBuilder<T>(this, this.objectDefinitionFactory, name, parentName);
            builderList.Add(definitionBuilder);
            return definitionBuilder;
        }

        internal ObjectDefinitionBuilder<T> RegisterObjectDefinitionBuilder<T>(ObjectDefinitionBuilder<T> definitionBuilder)
        {
            builderList.Add(definitionBuilder);
            return definitionBuilder;
        }

        public FluentStaticApplicationContext AddVariableSource(IVariableSource source)
        {
            if (this.variablePlaceHolderConfigurer == null)
            {
                this.variablePlaceHolderConfigurer = new VariablePlaceholderConfigurer();
                this.AddObjectFactoryPostProcessor(this.variablePlaceHolderConfigurer);
                this.variablePlaceHolderConfigurer.VariableSources = this.variableSources;
            }

            this.variableSources.Add(source);

            return this;
        }
    }
}
