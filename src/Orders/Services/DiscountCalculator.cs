namespace Orders.Services;

public class DiscountCalculator
{
    public decimal Calculate(decimal total)
    {
        if (total < 0)
            throw new ArgumentOutOfRangeException(nameof(total), "Total cannot be negative");

        return total >= 1000m ? total * 0.1m : 0m;
    }
}