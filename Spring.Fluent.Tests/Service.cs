using System;
using System.Collections.Generic;
using System.Text;

namespace Spring.Fluent.Tests
{
    public class Service : IService
    {
        public IRepository Repository { get; set; }

        public string SayHello()
        {
            return this.Repository.SayHello();
        }
    }
}
