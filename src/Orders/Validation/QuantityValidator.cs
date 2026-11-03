namespace Orders.Validation;

public static class QuantityValidator
{
    public static bool IsValidQty(int quantity) => quantity is >= 1 and <= 100;
}