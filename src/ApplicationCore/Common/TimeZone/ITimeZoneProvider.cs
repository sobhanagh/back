namespace GamaEdtech.Backend.Common.TimeZone
{
    using System.Collections.Generic;

    using GamaEdtech.Backend.Common.DataAnnotation;

    using GamaEdtech.Backend.Common.Data;

    [Injectable]
    public interface ITimeZoneProvider
    {
        IEnumerable<TimeZoneDto>? GetTimeZones();
    }
}
