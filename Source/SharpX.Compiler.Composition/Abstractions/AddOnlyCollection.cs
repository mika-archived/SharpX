using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SharpX.Compiler.Composition.Abstractions
{
    public class AddOnlyCollection<T> : ReadOnlyCollection<T>
    {
        public AddOnlyCollection() : base(new List<T>()) { }

        public void Add(T item)
        {
            Items.Add(item);
        }
    }
}