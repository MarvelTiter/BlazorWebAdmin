using System.Collections;

namespace Project.Constraints.Models;

public static class SelectItemHelper
{
    public static Dictionary<string, string> ParseEnumValues<T>(this SelectItem<T> self)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        foreach (var item in self)
        {
            _ = result.TryAdd(item.Value!.ToString()!, item.Label);
        }
        return result;
    }
    public static SelectItem<string> Convert(this Dictionary<string, string> self)
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
    private readonly List<Options<T>> items = new();
    private readonly Lazy<Dictionary<string, string>> lazyDictionary;
    private bool frozen;
    public SelectItem()
    {
        lazyDictionary = new Lazy<Dictionary<string, string>>(() =>
        {
            return this.ParseEnumValues();
        });
    }
    public bool Contains(string label, Func<T, bool> predicate) => items.Find(o => o.Label == label && predicate.Invoke(o.Value)) != null;
    public SelectItem<T> Add(string label, T value)
    {
        if (frozen) throw new InvalidOperationException();
        items.Add(new Options<T>(label, value));
        return this;
    }

    public SelectItem<T> AddRange(IEnumerable<Options<T>> options, bool frozen = false)
    {
        if (frozen) throw new InvalidOperationException();
        items.AddRange(options);
        if (!frozen)
        {
            Frozen();
        }
        return this;
    }

    public void Frozen() => frozen = true;
    public bool IsFrozen => frozen;
    public Dictionary<string, string> EnumValues
    {
        get
        {
            if (!frozen) throw new InvalidOperationException();
            return lazyDictionary.Value;
        }
    }
    public void Clear() => items.Clear();

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

    public static implicit operator Dictionary<string, string>(SelectItem<T> options) => options.EnumValues;
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