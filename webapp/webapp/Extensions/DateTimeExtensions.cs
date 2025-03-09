namespace webapp.Extensions;

public static class DateTimeExtensions {
    /// <summary>
    /// Returns the date corresponding to the start of the week for the given reference date.
    /// </summary>
    /// <param name="referenceDate">The reference date to calculate the start of the week.</param>
    /// <param name="startOfWeek">The day of the week considered as the first day of the week.</param>
    /// <returns>The date corresponding to the first day of the specified week.</returns>
    public static DateTime StartOfWeek(this DateTime referenceDate, DayOfWeek startOfWeek) {
        int diff = (7 + (referenceDate.DayOfWeek - startOfWeek)) % 7;
        return referenceDate.AddDays(-diff).Date;
    }
}