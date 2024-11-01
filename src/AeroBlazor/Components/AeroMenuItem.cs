namespace AeroBlazor.Components;

public class AeroMenuItem
{
    public AeroMenuItem(string label, string href, string? icon = null)
    {
        Href = href;
        Icon = icon;
        Label = label;
    }

    public string Href { get; set; }
    public string Label { get; set; }
    public string? Icon { get; set; }
    public bool Tertiary { get; set; } = false;
}