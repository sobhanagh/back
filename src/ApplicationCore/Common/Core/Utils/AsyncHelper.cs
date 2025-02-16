namespace GamaEdtech.Backend.Common.Core.Utils
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    public static class AsyncHelper
    {
        private static readonly TaskFactory TaskFactory = new(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            var cultureUi = CultureInfo.CurrentUICulture;
            var culture = CultureInfo.CurrentCulture;
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            return TaskFactory.StartNew(() =>
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = cultureUi;
                return func();
            }).Unwrap().GetAwaiter().GetResult();
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
        }

        public static void RunSync(Func<Task> func)
        {
            var cultureUi = CultureInfo.CurrentUICulture;
            var culture = CultureInfo.CurrentCulture;
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            TaskFactory.StartNew(() =>
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = cultureUi;
                return func();
            }).Unwrap().GetAwaiter().GetResult();
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
        }
    }
}
