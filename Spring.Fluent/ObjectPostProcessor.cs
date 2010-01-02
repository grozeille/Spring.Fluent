using System;
using System.Collections.Generic;
using System.Text;
using Spring.Objects.Factory.Config;

namespace Spring.Fluent
{
    public class ObjectPostProcessor : IObjectPostProcessor
    {
        private Func<object, string, object> onPostProcessAfterInitialization;
        private Func<object, string, object> onPostProcessBeforeInitialization;

        public ObjectPostProcessor(Func<object, string, object> after, Func<object, string, object> before)
        {
            this.onPostProcessAfterInitialization = after;
            this.onPostProcessBeforeInitialization = before;
        }

        #region IObjectPostProcessor Members

        public object PostProcessAfterInitialization(object instance, string objectName)
        {
            if (onPostProcessAfterInitialization == null)
                return instance;
            return onPostProcessAfterInitialization(instance, objectName);
        }

        public object PostProcessBeforeInitialization(object instance, string name)
        {
            if (onPostProcessBeforeInitialization == null)
                return instance;
            return onPostProcessBeforeInitialization(instance, name);
        }

        #endregion
    }
}
