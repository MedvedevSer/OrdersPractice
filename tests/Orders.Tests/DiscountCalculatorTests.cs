using Orders.Services;
using Xunit;

namespace Orders.Tests;

public class DiscountCalculatorTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(999.99, 0)]
    [InlineData(1000, 100)]
    [InlineData(2000, 200)]
    public void Calculate_ReturnsExpected(decimal total, decimal expected)
    {
        var calc = new DiscountCalculator();
        Assert.Equal(expected, calc.Calculate(total));
    }

    [Fact]
    public void Calculate_NegativeTotal_Throws()
    {
        var calc = new DiscountCalculator();
        Assert.Throws<ArgumentOutOfRangeException>(() => calc.Calculate(-1));
    }
}