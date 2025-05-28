using CsvHelper.Configuration;
using CsvHelper;
using Sirma.Models;
using System.Globalization;
using System.Text;

namespace Sirma.Services;

public class EmployeeOverlapService : IEmployeeOverlapService
{
    public async Task<List<EmployeePairResult>> ProcessCsvAsync(IFormFile file)
    {
        var records = new List<EmployeeProject>();

        using (var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            IgnoreBlankLines = true,
        }))
        {
            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                var empId = csv.GetField<int>("EmpID");
                var projectId = csv.GetField<int>("ProjectID");
                var dateFromStr = csv.GetField<string>("DateFrom");
                var dateToStr = csv.GetField<string>("DateTo");

                DateTime dateFrom = ParseDate(dateFromStr);
                DateTime dateTo = string.IsNullOrWhiteSpace(dateToStr) || dateToStr.ToUpper() == "NULL"
                    ? DateTime.Now
                    : ParseDate(dateToStr);

                records.Add(new EmployeeProject
                {
                    EmpID = empId,
                    ProjectID = projectId,
                    DateFrom = dateFrom,
                    DateTo = dateTo
                });
            }
        }

        return CalculateOverlaps(records);
    }

    private DateTime ParseDate(string input)
    {
        if (DateTime.TryParse(input, out var parsed))
        {
            return parsed;
        }

        if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
        {
            return parsed;
        }

        throw new FormatException($"Invalid date format: {input}");
    }

    private List<EmployeePairResult> CalculateOverlaps(List<EmployeeProject> records)
    {
        var overlaps = new List<EmployeePairResult>();

        var projects = records.GroupBy(r => r.ProjectID);

        foreach (var projectGroup in projects)
        {
            var employees = projectGroup.OrderBy(e => e.DateFrom).ToList();
            var active = new List<EmployeeProject>();

            foreach (var current in employees)
            {
                active.RemoveAll(a => a.DateTo < current.DateFrom);

                foreach (var item in active)
                {
                    var overlapDays = GetOverlapDays(item.DateFrom, item.DateTo.Value, current.DateFrom, current.DateTo.Value);
                    if (overlapDays > 0)
                    {
                        overlaps.Add(new EmployeePairResult
                        {
                            EmpID1 = item.EmpID,
                            EmpID2 = current.EmpID,
                            ProjectID = current.ProjectID,
                            DaysWorked = overlapDays
                        });
                    }
                }

                active.Add(current);
            }
        }

        return overlaps;
    }

    private int GetOverlapDays(DateTime firstStart, DateTime firstEnd, DateTime secondStart, DateTime secondEnd)
    {
        var start = firstStart > secondStart ? firstStart : secondStart;
        var end = firstEnd < secondEnd ? firstEnd : secondEnd;

        return (end - start).TotalDays >= 0 ? (end - start).Days + 1 : 0;
    }
}
