using System;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace ContentNegotiation.Formatters;

public class PlainTextTableOutputFormatter : TextOutputFormatter
{
    public PlainTextTableOutputFormatter()
    {
        SupportedMediaTypes.Add("text/primitives-table");
        SupportedEncodings.Add(Encoding.UTF8);
    }

    protected override bool CanWriteType(Type? type)
    {
        if(type == null)
            return false;
        return typeof(IEnumerable<>).IsAssignableFrom(type) 
            || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>));
    }
    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var response = context.HttpContext.Response;
        var writer = new StreamWriter(response.Body, selectedEncoding);

        var items = ((IEnumerable<object>)context.Object).ToList();
        if (!items.Any()) return;

        var type = items[0].GetType();
        var properties = type.GetProperties();

        var header = properties.Select(p => p.Name).ToArray();
        var colWidths = header.Select((h, i) =>

            Math.Max(h.Length, items.Max(item => FormatValue(properties[i].GetValue(item)).Length))
        ).ToArray();

        await writer.WriteLineAsync(FrormatRow(header, colWidths));
        await writer.WriteLineAsync(FormatSeparator(colWidths));

        foreach (var item in items)
        {
            var values = properties.Select(p => FormatValue(p.GetValue(item))).ToArray();
            await writer.WriteLineAsync(FrormatRow(values, colWidths));
        }
        await writer.FlushAsync();
    }

    private string FrormatRow(string[] values, int[] colWidths)
    {
       
        var cells = values.Select((v, i) => v.PadRight(colWidths[i]));

        return "| " + string.Join(" | ", cells) + " |";
    }

    private string FormatValue(object? val)
    {
        return val?.ToString() ?? "";
    }

    private string FormatSeparator(int[] colWidths)
    {
       
        var bars = colWidths.Select(w => new string('-', w)).ToArray();
        return "|-" + string.Join("-|-", bars) + "-|";
    }
}
