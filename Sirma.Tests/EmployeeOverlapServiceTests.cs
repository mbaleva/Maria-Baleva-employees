using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;
using Sirma.Services;

public class EmployeeOverlapServiceTests
{
    private EmployeeOverlapService _service = new EmployeeOverlapService();

    private IFormFile CreateMockFormFile(string content)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
        mockFile.Setup(f => f.Length).Returns(stream.Length);
        mockFile.Setup(f => f.FileName).Returns("test.csv");
        return mockFile.Object;
    }

    [Fact]
    public async Task ProcessCsvAsync_ShouldReturnCorrectOverlaps()
    {
        // Arrange
        var csv = "EmpID,ProjectID,DateFrom,DateTo\n" +
                  "1,100,2020-01-01,2020-01-10\n" +
                  "2,100,2020-01-05,2020-01-15\n" +
                  "3,200,2020-01-01,2020-01-10\n";

        var file = CreateMockFormFile(csv);

        // Act
        var result = await _service.ProcessCsvAsync(file);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var overlap = result[0];
        Assert.Equal(1, overlap.EmpID1);
        Assert.Equal(2, overlap.EmpID2);
        Assert.Equal(100, overlap.ProjectID);
        Assert.Equal(6, overlap.DaysWorked);
    }

    [Fact]
    public async Task ProcessCsvAsync_ShouldTreatNullDateToAsToday()
    {
        // Arrange
        var today = DateTime.Now.Date;
        var csv = $"EmpID,ProjectID,DateFrom,DateTo\n" +
                  $"1,100,2020-01-01,NULL\n" +
                  $"2,100,2020-12-01,NULL\n";

        var file = CreateMockFormFile(csv);

        // Act
        var result = await _service.ProcessCsvAsync(file);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var overlap = result[0];
        Assert.Equal(1, overlap.EmpID1);
        Assert.Equal(2, overlap.EmpID2);
        Assert.Equal(100, overlap.ProjectID);

        var expectedDays = (today - new DateTime(2020, 12, 1)).Days + 1;
        Assert.Equal(expectedDays, overlap.DaysWorked);
    }

    [Fact]
    public async Task ProcessCsvAsync_InvalidDateFormat_ShouldThrowFormatException()
    {
        // Arrange
        var csv = "EmpID,ProjectID,DateFrom,DateTo\n" +
                  "1,100,not-a-date,2020-01-10\n";

        var file = CreateMockFormFile(csv);

        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() => _service.ProcessCsvAsync(file));
    }

    [Fact]
    public void GetOverlapDays_ShouldReturnZeroIfNoOverlap()
    {
        // Arrange
        var service = new EmployeeOverlapService();

        var firstStart = new DateTime(2020, 1, 1);
        var firstEnd = new DateTime(2020, 1, 5);
        var secondStart = new DateTime(2020, 1, 6);
        var secondEnd = new DateTime(2020, 1, 10);

        var method = typeof(EmployeeOverlapService).GetMethod("GetOverlapDays", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var overlap = (int)method.Invoke(service, new object[] { firstStart, firstEnd, secondStart, secondEnd });

        Assert.Equal(0, overlap);
    }

    [Fact]
    public void GetOverlapDays_ShouldCalculateCorrectDays()
    {
        var service = new EmployeeOverlapService();

        var firstStart = new DateTime(2020, 1, 1);
        var firstEnd = new DateTime(2020, 1, 10);
        var secondStart = new DateTime(2020, 1, 5);
        var secondEnd = new DateTime(2020, 1, 15);

        var method = typeof(EmployeeOverlapService).GetMethod("GetOverlapDays", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var overlap = (int)method.Invoke(service, new object[] { firstStart, firstEnd, secondStart, secondEnd });

        Assert.Equal(6, overlap);
    }
}
