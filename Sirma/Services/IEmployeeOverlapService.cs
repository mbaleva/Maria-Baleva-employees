using Sirma.Models;

namespace Sirma.Services;

public interface IEmployeeOverlapService
{
    Task<List<EmployeePairResult>> ProcessCsvAsync(IFormFile file);
}
