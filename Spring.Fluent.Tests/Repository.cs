using System;
using System.Collections.Generic;
using System.Text;

namespace Spring.Fluent.Tests
{
    public class Repository : IRepository
    {
        public string Name { get; set; }

        public string SayHello()
        {
            return string.Format("Hello {0}!", this.Name);
        }
    }
}
