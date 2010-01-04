using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Spring.Objects.Factory.Config;
using Spring.Aop.Support;
using Spring.Aop.Framework;
using System.Text.RegularExpressions;
using AopAlliance.Aop;

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
			var context = new FluentStaticApplicationContext();
			
            context.RegisterObject<Repository>("TargetRepository")
                .AddPropertyValue(i => i.Name, "Mathias");

			context.RegisterObject<RegularExpressionMethodPointcutAdvisor>("Advisor")
				.AddPostProcessAfterInitialization(delegate(RegularExpressionMethodPointcutAdvisor i)
				{
					i.Pattern = ".*SayHello";
					return i;
				})
				.AddPropertyValue(i => i.Advice, new TestAdvice());
			
			context.RegisterObject<ProxyFactoryObject>("Repository")
				.AddPostProcessBeforeInitialization(delegate(ProxyFactoryObject pfo)
				{
					pfo.AddAdvice(context.GetObject<IAdvice>("Advisor"));
					pfo.Target = context.GetObject<IRepository>("TargetRepository");
					return pfo;
				});
			
            context.Refresh();

            var result = context.GetObject<IService>("Service")
                .SayHello();

            Assert.AreEqual("Intercepted: Hello Mathias!", result);
        }

        [Test]
        public void TestParentChild()
        {
			var context = new FluentStaticApplicationContext();

			context.RegisterObject<Service>("TemplateService")
				.AddPropertyReference(i => i.Repository, "Repository")
				.SetAbstract(true);
			
            context.RegisterObject<Service>("Service", "TemplateService");

            context.RegisterObject<Repository>("Repository")
                .AddPropertyValue(i => i.Name, "Mathias");

            context.Refresh();

            var result = context.GetObject<IService>("Service")
                .SayHello();

            Assert.AreEqual("Hello Mathias!", result);
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