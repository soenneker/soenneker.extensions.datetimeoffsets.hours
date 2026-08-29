[![](https://img.shields.io/nuget/v/soenneker.extensions.datetimeoffsets.hours.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.hours/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.hours/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.hours/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.datetimeoffsets.hours.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.hours/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.hours/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.hours/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.DateTimeOffsets.Hours
A collection of helpful DateTimeOffset hour extension methods.

## Installation

```bash
dotnet add package Soenneker.Extensions.DateTimeOffsets.Hours
```

## Quick start

```csharp
using Soenneker.Extensions.DateTimeOffsets.Hours;

DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
var result = dateTimeOffset.ToStartOfHour();
```

## Common operations

- `ToStartOfHour()` - Returns the start of the hour containing `dateTimeOffset` (minute/second/fraction set to zero).
- `ToStartOfNextHour()` - Returns the start of the next hour relative to `dateTimeOffset`.
- `ToStartOfPreviousHour()` - Returns the start of the previous hour relative to `dateTimeOffset`.
- `ToEndOfHour()` - Returns the end of the hour containing `dateTimeOffset` (one tick before the next hour).
- `ToEndOfNextHour()` - Returns the end of the next hour relative to `dateTimeOffset`.
- `ToEndOfPreviousHour()` - Returns the end of the previous hour relative to `dateTimeOffset`.
- `ToTzHourFormat()` - Converts `instant` to `tz` and formats the local wall-clock time as `"h:mm tt"` using the current culture. Returns a culture-formatted local time string such as `"3:05 PM"`.
- `ToTzHourFormatWithTrim()` - Converts `instant` to `tz`, trims to the start of the local hour, and formats the result as `"h:mm tt"` using the current culture.
- `ToHourFormat()` - Formats `dateTimeOffset` as `"h:mm tt"` using the current culture. Returns a culture-formatted time string such as `"3:05 PM"`.
- `ToTzHoursFromUtc()` - Converts a UTC hour-of-day to the corresponding local hour-of-day in `tz`, anchored to the UTC date of `utcInstant`. Returns the local hour-of-day [0.23] for the instant represented by (UTC date from `utcInstant`, `utcHour`). This method intentionally anchors on the UTC date, not the target time zone's local date.
- `ToTzHourFormatFromUtc()` - Formats a UTC hour-of-day as a local time string in `tz`, anchored to the UTC date of `utcInstant`. Returns a culture-formatted local time string such as `"3:00 PM"`. This computes the target local wall-clock time by converting the UTC instant (UTC date + `utcHour`) into `tz`.
- `ToUtcHoursFromTz()` - Converts a local hour-of-day in `tz` to the corresponding UTC hour-of-day, anchored to the local DATE in `tz` that contains `utcInstant`. Returns the UTC hour-of-day [0.23] corresponding to the requested local wall time on the selected local date, using zone rules.
