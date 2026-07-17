/// <summary>
/// Хелпер для работы со значениями версий.
/// </summary>
public static class VersionHelper
{
    /// <summary>
    /// Получить из отображаемой версии числовую.
    /// </summary>
    /// <param name="displayVersion">Отображаемая версия.</param>
    /// <returns>Версия в сиде числа.</returns>
    public static long GetVersionNumberFromDisplayVersion(string displayVersion)
    {
        return displayVersion
            .Split('.')
            .Aggregate<string?, long>(0, (current, part) => current * 1000 + long.Parse(part!));
    }
    
    /// <summary>
    /// Проверить является ли отображаемая версия справа больше левой.
    /// </summary>
    /// <param name="leftVersion">Отображаемая версия слева.</param>
    /// <param name="rightVersion">Отображаемая версия справа.</param>
    /// <returns>True, если правая больше.</returns>
    public static bool RightIsBiggerThanLeft(string leftVersion, string rightVersion)
    {
        var leftValue = GetVersionNumberFromDisplayVersion(leftVersion);
        var rightValue = GetVersionNumberFromDisplayVersion(rightVersion);

        return rightValue > leftValue;
    }
}