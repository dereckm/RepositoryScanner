using System;
using Ninject.Extensions.Logging.Serilog.Infrastructure;
using Ninject.Modules;
using RepositoryReaders.Directory;
using RepositoryReaders.Path;
using RepositoryReaders.Text;
using Serilog;
using ILogger = Ninject.Extensions.Logging.ILogger;

namespace RepositoryScanComparer.ScanComparison
{
    internal class ScanComparisonModule : NinjectModule
    {
        public override void Load()
        {
            var dateOfScan = DateTime.Now;
            Log.Logger = new LoggerConfiguration().WriteTo.File($"comparison_{dateOfScan.ToString("yyyy_MM_d__HH_mm_ss")}.txt").CreateLogger();
            Bind<ScanComparer>().ToSelf();
            Bind<ILogger>().ToMethod((context) => new SerilogLogger("Comparer"));
            Bind<IFileReader>().To<DefaultFileReader>();
            Bind<IDirectoryReader>().To<DefaultDirectoryReader>();
            Bind<IPathReader>().To<DefaultPathReader>();
        }
    }
}