using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migration.Migrant
{
    [Serializable]
    public class Foo
    {
        private int bar;
        private int baz;

        public Foo(int bar, int baz)
        {
            this.bar = bar;
            this.baz = baz;
        }

        public override string ToString() => $"Foo  {{ bar = {bar}, baz = {baz} }}";
    }
}
