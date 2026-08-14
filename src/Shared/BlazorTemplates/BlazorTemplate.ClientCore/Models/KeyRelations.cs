namespace BlazorTemplate.ClientCore.Models;

public class KeyRelations<TM, TS>
{
    public KeyRelations(TM? main, IEnumerable<TS>? values)
    {
        Main = main;
        if (values is not null)
            Slaves = [.. values];
    }
    public KeyRelations()
    {

    }
    public TM? Main { get; set; }
    public TS[]? Slaves { get; set; }

    public static implicit operator KeyRelations<TM, TS>((TM?, IEnumerable<TS>?) v) => new(v.Item1, v.Item2);
}