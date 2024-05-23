using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Primitives;
using System.Text.Encodings.Web;

namespace CommentMap.Mvc.TagHelpers;

[HtmlTargetElement(tag: "a", Attributes = ActivePagesAttributeName)]
public class AnchorTagHelper : TagHelper
{
    private const string ActivePagesAttributeName = "asp-active-pages";
    private const string ActiveClassAttributeName = "asp-active-class";
    private static readonly char[] ActivePagesSeparator = [','];

    [HtmlAttributeName(ActivePagesAttributeName)]
    public required string ActivePages { get; set; }

    [HtmlAttributeName(ActiveClassAttributeName)]
    public string Class { get; set; } = "active";

    [HtmlAttributeNotBound]
    [ViewContext]
    public required ViewContext ViewContext { get; init; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!ViewContext.RouteData.Values.TryGetValue("page", out var currentRouteUntyped) || currentRouteUntyped is not string currentRoute)
        {
            return;
        }

        var tokenizer = new StringTokenizer(ActivePages, ActivePagesSeparator);
        var currentPage = currentRoute.Split('/', StringSplitOptions.RemoveEmptyEntries)[^1];
        if (tokenizer.Any(stringSegment => stringSegment.Equals(currentPage, StringComparison.OrdinalIgnoreCase)))
        {
            output.AddClass(Class, HtmlEncoder.Default);
        }
    }
}
