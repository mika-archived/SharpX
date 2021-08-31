using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SharpX.Compiler.Composition.Abstractions
{
    public class SafeStack<T>
    {
        private readonly Stack<T> _stack;

        public int Count => _stack.Count;

        public SafeStack()
        {
            _stack = new Stack<T>();
        }

        public SafeStack(SafeStack<T> innerStack)
        {
            _stack = new Stack<T>(innerStack._stack);
        }

        [return: NotNullIfNotNull("default")]
        public T? SafePeek(T? @default = default)
        {
            return _stack.Count > 0 ? _stack.Peek() : @default;
        }

        public T Peek()
        {
            return _stack.Peek();
        }

        public void Push(T data)
        {
            _stack.Push(data);
        }

        public T Pop()
        {
            return _stack.Pop();
        }

        public bool Contains(T data)
        {
            return _stack.Contains(data);
        }

        public void Clear()
        {
            _stack.Clear();
        }
    }
}