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
using Spring.Objects.Factory.Config;

#endregion

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
