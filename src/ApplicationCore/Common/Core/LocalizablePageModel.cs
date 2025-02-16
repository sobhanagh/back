namespace GamaEdtech.Backend.Common.Core
{
    using System;

    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public abstract class LocalizablePageModel<T>(Lazy<ILogger<T>> logger, Lazy<IStringLocalizer<T>> localizer) : PageModel<T>(logger)
        where T : class
    {
        protected Lazy<IStringLocalizer<T>> Localizer { get; } = localizer;
    }
}
