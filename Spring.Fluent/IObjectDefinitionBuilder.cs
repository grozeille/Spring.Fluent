using System;
using System.Collections.Generic;
using System.Text;
using Spring.Objects.Factory.Support;

namespace Spring.Fluent
{
    public interface IObjectDefinitionBuilder
    {
        string Name { get; }

        AbstractObjectDefinition ObjectDefinition { get; }

        IList<ObjectPostProcessor> OnBeforeInitializationObjects { get; }

        IList<ObjectPostProcessor> OnAfterInitializationObjects { get; }
    }
}
