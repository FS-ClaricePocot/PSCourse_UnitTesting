

using WiredBrainCoffee.DataProcessor.Model;

namespace WiredBrainCoffee.DataProcessor.Parsing;

public class CsvLineParserTests
{

    [Fact]
    public void ShouldParseValidLine()
    {
        // Arrange
        string[] csvlines = new[] { "Espresso;10/27/2022 8:01:16 AM" };

        // Act
        MachineDataItem[] parsedItem = CsvLineParser.Parse(csvlines);

        //Assert
        Assert.NotNull(parsedItem);
        Assert.Single(parsedItem);
        Assert.Equal("Espresso", parsedItem[0].CoffeeType);
        Assert.Equal(new DateTime(2022, 10, 27, 8, 1, 16), parsedItem[0].CreatedAt);
    }


    [Fact]
    public void ShouldSkipEmptyLines()
    {
        // Arrange
        string[] csvlines = new[] { "", " " };

        // Act
        MachineDataItem[] parsedItem = CsvLineParser.Parse(csvlines);

        //Assert
        Assert.NotNull(parsedItem);
        Assert.Empty(parsedItem);
    }

    [InlineData("Espresso", "Invalid csv line")]
    [InlineData("Espresso;InvalidDateTime", "Invalid DateTime in csv line")]
    [Theory]
    public void ShouldThrowExceptionForInvalidLine(string csvLine, string expectedPrefixMessage)
    {
        // Arrange
        string[] csvLines = new[] { csvLine };

        // Act and Assert
        var exception = Assert.Throws<Exception>(() => CsvLineParser.Parse(csvLines));

        Assert.Equal($"{expectedPrefixMessage}: {csvLine}", exception.Message);
    }

}
