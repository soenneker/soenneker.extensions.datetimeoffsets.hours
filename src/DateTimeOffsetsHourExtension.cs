using System;
using System.Diagnostics.Contracts;
using Soenneker.Enums.UnitOfTime;

namespace Soenneker.Extensions.DateTimeOffsets.Hours;

/// <summary>
/// Extension methods for <see cref="DateTimeOffset"/> focused on hour boundaries and hour-of-day conversions.
/// </summary>
public static class DateTimeOffsetsHourExtension
{
    /// <summary>
    /// Returns the start of the hour containing <paramref name="dateTimeOffset"/> (minute/second/fraction set to zero).
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>The first moment of the hour containing <paramref name="dateTimeOffset"/>. The original offset is preserved.</returns>
    [Pure]
    public static DateTimeOffset ToStartOfHour(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOf(UnitOfTime.Hour);

    /// <summary>
    /// Returns the start of the next hour relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToStartOfNextHour(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOfHour()
                      .AddHours(1);

    /// <summary>
    /// Returns the start of the previous hour relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToStartOfPreviousHour(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOfHour()
                      .AddHours(-1);

    /// <summary>
    /// Returns the end of the hour containing <paramref name="dateTimeOffset"/> (one tick before the next hour).
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>The last tick of the hour containing <paramref name="dateTimeOffset"/>. The original offset is preserved.</returns>
    [Pure]
    public static DateTimeOffset ToEndOfHour(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToEndOf(UnitOfTime.Hour);

    /// <summary>
    /// Returns the end of the next hour relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToEndOfNextHour(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToEndOfHour()
                      .AddHours(1);

    /// <summary>
    /// Returns the end of the previous hour relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToEndOfPreviousHour(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToEndOfHour()
                      .AddHours(-1);

    /// <summary>
    /// Converts the UTC instant <paramref name="utcInstant"/> to <paramref name="tz"/> and formats the local time as "h:mm tt".
    /// </summary>
    /// <param name="utcInstant">An instant in time (any offset is normalized to UTC).</param>
    /// <param name="tz">The target time zone.</param>
    [Pure]
    public static string ToTzHourFormat(this DateTimeOffset utcInstant, TimeZoneInfo tz)
    {
        DateTimeOffset local = TimeZoneInfo.ConvertTime(utcInstant.ToUniversalTime(), tz);
        return local.ToHourFormat();
    }

    /// <summary>
    /// Converts the UTC instant <paramref name="utcInstant"/> to <paramref name="tz"/>, trims to the start of the local hour,
    /// and formats the local time as "h:mm tt".
    /// </summary>
    /// <param name="utcInstant">An instant in time (any offset is normalized to UTC).</param>
    /// <param name="tz">The target time zone.</param>
    [Pure]
    public static string ToTzHourFormatWithTrim(this DateTimeOffset utcInstant, TimeZoneInfo tz)
    {
        DateTimeOffset local = TimeZoneInfo.ConvertTime(utcInstant.ToUniversalTime(), tz)
                                           .ToStartOfHour();
        return local.ToHourFormat();
    }

    /// <summary>
    /// Formats <paramref name="dateTimeOffset"/> as "h:mm tt" using the current culture.
    /// </summary>
    [Pure]
    public static string ToHourFormat(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToString("h:mm tt");

    /// <summary>
    /// Converts a UTC hour-of-day to the corresponding local hour-of-day in <paramref name="tz"/>,
    /// anchored to the UTC date of <paramref name="utcInstant"/>.
    /// </summary>
    /// <param name="utcInstant">An instant used only for its UTC date (any offset is normalized to UTC).</param>
    /// <param name="utcHour">UTC hour-of-day [0..23].</param>
    /// <param name="tz">The target time zone.</param>
    /// <returns>The local hour-of-day [0..23] for that UTC date/hour mapped into <paramref name="tz"/>.</returns>
    [Pure]
    public static int ToTzHoursFromUtc(this DateTimeOffset utcInstant, int utcHour, TimeZoneInfo tz)
    {
        if ((uint)utcHour > 23)
            throw new ArgumentOutOfRangeException(nameof(utcHour), "Hour must be in range [0, 23].");
        if (tz is null)
            throw new ArgumentNullException(nameof(tz));

        DateTimeOffset utc = utcInstant.ToUniversalTime();
        DateTime utcDateTime = new(utc.Year, utc.Month, utc.Day, utcHour, 0, 0, DateTimeKind.Utc);

        DateTime local = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tz);
        return local.Hour;
    }

    /// <summary>
    /// Formats a UTC hour-of-day as a local time string in <paramref name="tz"/>, anchored to the UTC date of <paramref name="utcInstant"/>.
    /// </summary>
    /// <param name="utcInstant">An instant used only for its UTC date (any offset is normalized to UTC).</param>
    /// <param name="utcHour">UTC hour-of-day [0..23].</param>
    /// <param name="tz">The target time zone.</param>
    /// <returns>A string formatted as "h:mm tt" in the target time zone.</returns>
    [Pure]
    public static string ToTzHourFormatFromUtc(this DateTimeOffset utcInstant, int utcHour, TimeZoneInfo tz)
    {
        if ((uint)utcHour > 23)
            throw new ArgumentOutOfRangeException(nameof(utcHour), "Hour must be in range [0, 23].");
        if (tz is null)
            throw new ArgumentNullException(nameof(tz));

        DateTimeOffset utc = utcInstant.ToUniversalTime();
        DateTime utcDateTime = new(utc.Year, utc.Month, utc.Day, utcHour, 0, 0, DateTimeKind.Utc);

        DateTime local = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tz);
        // Represent as DateTimeOffset using the correct offset for that local instant
        TimeSpan offset = tz.GetUtcOffset(utcDateTime);
        DateTimeOffset localDto = new(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), offset);

        return localDto.ToHourFormat();
    }

    /// <summary>
    /// Converts a local hour-of-day in <paramref name="tz"/> to the corresponding UTC hour-of-day,
    /// anchored to the local DATE in <paramref name="tz"/> that contains <paramref name="utcInstant"/>.
    /// </summary>
    /// <param name="utcInstant">An instant used to select the local date in <paramref name="tz"/> (any offset is normalized to UTC).</param>
    /// <param name="tzHour">Local hour-of-day [0..23].</param>
    /// <param name="tz">The time zone that defines the local hour.</param>
    /// <returns>The UTC hour-of-day [0..23] for that local wall time, using zone rules (DST-safe).</returns>
    [Pure]
    public static int ToUtcHoursFromTz(this DateTimeOffset utcInstant, int tzHour, TimeZoneInfo tz)
    {
        if ((uint)tzHour > 23)
            throw new ArgumentOutOfRangeException(nameof(tzHour), "Hour must be in range [0, 23].");
        if (tz is null)
            throw new ArgumentNullException(nameof(tz));

        // Determine the local date that corresponds to this instant
        DateTimeOffset utc = utcInstant.ToUniversalTime();
        DateTimeOffset localNow = TimeZoneInfo.ConvertTime(utc, tz);

        // Build the requested local wall-clock hour on that local date
        DateTime localWall = new(localNow.Year, localNow.Month, localNow.Day, tzHour, 0, 0, DateTimeKind.Unspecified);

        // Handle spring-forward gaps (invalid local times) by advancing to the first valid minute
        if (tz.IsInvalidTime(localWall))
        {
            do
            {
                localWall = localWall.AddMinutes(1);
            }
            while (tz.IsInvalidTime(localWall));
        }

        // Handle fall-back folds (ambiguous local times) by choosing the earlier UTC instant
        if (tz.IsAmbiguousTime(localWall))
        {
            TimeSpan[] offsets = tz.GetAmbiguousTimeOffsets(localWall);
            TimeSpan chosen = offsets[0] >= offsets[1] ? offsets[0] : offsets[1]; // larger offset => earlier UTC (UTC = local - offset)
            DateTime utcAmb = DateTime.SpecifyKind(localWall - chosen, DateTimeKind.Utc);
            return utcAmb.Hour;
        }

        DateTime utcWall = TimeZoneInfo.ConvertTimeToUtc(localWall, tz);
        return utcWall.Hour;
    }
}