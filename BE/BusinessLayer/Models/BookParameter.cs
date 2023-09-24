namespace BusinessLayer.Models;

public class BookParameters : QueryParameters
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string DescriptionKeyword { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
}