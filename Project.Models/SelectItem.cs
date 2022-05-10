using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public static class SelectItemHelper
    {
        public static SelectItem<string> Convert(this Dictionary<string,string> self)
        {
            SelectItem<string> options = new SelectItem<string>();
            foreach (var item in self)
            {
                options.Add(item.Value, item.Key);
            }
            return options;
        }
        public static SelectItem<T> Convert<T>(this IEnumerable<T> self, Func<T, string> func)
        {
            SelectItem<T> options = new SelectItem<T>();
            foreach (T item in self)
            {
                options.Add(func(item), item);
            }
            return options;
        }

        public static SelectItem<string> ConvertStringOptions<T>(this IEnumerable<T> self, Action<SelectItem<string>, T> action)
        {
            SelectItem<string> options = new SelectItem<string>();
            foreach (T item in self)
            {
                action(options, item);
            }
            return options;
        }
    }

    public class SelectItem<T> : IEnumerable<Options<T>>
    {
        List<Options<T>> items = new List<Options<T>>();
        public SelectItem<T> Add(string label, T value)
        {
            items.Add(new Options<T>(label, value));
            return this;
        }

        private class Enumerator : IEnumerator<Options<T>>
        {
            public Enumerator(List<Options<T>> options)
            {
                items = options.ToArray();
            }
            private int index;
            private Options<T>[] items;
            public Options<T> Current => items[index++];

            object IEnumerator.Current => Current;

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                return index < items.Length;
            }

            public void Reset()
            {
                index = 0;
            }
        }

        public IEnumerator<Options<T>> GetEnumerator()
        {
            return new Enumerator(items);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(items);
        }
    }

    public class Options<T>
    {
        public Options(string label, T value)
        {
            Label = label;
            Value = value;
        }
        public string Label { get; set; }
        public T Value { get; set; }
    }
}
