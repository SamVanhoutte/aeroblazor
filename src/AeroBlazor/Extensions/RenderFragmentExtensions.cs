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
        var frames = builder.GetFrames().Array
            .OfType<RenderTreeFrame>();
        var parsedContent = frames.FirstOrDefault(f => f.FrameType == RenderTreeFrameType.Text || f.FrameType== RenderTreeFrameType.Markup);
        var result = parsedContent.TextContent;
        return result;
    }
}