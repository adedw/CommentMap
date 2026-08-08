using System.Text.Json;

using CommentMap.Mvc.ViewModels;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CommentMap.Mvc.Extensions;

public static class TempDataExtensions
{
    private const string StatusMessageKey = "StatusMessage";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static void SetStatus(this ITempDataDictionary tempData, StatusMessage message)
        => tempData[StatusMessageKey] = JsonSerializer.Serialize(message, JsonOptions);

    public static StatusMessage? GetStatus(this ITempDataDictionary tempData)
    {
        if (!tempData.TryGetValue(StatusMessageKey, out var value) || value is not string json)
            return null;

        tempData.Remove(StatusMessageKey);
        return JsonSerializer.Deserialize<StatusMessage>(json, JsonOptions);
    }
}