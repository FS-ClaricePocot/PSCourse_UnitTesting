using WiredBrainCoffee.DataProcessor.Data;
using WiredBrainCoffee.DataProcessor.Model;

namespace WiredBrainCoffee.DataProcessor.Processing;

public class MachineDataProcessorTests:IDisposable
{
    private FakeCoffeeCountStore _coffeeCountStore;
    private MachineDataProcessor _machineDataProcessor;

    public MachineDataProcessorTests()
    {
        _coffeeCountStore = new FakeCoffeeCountStore();
        _machineDataProcessor = new MachineDataProcessor(_coffeeCountStore);
    }


    [Fact]
    public void ShouldSaveCountPerCoffeeType()
    {
        // Arrange

        var items = new[]
        {
            new MachineDataItem("Cappuccino", new DateTime(2025, 8, 23, 5, 0, 0, 0)),
            new MachineDataItem("Cappuccino", new DateTime(2025, 8, 23, 6, 0, 0, 0)),
            new MachineDataItem("Espresso", new DateTime(2025, 8, 23, 7, 0, 0, 0))
        };

        // Act
        _machineDataProcessor.ProcessItems(items);

        // Assert
        Assert.Equal(2, _coffeeCountStore.SavedItems.Count);
        var item_Cappucino = _coffeeCountStore.SavedItems[0];
        Assert.Equal("Cappuccino", item_Cappucino.CoffeeType);
        Assert.Equal(2, item_Cappucino.Count);

        var item_Espresso = _coffeeCountStore.SavedItems[1];
        Assert.Equal("Espresso", item_Espresso.CoffeeType);
        Assert.Equal(1, item_Espresso.Count);

    }

    [Fact]
    public void ShouldIgnoreItemsThatAreNotNewer()
    {
        // Arrange

        var items = new[]
        {
            new MachineDataItem("Cappuccino", new DateTime(2025, 8, 23, 5, 0, 0, 0)),
            new MachineDataItem("Cappuccino", new DateTime(2025, 8, 23, 4, 0, 0, 0)),// should be ignored
            new MachineDataItem("Cappuccino", new DateTime(2025, 8, 23, 4, 10, 0, 0)),// should be ignored
            new MachineDataItem("Cappuccino", new DateTime(2025, 8, 23, 6, 0, 0, 0)),
            new MachineDataItem("Espresso", new DateTime(2025, 8, 23, 7, 0, 0, 0)),
            new MachineDataItem("Espresso", new DateTime(2025, 8, 23, 7, 0, 0, 0)), // should be ignored
        };

        // Act
        _machineDataProcessor.ProcessItems(items);

        // Assert
        Assert.Equal(2, _coffeeCountStore.SavedItems.Count);
        var item_Cappucino = _coffeeCountStore.SavedItems[0];
        Assert.Equal("Cappuccino", item_Cappucino.CoffeeType);
        Assert.Equal(2, item_Cappucino.Count);

        var item_Espresso = _coffeeCountStore.SavedItems[1];
        Assert.Equal("Espresso", item_Espresso.CoffeeType);
        Assert.Equal(1, item_Espresso.Count);

    }

    [Fact]
    public void ShouldClearPreviousCoffeeCount()
    {
        // Arrange

        var items = new[]
        {
            new MachineDataItem("Cappuccino", new DateTime(2025, 8, 23, 5, 0, 0, 0))
        };

        // Act
        _machineDataProcessor.ProcessItems(items);
        _machineDataProcessor.ProcessItems(items);

        // Assert
        Assert.Equal(2, _coffeeCountStore.SavedItems.Count);
        foreach (var item in _coffeeCountStore.SavedItems)
        {
            Assert.Equal("Cappuccino", item.CoffeeType);
            Assert.Equal(1, item.Count);
        }

    }

    public void Dispose()
    {
        // This runs after every test

    }
}

public class FakeCoffeeCountStore : ICoffeeCountStore
{
    public List<CoffeeCountItem> SavedItems { get; } = new();
    public void Save(CoffeeCountItem item)
    {
        SavedItems.Add(item);
    }
}
