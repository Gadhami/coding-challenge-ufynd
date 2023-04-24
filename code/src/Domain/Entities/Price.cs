namespace Hotels.Domain.Entities;

public class Price
{
    public string Currency       { get; set; } = string.Empty;
    public float  NumericFloat   { get; set; }
    public int    NumericInteger { get; set; }
}