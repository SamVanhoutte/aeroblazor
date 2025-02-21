using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;

namespace AeroBlazor.Extensions;

public static class RenderFragmentExtensions
{
    public static string? GetText(this RenderFragment? renderFragment)
    {
        if (renderFragment == null)
        {
            return null;
        }
        var builder = new RenderTreeBuilder();
        renderFragment.Invoke(builder);
        return builder.GetFrames().Array
            .OfType<RenderTreeFrame>()
            .FirstOrDefault(f => f.FrameType == RenderTreeFrameType.Text).TextContent;

    }
}