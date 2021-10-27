using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Delegates.Reports
{
    public interface IFormatter
    {
        Func<string, string, string> MakeItem { get; }
        Func<string, string> MakeCaption { get; }
        Func<string> BeginList { get; }
        Func<string> EndList { get; }
    }

    public class HTMLFormatter : IFormatter
    {
        public Func<string, string, string> MakeItem
            => (valueType, entry) => $"<li><b>{valueType}</b>: {entry}";
        public Func<string, string> MakeCaption => (caption) => $"<h1>{caption}</h1>";
        public Func<string> BeginList => () => "<ul>";
        public Func<string> EndList => () => "</ul>";
    }

    public class MarkdownFormatter : IFormatter
    {
        public Func<string, string, string> MakeItem
            => (valueType, entry) => $" * **{valueType}**: {entry}\n\n";
        public Func<string, string> MakeCaption => (caption) => $"## {caption}\n\n";
        public Func<string> BeginList => () => "";
        public Func<string> EndList => () => "";
    }

    public static class Statistics
    {
        public static object GetMedian(IEnumerable<double> data)
        {
            var list = data.OrderBy(z => z).ToList();
            if (list.Count % 2 == 0)
                return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;

            return list[list.Count / 2];
        }

        public static object GetMeanAndStd(IEnumerable<double> data)
        {
            var mean = data.Average();
            var std = Math.Sqrt(data.Select(z => Math.Pow(z - mean, 2)).Sum() / (data.Count() - 1));

            return new MeanAndStd
            {
                Mean = mean,
                Std = std
            };
        }
    }

    public class Report
    {
        public string Caption { get; }
        public string Temperature { get; }
        public string Humidity { get; }
        public Report(
            string caption,
            IEnumerable<Measurement> measurements,
            Func<IEnumerable<double>, object> statistics)
        {
            Caption = caption;
            Temperature = statistics(measurements.Select(z => z.Temperature)).ToString();
            Humidity = statistics(measurements.Select(z => z.Humidity)).ToString();
        }

        public string Format(IFormatter formatter)
        {
            var result = new StringBuilder()
                .Append(formatter.MakeCaption(Caption))
                .Append(formatter.BeginList())
                .Append(formatter.MakeItem("Temperature", Temperature))
                .Append(formatter.MakeItem("Humidity", Humidity))
                .Append(formatter.EndList());
            return result.ToString();
        }
    }

    public static class ReportMakerHelper
    {
        public static string MeanAndStdHtmlReport(IEnumerable<Measurement> data)
        {
            var report = new Report("Mean and Std", data, Statistics.GetMeanAndStd);
            return report.Format(new HTMLFormatter());
        }

        public static string MedianMarkdownReport(IEnumerable<Measurement> data)
        {
            var report = new Report("Median", data, Statistics.GetMedian);
            return report.Format(new MarkdownFormatter());
        }

        public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> measurements)
        {
            var report = new Report("Mean and Std", measurements, Statistics.GetMeanAndStd);
            return report.Format(new MarkdownFormatter());
        }

        public static string MedianHtmlReport(IEnumerable<Measurement> measurements)
        {
            var report = new Report("Median", measurements, Statistics.GetMedian);
            return report.Format(new HTMLFormatter());
        }
    }
}
