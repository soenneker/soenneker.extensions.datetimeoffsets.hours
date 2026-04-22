using System;
using System.Globalization;
using AwesomeAssertions;
using Soenneker.Extensions.DateTimeOffsets.Hours;
using Soenneker.Tests.Unit;

namespace Soenneker.Extensions.DateTimeOffsets.Hours.Tests;

public sealed class DateTimeOffsetsHourExtensionTests : UnitTest
{
    private static readonly TimeZoneInfo Eastern = TimeZoneInfo.FindSystemTimeZoneById(
        OperatingSystem.IsWindows() ? "Eastern Standard Time" : "America/New_York");
    private static readonly TimeSpan ZeroOffset = TimeSpan.Zero;

    #region ToStartOfHour

    [Test]
    public void ToStartOfHour_mid_hour_returns_start_of_same_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 45, 123, TimeSpan.FromHours(-4));
        var start = dto.ToStartOfHour();
        start.Should().Be(new DateTimeOffset(2024, 6, 15, 14, 0, 0, 0, dto.Offset));
    }

    [Test]
    public void ToStartOfHour_already_at_start_preserves_value()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 0, 0, 0, TimeSpan.Zero);
        var start = dto.ToStartOfHour();
        start.Should().Be(dto);
    }

    [Test]
    public void ToStartOfHour_last_tick_of_hour_returns_start_of_that_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 59, 59, 999, TimeSpan.Zero).AddTicks(9999);
        var start = dto.ToStartOfHour();
        start.Should().Be(new DateTimeOffset(2024, 6, 15, 14, 0, 0, 0, ZeroOffset));
    }

    #endregion

    #region ToStartOfNextHour / ToStartOfPreviousHour

    [Test]
    public void ToStartOfNextHour_returns_first_moment_of_following_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, TimeSpan.Zero);
        var next = dto.ToStartOfNextHour();
        next.Should().Be(new DateTimeOffset(2024, 6, 15, 15, 0, 0, 0, ZeroOffset));
    }

    [Test]
    public void ToStartOfNextHour_at_midnight_returns_next_day_one_am()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 0, 0, 0, 0, TimeSpan.Zero);
        var next = dto.ToStartOfNextHour();
        next.Should().Be(new DateTimeOffset(2024, 6, 15, 1, 0, 0, 0, ZeroOffset));
    }

    [Test]
    public void ToStartOfNextHour_last_tick_of_day_returns_next_day()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 23, 59, 59, 999, TimeSpan.Zero).AddTicks(9999);
        var next = dto.ToStartOfNextHour();
        next.Should().Be(new DateTimeOffset(2024, 6, 16, 0, 0, 0, 0, ZeroOffset));
    }

    [Test]
    public void ToStartOfPreviousHour_returns_first_moment_of_previous_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, TimeSpan.Zero);
        var prev = dto.ToStartOfPreviousHour();
        prev.Should().Be(new DateTimeOffset(2024, 6, 15, 13, 0, 0, 0, ZeroOffset));
    }

    [Test]
    public void ToStartOfPreviousHour_at_midnight_returns_previous_day_23_00()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 0, 0, 0, 0, TimeSpan.Zero);
        var prev = dto.ToStartOfPreviousHour();
        prev.Should().Be(new DateTimeOffset(2024, 6, 14, 23, 0, 0, 0, ZeroOffset));
    }

    #endregion

    #region ToEndOfHour

    [Test]
    public void ToEndOfHour_returns_last_tick_of_same_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, TimeSpan.Zero);
        var end = dto.ToEndOfHour();
        var expected = new DateTimeOffset(2024, 6, 15, 15, 0, 0, 0, TimeSpan.Zero).AddTicks(-1);
        end.Should().Be(expected);
    }

    [Test]
    public void ToEndOfHour_at_start_of_hour_returns_same_hour_end()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 0, 0, 0, TimeSpan.Zero);
        var end = dto.ToEndOfHour();
        end.Hour.Should().Be(14);
        end.Minute.Should().Be(59);
        end.Second.Should().Be(59);
        end.Millisecond.Should().Be(999);
        end.Offset.Should().Be(ZeroOffset);
    }

    #endregion

    #region ToEndOfNextHour / ToEndOfPreviousHour

    [Test]
    public void ToEndOfNextHour_returns_last_tick_of_following_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, TimeSpan.Zero);
        var endNext = dto.ToEndOfNextHour();
        var expected = new DateTimeOffset(2024, 6, 15, 16, 0, 0, 0, TimeSpan.Zero).AddTicks(-1);
        endNext.Should().Be(expected);
    }

    [Test]
    public void ToEndOfNextHour_at_last_tick_of_hour_consistent()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 59, 59, 999, TimeSpan.Zero).AddTicks(9999);
        var endNext = dto.ToEndOfNextHour();
        endNext.Hour.Should().Be(15);
        endNext.Minute.Should().Be(59);
        endNext.Second.Should().Be(59);
    }

    [Test]
    public void ToEndOfPreviousHour_returns_last_tick_of_previous_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, TimeSpan.Zero);
        var endPrev = dto.ToEndOfPreviousHour();
        var expected = new DateTimeOffset(2024, 6, 15, 14, 0, 0, 0, TimeSpan.Zero).AddTicks(-1);
        endPrev.Should().Be(expected);
    }

    [Test]
    public void ToEndOfPreviousHour_at_midnight_returns_previous_day_23_59_59()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 0, 0, 0, 0, TimeSpan.Zero);
        var endPrev = dto.ToEndOfPreviousHour();
        endPrev.Year.Should().Be(2024);
        endPrev.Month.Should().Be(6);
        endPrev.Day.Should().Be(14);
        endPrev.Hour.Should().Be(23);
        endPrev.Minute.Should().Be(59);
        endPrev.Second.Should().Be(59);
    }

    #endregion

    #region ToHourFormat

    [Test]
    public void ToHourFormat_returns_readable_time_string()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 15, 5, 0, 0, TimeSpan.Zero);
        var formatted = dto.ToHourFormat();
        formatted.Should().NotBeNull();
        formatted.Should().NotBeEmpty();
        // Format is "h:mm tt" (e.g. "3:05 PM") - culture dependent
        formatted.Should().Contain(":");
    }

    [Test]
    public void ToHourFormat_noon_and_midnight()
    {
        var noon = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        var midnight = new DateTimeOffset(2024, 6, 15, 0, 0, 0, 0, TimeSpan.Zero);
        noon.ToHourFormat().Should().NotBeEmpty();
        midnight.ToHourFormat().Should().NotBeEmpty();
    }

    #endregion

    #region ToTzHourFormat / ToTzHourFormatWithTrim

    [Test]
    public void ToTzHourFormat_converts_to_zone_and_formats()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 17, 5, 0, 0, TimeSpan.Zero); // 5:05 PM UTC
        var s = utc.ToTzHourFormat(TimeZoneInfo.Utc);
        s.Should().NotBeNull();
        s.Should().Contain(":");
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
        s.Should().NotBeNull();
        // Should be on the hour in that zone
        s.Should().Contain(":");
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
        hour.Should().Be(14);
    }

    [Test]
    public void ToTzHoursFromUtc_anchors_on_utc_date()
    {
        // UTC date 2024-06-15, hour 23 -> in Eastern might be next local day
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        int hour = utc.ToTzHoursFromUtc(23, Eastern);
        hour.Should().BeInRange(0, 23);
        // 23 UTC = 19 Eastern (EDT -4), so we expect 19
        hour.Should().Be(19);
    }

    [Test]
    public void ToTzHoursFromUtc_hour_0_and_23_valid()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        utc.ToTzHoursFromUtc(0, TimeZoneInfo.Utc).Should().Be(0);
        utc.ToTzHoursFromUtc(23, TimeZoneInfo.Utc).Should().Be(23);
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
        s.Should().NotBeNull();
        s.Should().Contain(":");
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
        hour.Should().Be(14);
    }

    [Test]
    public void ToUtcHoursFromTz_anchors_on_local_date_in_tz()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 18, 0, 0, 0, TimeSpan.Zero); // 2 PM Eastern
        int hour = utc.ToUtcHoursFromTz(14, Eastern);
        hour.Should().BeInRange(0, 23);
        // 2 PM Eastern on that date (EDT) = 18 UTC
        hour.Should().Be(18);
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
        utcHour.Should().Be(7);
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
        utcHour.Should().Be(5);
    }

    [Test]
    public void ToUtcHoursFromTz_hour_0_and_23_valid()
    {
        var utc = new DateTimeOffset(2024, 6, 15, 12, 0, 0, 0, TimeSpan.Zero);
        utc.ToUtcHoursFromTz(0, TimeZoneInfo.Utc).Should().Be(0);
        utc.ToUtcHoursFromTz(23, TimeZoneInfo.Utc).Should().Be(23);
    }

    #endregion

    #region Weird / edge scenarios

    [Test]
    public void Offset_preserved_through_start_and_end_of_hour()
    {
        var offset = TimeSpan.FromHours(5).Add(TimeSpan.FromMinutes(30));
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, offset);
        dto.ToStartOfHour().Offset.Should().Be(offset);
        dto.ToEndOfHour().Offset.Should().Be(offset);
        dto.ToStartOfNextHour().Offset.Should().Be(offset);
        dto.ToStartOfPreviousHour().Offset.Should().Be(offset);
        dto.ToEndOfNextHour().Offset.Should().Be(offset);
        dto.ToEndOfPreviousHour().Offset.Should().Be(offset);
    }

    [Test]
    public void ToEndOfNextHour_and_ToStartOfNextHour_are_adjacent()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, TimeSpan.Zero);
        var startNext = dto.ToStartOfNextHour();
        var endNext = dto.ToEndOfNextHour();
        endNext.Should().Be(startNext.AddHours(1).AddTicks(-1));
    }

    [Test]
    public void ToEndOfPreviousHour_and_ToStartOfPreviousHour_are_same_hour()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, 0, TimeSpan.Zero);
        var startPrev = dto.ToStartOfPreviousHour();
        var endPrev = dto.ToEndOfPreviousHour();
        endPrev.Should().Be(startPrev.AddHours(1).AddTicks(-1));
    }

    [Test]
    public void utcInstant_with_non_utc_offset_normalized_to_utc_date()
    {
        // Local 2024-06-15 00:30 UTC-4 => UTC 2024-06-15 04:30
        var withOffset = new DateTimeOffset(2024, 6, 15, 0, 30, 0, 0, TimeSpan.FromHours(-4));
        int tzHour = withOffset.ToTzHoursFromUtc(0, TimeZoneInfo.Utc);
        tzHour.Should().Be(0);
        // Date used is UTC date (year/month/day of 2024-06-15 04:30 UTC = 2024-06-15)
        int utcBack = withOffset.ToUtcHoursFromTz(0, TimeZoneInfo.Utc);
        utcBack.Should().Be(0);
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
            formatted.Should().Be("3:05 PM");
        }
        finally
        {
            CultureInfo.CurrentCulture = prev;
        }
    }

    #endregion
}
