namespace AeroBlazor.Components;

public class AeroSquareSelectionOption
{
    public AeroSquareSelectionOption(string label, string image, object item)
    {
        Label = label;
        ImageUrl = image;
        Item = item;
    }

    public string ImageUrl { get; set; }
    public string Label { get; set; }
    public object Item { get; set; }
}