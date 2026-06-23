using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace onsoftware.task4.Helpers;

public static class TempDataExtensions
{
    public static void Success(
        this ITempDataDictionary tempData,
        string message)
    {
        tempData["Success"] = message;
    }

    public static void Error(
        this ITempDataDictionary tempData,
        string message)
    {
        tempData["Error"] = message;
    }

    public static void Warning(
        this ITempDataDictionary tempData,
        string message)
    {
        tempData["Warning"] = message;
    }

    public static void Info(
        this ITempDataDictionary tempData,
        string message)
    {
        tempData["Info"] = message;
    }
}