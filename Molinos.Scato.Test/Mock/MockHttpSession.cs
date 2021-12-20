using System.Collections.Generic;
using System.Web;

namespace Molinos.Scato.Test.Mock
{
    public class MockHttpSession : HttpSessionStateBase
    {
        private readonly Dictionary<string, object> sessionDictionary = new Dictionary<string, object>();

        public override object this[string name]
        {
            get { return sessionDictionary[name]; }
            set { sessionDictionary[name] = value; }
        }
    }
}

