using System.Text;

namespace Project.Web.Shared.Components;

public partial class ConsoleOutput
{
    private static Lazy<HtmlStreamWriter> outputWriter = new(static () => new());
    class HtmlStreamWriter : TextWriter
    {
        private readonly StringBuilder output = new();
        private readonly TextWriter originalOut;
        public HtmlStreamWriter()
        {
            originalOut = Console.Out;
            Console.SetOut(this);
        }
        public string Output => output.ToString();
        public override void Write(char value)
        {
            Write(value.ToString());
        }

        public override void Write(string? value)
        {
            originalOut.Write(value);
            output.Append(value);
        }

        public override Encoding Encoding => Encoding.UTF8;

        public void Test()
        {
            originalOut.WriteLine($"<7>TEST {DateTime.Now}");
        }
    }

    private void Refresh()
    {
        outputWriter.Value.Test();
        StateHasChanged();
    }

}
