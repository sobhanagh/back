namespace GamaEdtech.Backend.Common.TimeZone
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using GamaEdtech.Backend.Common.Caching;
    using GamaEdtech.Backend.Common.Data;

    using static GamaEdtech.Backend.Common.Core.Constants;

    public class TimeZoneProvider(Lazy<ICacheProvider> cacheProvider) : ITimeZoneProvider
    {
        private readonly Lazy<ICacheProvider> cacheProvider = cacheProvider;

        public IEnumerable<TimeZoneDto>? GetTimeZones() => cacheProvider.Value.Get(TimeZoneIdClaim, () =>
                                                                        TimeZoneInfo.GetSystemTimeZones().Select(t => new TimeZoneDto
                                                                        {
                                                                            Id = t.Id,
                                                                            BaseUtcOffset = t.BaseUtcOffset,
                                                                            DisplayName = t.DisplayName,
                                                                        }));
    }
}
