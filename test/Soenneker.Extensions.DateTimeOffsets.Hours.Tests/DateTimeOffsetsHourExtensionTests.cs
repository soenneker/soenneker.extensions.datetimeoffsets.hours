using System;
using System.Globalization;
using Soenneker.Extensions.DateTimeOffsets.Hours;
using Soenneker.Tests.Unit;

namespace Soenneker.Extensions.DateTimeOffsets.Hours.Tests;

public sealed class DateTimeOffsetsHourExtensionTests : UnitTest
{
    private static readonly TimeZoneInfo Eastern = TimeZoneInfo.FindSystemTimeZoneById(
        OperatingSystem.IsWindows() ? "Eastern Standard Time" : "America/New_York");

    #region ToStartOfHour

    [Test]
    public void ToStartOfHour_mid_hour_returns_start_of_same_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 45, 123, TimeSpan.FromHours(-4));
        var start = dto.ToStartOfHour();
        Assert.Equal(new DateTimeOffset(2024, 6, 15, 14, 0, 0, 0, dto.Offset), start);
    }

    [Test]
    public void ToStartOfHour_already_at_start_preserves_value()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 0, 0, 0, TimeSpan.Zero);
        var start = dto.ToStartOfHour();
        Assert.Equal(dto, start);
    }

    [Test]
    public void ToStartOfHour_last_tick_of_hour_returns_start_of_that_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 59, 59, 999, TimeSpan.Zero).AddTicks(9999);
        var start = dto.ToStartOfHour();
        Assert.Equal(new DateTimeOffset(2024, 6, 15, 14, 0, 0, 0, TimeSpan.Zero), start);
    }

    #endregion

    #region ToStartOfNextHour / ToStartOfPreviousHour

    [Test]
    public void ToStartOfNextHour_returns_first_moment_of_following_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, TimeSpan.Zero);
        var next = dto.ToStartOfNextHour();
        Assert.Equal(new DateTimeOffset(2024, 6, 15, 15, 0, 0, 0, TimeSpan.Zero), next);
    }

    [Test]
    public void ToStartOfNextHour_at_midnight_returns_next_day_one_am()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 0, 0, 0, 0, TimeSpan.Zero);
        var next = dto.ToStartOfNextHour();
        Assert.Equal(new DateTimeOffset(2024, 6, 15, 1, 0, 0, 0, TimeSpan.Zero), next);
    }

    [Test]
    public void ToStartOfNextHour_last_tick_of_day_returns_next_day()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 23, 59, 59, 999, TimeSpan.Zero).AddTicks(9999);
        var next = dto.ToStartOfNextHour();
        Assert.Equal(new DateTimeOffset(2024, 6, 16, 0, 0, 0, 0, TimeSpan.Zero), next);
    }

    [Test]
    public void ToStartOfPreviousHour_returns_first_moment_of_previous_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, TimeSpan.Zero);
        var prev = dto.ToStartOfPreviousHour();
        Assert.Equal(new DateTimeOffset(2024, 6, 15, 13, 0, 0, 0, TimeSpan.Zero), prev);
    }

    [Test]
    public void ToStartOfPreviousHour_at_midnight_returns_previous_day_23_00()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 0, 0, 0, 0, TimeSpan.Zero);
        var prev = dto.ToStartOfPreviousHour();
        Assert.Equal(new DateTimeOffset(2024, 6, 14, 23, 0, 0, 0, TimeSpan.Zero), prev);
    }

    #endregion

    #region ToEndOfHour

    [Test]
    public void ToEndOfHour_returns_last_tick_of_same_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, TimeSpan.Zero);
        var end = dto.ToEndOfHour();
        var expected = new DateTimeOffset(2024, 6, 15, 15, 0, 0, 0, TimeSpan.Zero).AddTicks(-1);
        Assert.Equal(expected, end);
    }

    [Test]
    public void ToEndOfHour_at_start_of_hour_returns_same_hour_end()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 0, 0, 0, TimeSpan.Zero);
        var end = dto.ToEndOfHour();
        Assert.Equal(14, end.Hour);
        Assert.Equal(59, end.Minute);
        Assert.Equal(59, end.Second);
        Assert.Equal(999, end.Millisecond);
        Assert.Equal(TimeSpan.Zero, end.Offset);
    }

    #endregion

    #region ToEndOfNextHour / ToEndOfPreviousHour

    [Test]
    public void ToEndOfNextHour_returns_last_tick_of_following_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, TimeSpan.Zero);
        var endNext = dto.ToEndOfNextHour();
        var expected = new DateTimeOffset(2024, 6, 15, 16, 0, 0, 0, TimeSpan.Zero).AddTicks(-1);
        Assert.Equal(expected, endNext);
    }

    [Test]
    public void ToEndOfNextHour_at_last_tick_of_hour_consistent()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 59, 59, 999, TimeSpan.Zero).AddTicks(9999);
        var endNext = dto.ToEndOfNextHour();
        Assert.Equal(15, endNext.Hour);
        Assert.Equal(59, endNext.Minute);
        Assert.Equal(59, endNext.Second);
    }

    [Test]
    public void ToEndOfPreviousHour_returns_last_tick_of_previous_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, TimeSpan.Zero);
        var endPrev = dto.ToEndOfPreviousHour();
        var expected = new DateTimeOffset(2024, 6, 15, 14, 0, 0, 0, TimeSpan.Zero).AddTicks(-1);
        Assert.Equal(expected, endPrev);
    }

    [Test]
    public void ToEndOfPreviousHour_at_midnight_returns_previous_day_23_59_59()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 0, 0, 0, 0, TimeSpan.Zero);
        var endPrev = dto.ToEndOfPreviousHour();
        Assert.Equal(2024, endPrev.Year);
        Assert.Equal(6, endPrev.Month);
        Assert.Equal(14, endPrev.Day);
        Assert.Equal(23, endPrev.Hour);
        Assert.Equal(59, endPrev.Minute);
        Assert.Equal(59, endPrev.Second);
    }

    #endregion

    #region ToHourFormat

    [Test]
    public void ToHourFormat_returns_readable_time_string()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 15, 5, 0, 0, TimeSpan.Zero);
        var formatted = dto.ToHourFormat();
        Assert.NotNull(formatted);
        Assert.NotEmpty(formatted);
        // Format is "h:mm tt" (e.g. "3:05 PM") - culture dependent
        Assert.Contains(":", formatted);
    }

    [Test]
    public void ToHourFormat_noon_and_midnight()
    {
        var noon = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        var midnight = new DateTimeOffset(2024, 6, 15, 0, 0, 0, 0, TimeSpan.Zero);
        Assert.NotEmpty(noon.ToHourFormat());
        Assert.NotEmpty(midnight.ToHourFormat());
    }

    #endregion

    #region ToTzHourFormat / ToTzHourFormatWithTrim

    [Test]
    public void ToTzHourFormat_converts_to_zone_and_formats()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 17, 5, 0, 0, TimeSpan.Zero); // 5:05 PM UTC
        var s = utc.ToTzHourFormat(TimeZoneInfo.Utc);
        Assert.NotNull(s);
        Assert.Contains(":", s);
    }

    [Test]
    public void ToTzHourFormat_throws_on_null_tz()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        Assert.Throws<ArgumentNullException>(() => dto.ToTzHourFormat(null!));
    }

    [Test]
    public void ToTzHourFormatWithTrim_trims_to_start_of_hour_then_formats()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 17, 30, 0, 0, TimeSpan.Zero);
        var s = utc.ToTzHourFormatWithTrim(TimeZoneInfo.Utc);
        Assert.NotNull(s);
        // Should be on the hour in that zone
        Assert.Contains(":", s);
    }

    [Test]
    public void ToTzHourFormatWithTrim_throws_on_null_tz()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        Assert.Throws<ArgumentNullException>(() => dto.ToTzHourFormatWithTrim(null!));
    }

    #endregion

    #region ToTzHoursFromUtc

    [Test]
    public void ToTzHoursFromUtc_utc_zone_returns_same_hour()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 14, 0, 0, 0, TimeSpan.Zero);
        int hour = utc.ToTzHoursFromUtc(14, TimeZoneInfo.Utc);
        Assert.Equal(14, hour);
    }

    [Test]
    public void ToTzHoursFromUtc_anchors_on_utc_date()
    {
        // UTC date 2024-06-15, hour 23 -> in Eastern might be next local day
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        int hour = utc.ToTzHoursFromUtc(23, Eastern);
        Assert.InRange(hour, 0, 23);
        // 23 UTC = 19 Eastern (EDT -4), so we expect 19
        Assert.Equal(19, hour);
    }

    [Test]
    public void ToTzHoursFromUtc_hour_0_and_23_valid()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(0, utc.ToTzHoursFromUtc(0, TimeZoneInfo.Utc));
        Assert.Equal(23, utc.ToTzHoursFromUtc(23, TimeZoneInfo.Utc));
    }

    [Test]
    public void ToTzHoursFromUtc_throws_when_utcHour_negative()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        Assert.Throws<ArgumentOutOfRangeException>(() => utc.ToTzHoursFromUtc(-1, TimeZoneInfo.Utc));
    }

    [Test]
    public void ToTzHoursFromUtc_throws_when_utcHour_24()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        Assert.Throws<ArgumentOutOfRangeException>(() => utc.ToTzHoursFromUtc(24, TimeZoneInfo.Utc));
    }

    [Test]
    public void ToTzHoursFromUtc_throws_on_null_tz()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        Assert.Throws<ArgumentNullException>(() => utc.ToTzHoursFromUtc(12, null!));
    }

    #endregion

    #region ToTzHourFormatFromUtc

    [Test]
    public void ToTzHourFormatFromUtc_returns_formatted_string()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        var s = utc.ToTzHourFormatFromUtc(14, TimeZoneInfo.Utc);
        Assert.NotNull(s);
        Assert.Contains(":", s);
    }

    [Test]
    public void ToTzHourFormatFromUtc_throws_when_utcHour_invalid()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        Assert.Throws<ArgumentOutOfRangeException>(() => utc.ToTzHourFormatFromUtc(-1, TimeZoneInfo.Utc));
        Assert.Throws<ArgumentOutOfRangeException>(() => utc.ToTzHourFormatFromUtc(24, TimeZoneInfo.Utc));
    }

    [Test]
    public void ToTzHourFormatFromUtc_throws_on_null_tz()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        Assert.Throws<ArgumentNullException>(() => utc.ToTzHourFormatFromUtc(12, null!));
    }

    #endregion

    #region ToUtcHoursFromTz

    [Test]
    public void ToUtcHoursFromTz_utc_zone_returns_same_hour()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 14, 0, 0, 0, TimeSpan.Zero);
        int hour = utc.ToUtcHoursFromTz(14, TimeZoneInfo.Utc);
        Assert.Equal(14, hour);
    }

    [Test]
    public void ToUtcHoursFromTz_anchors_on_local_date_in_tz()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 18, 0, 0, 0, TimeSpan.Zero); // 2 PM Eastern
        int hour = utc.ToUtcHoursFromTz(14, Eastern);
        Assert.InRange(hour, 0, 23);
        // 2 PM Eastern on that date (EDT) = 18 UTC
        Assert.Equal(18, hour);
    }

    [Test]
    public void ToUtcHoursFromTz_throws_when_tzHour_invalid()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        Assert.Throws<ArgumentOutOfRangeException>(() => utc.ToUtcHoursFromTz(-1, TimeZoneInfo.Utc));
        Assert.Throws<ArgumentOutOfRangeException>(() => utc.ToUtcHoursFromTz(24, TimeZoneInfo.Utc));
    }

    [Test]
    public void ToUtcHoursFromTz_throws_on_null_tz()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        Assert.Throws<ArgumentNullException>(() => utc.ToUtcHoursFromTz(12, null!));
    }

    /// <summary>
    /// When DST springs forward, 2:00 AM local does not exist. Method should advance to first valid time (3:00 AM) and return its UTC hour.
    /// </summary>
    [Test]
    public void ToUtcHoursFromTz_invalid_time_spring_forward_advances_to_valid()
    {
        // March 10, 2024: DST in US Eastern; 2:00 AM doesn't exist
        var utcOnThatDay = new DateTimeOffset(2024, 3, 10, 12, 0, 0, 0, TimeSpan.Zero);
        int utcHour = utcOnThatDay.ToUtcHoursFromTz(2, Eastern);
        // 2 AM is invalid; code advances to 3 AM EDT = 7 UTC
        Assert.Equal(7, utcHour);
    }

    /// <summary>
    /// When DST falls back, 1:00 AM local occurs twice. Method should choose earlier UTC (larger offset = EDT).
    /// </summary>
    [Test]
    public void ToUtcHoursFromTz_ambiguous_time_fall_back_chooses_earlier_utc()
    {
        // November 3, 2024: fall back in US Eastern; 1:00 AM happens twice (EDT then EST)
        var utcOnThatDay = new DateTimeOffset(2024, 11, 3, 12, 0, 0, 0, TimeSpan.Zero);
        int utcHour = utcOnThatDay.ToUtcHoursFromTz(1, Eastern);
        // Earlier UTC = first occurrence = 1 AM EDT = 5 UTC
        Assert.Equal(5, utcHour);
    }

    [Test]
    public void ToUtcHoursFromTz_hour_0_and_23_valid()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(0, utc.ToUtcHoursFromTz(0, TimeZoneInfo.Utc));
        Assert.Equal(23, utc.ToUtcHoursFromTz(23, TimeZoneInfo.Utc));
    }

    #endregion

    #region Weird / edge scenarios

    [Test]
    public void Offset_preserved_through_start_and_end_of_hour()
    {
        var offset = TimeSpan.FromHours(5).Add(TimeSpan.FromMinutes(30));
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, offset);
        Assert.Equal(offset, dto.ToStartOfHour().Offset);
        Assert.Equal(offset, dto.ToEndOfHour().Offset);
        Assert.Equal(offset, dto.ToStartOfNextHour().Offset);
        Assert.Equal(offset, dto.ToStartOfPreviousHour().Offset);
        Assert.Equal(offset, dto.ToEndOfNextHour().Offset);
        Assert.Equal(offset, dto.ToEndOfPreviousHour().Offset);
    }

    [Test]
    public void ToEndOfNextHour_and_ToStartOfNextHour_are_adjacent()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, TimeSpan.Zero);
        var startNext = dto.ToStartOfNextHour();
        var endNext = dto.ToEndOfNextHour();
        Assert.Equal(startNext.AddHours(1).AddTicks(-1), endNext);
    }

    [Test]
    public void ToEndOfPreviousHour_and_ToStartOfPreviousHour_are_same_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, TimeSpan.Zero);
        var startPrev = dto.ToStartOfPreviousHour();
        var endPrev = dto.ToEndOfPreviousHour();
        Assert.Equal(startPrev.AddHours(1).AddTicks(-1), endPrev);
    }

    [Test]
    public void utcInstant_with_non_utc_offset_normalized_to_utc_date()
    {
        // Local 2024-06-15 00:30 UTC-4 => UTC 2024-06-15 04:30
        var withOffset = new DateTimeOffset(2024, 6, 15, 0, 30, 0, 0, TimeSpan.FromHours(-4));
        int tzHour = withOffset.ToTzHoursFromUtc(0, TimeZoneInfo.Utc);
        Assert.Equal(0, tzHour);
        // Date used is UTC date (year/month/day of 2024-06-15 04:30 UTC = 2024-06-15)
        int utcBack = withOffset.ToUtcHoursFromTz(0, TimeZoneInfo.Utc);
        Assert.Equal(0, utcBack);
    }

    [Test]
    public void ToHourFormat_invariant_culture_consistent()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 15, 5, 0, 0, TimeSpan.Zero);
        var prev = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            string formatted = dto.ToHourFormat();
            Assert.Equal("3:05 PM", formatted);
        }
        finally
        {
            CultureInfo.CurrentCulture = prev;
        }
    }

    #endregion
}
