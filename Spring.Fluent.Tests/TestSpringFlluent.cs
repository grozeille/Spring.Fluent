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
using System.Linq;
using System.Text;
using NUnit.Framework;
using Spring.Objects.Factory.Config;

#endregion

namespace Spring.Fluent.Tests
{
    [TestFixture]
    public class TestSpringFlluent : IVariableSource
    {
        [Test]
        public void TestWithAutowiring()
        {
            var context = new FluentStaticApplicationContext();

            context.RegisterObject<Service>("Service")
                .SetAutowireMode(AutoWiringMode.ByName);

            context.RegisterObject<Repository>("Repository")
                .AddPropertyValue(i => i.Name, "Mathias");

            context.Refresh();

            var result = context.GetObject<IService>("Service")
                .SayHello();

            Assert.AreEqual("Hello Mathias!", result);
        }

        [Test]
        public void TestWithPropertyRef()
        {
            var context = new FluentStaticApplicationContext();

            context.RegisterObject<Service>("Service")
                .AddPropertyReference(i => i.Repository, "Repository");

            context.RegisterObject<Repository>("Repository")
                .AddPropertyValue(i => i.Name, "Mathias");

            context.Refresh();

            var result = context.GetObject<IService>("Service")
                .SayHello();

            Assert.AreEqual("Hello Mathias!", result);
        }

        [Test]
        public void TestVariableSource()
        {
            var context = new FluentStaticApplicationContext();

            context.RegisterObject<Service>("Service")
                .SetAutowireMode(AutoWiringMode.ByName);

            context.RegisterObject<Repository>("Repository")
                .AddPropertyValue(i => i.Name, "${name}");

            context.AddVariableSource(this);

            context.Refresh();

            var result = context.GetObject<IService>("Service")
                .SayHello();

            Assert.AreEqual("Hello Mathias!", result);
        }

        [Test]
        public void TestPostProcess()
        {
            var context = new FluentStaticApplicationContext();

            context.RegisterObject<Service>("Service")
                .AddPropertyReference(i => i.Repository, "Repository");

            context.RegisterObject<Repository>("Repository")
                .AddPostProcessAfterInitialization(i =>
                {
                    i.Name = "Mathias";
                    return i;
                });

            context.Refresh();

            var result = context.GetObject<IService>("Service")
                .SayHello();

            Assert.AreEqual("Hello Mathias!", result);
        }

        [Test]
        public void TestAop()
        {
        }

        [Test]
        public void TestParentChild()
        {

        }

        [Test]
        public void TestInnerObjectDefinition()
        {
            var context = new FluentStaticApplicationContext();

            context.RegisterObject<Service>("Service")
                .AddPropertyInnerObject<Repository>(i => i.Repository)
                .AddPropertyValue(i => i.Name, "Mathias");

            // register another "Repository" to test inner object name generation
            context.RegisterObject<Repository>("Repository");

            context.Refresh();

            var result = context.GetObject<IService>("Service")
                .SayHello();

            Assert.AreEqual("Hello Mathias!", result);
        }

        [Test]
        public void TestParentContext()
        {

        }

        #region IVariableSource Members

        public bool CanResolveVariable(string name)
        {
            return name.Equals("name");
        }

        public string ResolveVariable(string name)
        {
            return name.Equals("name") ? "Mathias" : null;
        }

        #endregion
    }
}