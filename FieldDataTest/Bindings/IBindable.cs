using System.Collections.Generic;

namespace FieldDataTest.Bindings
{
    public interface IBindable
    {
        Dictionary<string, Binding> Bindings { get; set; }
    }
}
