[![](https://img.shields.io/nuget/v/soenneker.extensions.datetimeoffsets.hours.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.hours/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.hours/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.hours/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.datetimeoffsets.hours.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.hours/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.hours/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.hours/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.DateTimeOffsets.Hours

Provides fixed-offset hour boundaries, local-hour formatting, and hour-of-day conversion between UTC and named time zones.

## Installation

```bash
dotnet add package Soenneker.Extensions.DateTimeOffsets.Hours
```

## Hour boundaries

```csharp
using Soenneker.Extensions.DateTimeOffsets.Hours;

DateTimeOffset value = new(2026, 8, 29, 16, 42, 30, TimeSpan.FromHours(-4));

DateTimeOffset start = value.ToStartOfHour();
DateTimeOffset end = value.ToEndOfHour();
DateTimeOffset previousStart = value.ToStartOfPreviousHour();
DateTimeOffset nextEnd = value.ToEndOfNextHour();
```

| Method | Result for `16:42:30` |
| --- | --- |
| `ToStartOfHour()` | `16:00:00` |
| `ToEndOfHour()` | One tick before `17:00:00` |
| `ToStartOfPreviousHour()` | `15:00:00` |
| `ToEndOfPreviousHour()` | One tick before `16:00:00` |
| `ToStartOfNextHour()` | `17:00:00` |
| `ToEndOfNextHour()` | One tick before `18:00:00` |

These methods preserve the stored offset and use fixed elapsed hours. They do not consult a named time zone or reinterpret repeated/skipped local clock hours.

## Format an hour

```csharp
string storedClock = value.ToHourFormat();

TimeZoneInfo eastern = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
string easternClock = value.ToTzHourFormat(eastern);
string easternHour = value.ToTzHourFormatWithTrim(eastern);
```

All three methods use `h:mm tt` with the current culture. `ToHourFormat()` formats the stored clock fields. `ToTzHourFormat()` first converts the instant to the target zone. `ToTzHourFormatWithTrim()` also resets local minutes and seconds to zero.

## Convert a UTC hour to local time

```csharp
DateTimeOffset reference = new(2026, 8, 29, 12, 0, 0, TimeSpan.Zero);

int localHour = reference.ToTzHoursFromUtc(utcHour: 18, eastern);
string localClock = reference.ToTzHourFormatFromUtc(utcHour: 18, eastern);
```

Both methods combine `utcHour` with the UTC date of `reference`, then convert that instant to the target zone. The integer method returns only `0–23`; it discards the local date and minutes. The formatted method preserves fractional-hour offsets such as `5:30 AM`.

## Convert a local hour to UTC

```csharp
int utcHour = reference.ToUtcHoursFromTz(tzHour: 9, eastern);
```

`ToUtcHoursFromTz()` chooses the local date in `eastern` that contains `reference`, interprets `tzHour` on that date, and returns only the UTC hour from `0` through `23`.

If the local wall time is invalid, it advances minute-by-minute to the first valid minute. If it is ambiguous, it selects the earlier UTC instant. The returned integer does not include UTC date rollover or minutes; use a full-instant API when those details matter.

Hour arguments must be from `0` through `23`.
