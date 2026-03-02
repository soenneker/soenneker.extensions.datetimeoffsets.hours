using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;
using Soenneker.Enums.UnitOfTime;

namespace Soenneker.Extensions.DateTimeOffsets.Hours;

/// <summary>
/// Extension methods for <see cref="DateTimeOffset"/> focused on hour boundaries and hour-of-day conversions,
/// including helpers that map between UTC hour-of-day and a target time zone's hour-of-day using explicit anchoring rules.
/// </summary>
public static class DateTimeOffsetsHourExtension
{
    /// <summary>
    /// Returns the start of the hour containing <paramref name="dateTimeOffset"/> (minute/second/fraction set to zero).
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>
    /// The first moment of the hour containing <paramref name="dateTimeOffset"/>. The original offset is preserved.
    /// </returns>
    /// <remarks>
    /// This delegates to your library's <c>ToStartOf(UnitOfTime.Hour)</c> implementation. No time zone conversion is performed.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToStartOfHour(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOf(UnitOfTime.Hour);

    /// <summary>
    /// Returns the start of the next hour relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The reference instant.</param>
    /// <returns>
    /// The first moment of the hour immediately following the hour containing <paramref name="dateTimeOffset"/>,
    /// preserving the original offset.
    /// </returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToStartOfNextHour(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOfHour().AddHours(1);

    /// <summary>
    /// Returns the start of the previous hour relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The reference instant.</param>
    /// <returns>
    /// The first moment of the hour immediately preceding the hour containing <paramref name="dateTimeOffset"/>,
    /// preserving the original offset.
    /// </returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToStartOfPreviousHour(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOfHour().AddHours(-1);

    /// <summary>
    /// Returns the end of the hour containing <paramref name="dateTimeOffset"/> (one tick before the next hour).
    /// </summary>
    /// <param name="dateTimeOffset">The value to adjust.</param>
    /// <returns>
    /// The last tick of the hour containing <paramref name="dateTimeOffset"/>. The original offset is preserved.
    /// </returns>
    /// <remarks>
    /// This delegates to your library's <c>ToEndOf(UnitOfTime.Hour)</c> implementation. No time zone conversion is performed.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToEndOfHour(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToEndOf(UnitOfTime.Hour);

    /// <summary>
    /// Returns the end of the next hour relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The reference instant.</param>
    /// <returns>
    /// The last tick of the hour immediately following the hour containing <paramref name="dateTimeOffset"/>,
    /// preserving the original offset.
    /// </returns>
    /// <remarks>
    /// This is computed as: <c>start-of-next-hour + 1 hour - 1 tick</c>, which avoids relying on any
    /// <c>ToEndOf</c> implementation details and is stable across DST transitions because it operates on instants.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToEndOfNextHour(this DateTimeOffset dateTimeOffset)
    {
        DateTimeOffset startNext = dateTimeOffset.ToStartOfHour().AddHours(1);
        return startNext.AddHours(1).AddTicks(-1);
    }

    /// <summary>
    /// Returns the end of the previous hour relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The reference instant.</param>
    /// <returns>
    /// The last tick of the hour immediately preceding the hour containing <paramref name="dateTimeOffset"/>,
    /// preserving the original offset.
    /// </returns>
    /// <remarks>
    /// This is computed as: <c>start-of-this-hour - 1 tick</c>.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToEndOfPreviousHour(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToStartOfHour().AddTicks(-1);

    /// <summary>
    /// Converts <paramref name="instant"/> to <paramref name="tz"/> and formats the local wall-clock time as <c>"h:mm tt"</c>
    /// using the current culture.
    /// </summary>
    /// <param name="instant">
    /// An instant in time. Any offset is allowed; the value is converted to the target time zone as an instant.
    /// </param>
    /// <param name="tz">The target time zone.</param>
    /// <returns>A culture-formatted local time string such as <c>"3:05 PM"</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="tz"/> is <see langword="null"/>.</exception>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToTzHourFormat(this DateTimeOffset instant, TimeZoneInfo tz)
    {
        if (tz is null)
            throw new ArgumentNullException(nameof(tz));

        DateTimeOffset local = TimeZoneInfo.ConvertTime(instant, tz);
        return local.ToHourFormat();
    }

    /// <summary>
    /// Converts <paramref name="instant"/> to <paramref name="tz"/>, trims to the start of the local hour,
    /// and formats the result as <c>"h:mm tt"</c> using the current culture.
    /// </summary>
    /// <param name="instant">
    /// An instant in time. Any offset is allowed; the value is converted to the target time zone as an instant.
    /// </param>
    /// <param name="tz">The target time zone.</param>
    /// <returns>A culture-formatted local time string aligned to the hour, such as <c>"3:00 PM"</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="tz"/> is <see langword="null"/>.</exception>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToTzHourFormatWithTrim(this DateTimeOffset instant, TimeZoneInfo tz)
    {
        if (tz is null)
            throw new ArgumentNullException(nameof(tz));

        DateTimeOffset local = TimeZoneInfo.ConvertTime(instant, tz).ToStartOfHour();
        return local.ToHourFormat();
    }

    /// <summary>
    /// Formats <paramref name="dateTimeOffset"/> as <c>"h:mm tt"</c> using the current culture.
    /// </summary>
    /// <param name="dateTimeOffset">The value to format.</param>
    /// <returns>A culture-formatted time string such as <c>"3:05 PM"</c>.</returns>
    /// <remarks>
    /// This uses <see cref="CultureInfo.CurrentCulture"/>. If you need an invariant format, call
    /// <c>dateTimeOffset.ToString("h:mm tt", CultureInfo.InvariantCulture)</c>.
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToHourFormat(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToString("h:mm tt");

    /// <summary>
    /// Converts a UTC hour-of-day to the corresponding local hour-of-day in <paramref name="tz"/>,
    /// anchored to the UTC date of <paramref name="utcInstant"/>.
    /// </summary>
    /// <param name="utcInstant">
    /// An instant used only for its UTC date (year/month/day). Any offset is allowed; the value is normalized to UTC for the date.
    /// </param>
    /// <param name="utcHour">A UTC hour-of-day in the inclusive range [0..23].</param>
    /// <param name="tz">The target time zone.</param>
    /// <returns>
    /// The local hour-of-day [0..23] for the instant represented by (UTC date from <paramref name="utcInstant"/>, <paramref name="utcHour"/>).
    /// </returns>
    /// <remarks>
    /// This method intentionally anchors on the UTC date, not the target time zone's local date. Near midnight, the resulting local
    /// time may fall on the previous or next local day.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="utcHour"/> is outside [0..23].</exception>
    /// <exception cref="ArgumentNullException"><paramref name="tz"/> is <see langword="null"/>.</exception>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    /// Formats a UTC hour-of-day as a local time string in <paramref name="tz"/>,
    /// anchored to the UTC date of <paramref name="utcInstant"/>.
    /// </summary>
    /// <param name="utcInstant">
    /// An instant used only for its UTC date (year/month/day). Any offset is allowed; the value is normalized to UTC for the date.
    /// </param>
    /// <param name="utcHour">A UTC hour-of-day in the inclusive range [0..23].</param>
    /// <param name="tz">The target time zone.</param>
    /// <returns>A culture-formatted local time string such as <c>"3:00 PM"</c>.</returns>
    /// <remarks>
    /// This computes the target local wall-clock time by converting the UTC instant (UTC date + <paramref name="utcHour"/>)
    /// into <paramref name="tz"/>. No manual offset reconstruction is performed, avoiding DST/offset-kind pitfalls.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="utcHour"/> is outside [0..23].</exception>
    /// <exception cref="ArgumentNullException"><paramref name="tz"/> is <see langword="null"/>.</exception>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToTzHourFormatFromUtc(this DateTimeOffset utcInstant, int utcHour, TimeZoneInfo tz)
    {
        if ((uint)utcHour > 23)
            throw new ArgumentOutOfRangeException(nameof(utcHour), "Hour must be in range [0, 23].");
        if (tz is null)
            throw new ArgumentNullException(nameof(tz));

        DateTimeOffset utc = utcInstant.ToUniversalTime();
        DateTime utcDateTime = new(utc.Year, utc.Month, utc.Day, utcHour, 0, 0, DateTimeKind.Utc);

        DateTime local = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tz);

        // We only need "h:mm tt"; formatting the local wall-clock DateTime avoids any offset reconstruction bugs.
        return local.ToString("h:mm tt");
    }

    /// <summary>
    /// Converts a local hour-of-day in <paramref name="tz"/> to the corresponding UTC hour-of-day,
    /// anchored to the local DATE in <paramref name="tz"/> that contains <paramref name="utcInstant"/>.
    /// </summary>
    /// <param name="utcInstant">
    /// An instant used to choose the anchor local date in <paramref name="tz"/>. Any offset is allowed; the value is normalized to UTC first.
    /// </param>
    /// <param name="tzHour">A local hour-of-day in the inclusive range [0..23].</param>
    /// <param name="tz">The time zone that defines the local hour.</param>
    /// <returns>
    /// The UTC hour-of-day [0..23] corresponding to the requested local wall time on the selected local date, using zone rules.
    /// </returns>
    /// <remarks>
    /// DST policy:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// If the local wall time is <em>invalid</em> (spring-forward gap), the time is advanced minute-by-minute to the first valid minute at or after
    /// the requested hour.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// If the local wall time is <em>ambiguous</em> (fall-back fold), the earlier UTC instant is chosen (the larger offset).
    /// </description>
    /// </item>
    /// </list>
    /// Note that this method returns only the hour component of the resulting UTC time. If you need the full instant,
    /// return a <see cref="DateTimeOffset"/> instead.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="tzHour"/> is outside [0..23].</exception>
    /// <exception cref="ArgumentNullException"><paramref name="tz"/> is <see langword="null"/>.</exception>
    [Pure]
    public static int ToUtcHoursFromTz(this DateTimeOffset utcInstant, int tzHour, TimeZoneInfo tz)
    {
        if ((uint)tzHour > 23)
            throw new ArgumentOutOfRangeException(nameof(tzHour), "Hour must be in range [0, 23].");
        if (tz is null)
            throw new ArgumentNullException(nameof(tz));

        // Determine the local date that corresponds to this instant.
        DateTimeOffset utc = utcInstant.ToUniversalTime();
        DateTimeOffset localNow = TimeZoneInfo.ConvertTime(utc, tz);

        // Build the requested local wall-clock hour on that local date (Unspecified = "wall time in tz").
        DateTime localWall = new(localNow.Year, localNow.Month, localNow.Day, tzHour, 0, 0, DateTimeKind.Unspecified);

        // Handle spring-forward gaps (invalid local times) by advancing to the first valid minute.
        // This is typically at most 60 iterations; still, it's only used on the rare DST-transition dates.
        if (tz.IsInvalidTime(localWall))
        {
            // Fast path: jump by hours first (common gap is 1 hour), then fall back to minutes if still invalid.
            // Keeps correctness for zones with non-1h gaps.
            localWall = localWall.AddHours(1);

            while (tz.IsInvalidTime(localWall))
                localWall = localWall.AddMinutes(1);
        }

        // Handle fall-back folds (ambiguous local times) by choosing the earlier UTC instant (larger offset).
        if (tz.IsAmbiguousTime(localWall))
        {
            TimeSpan[] offsets = tz.GetAmbiguousTimeOffsets(localWall);

            // Larger offset => earlier UTC because UTC = local - offset.
            TimeSpan chosen = offsets[0] >= offsets[1] ? offsets[0] : offsets[1];

            // localWall is "unspecified wall time"; subtract chosen offset to get the corresponding UTC instant.
            DateTime utcAmb = DateTime.SpecifyKind(localWall - chosen, DateTimeKind.Utc);
            return utcAmb.Hour;
        }

        DateTime utcWall = TimeZoneInfo.ConvertTimeToUtc(localWall, tz);
        return utcWall.Hour;
    }
}