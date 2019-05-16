using System;
using Ninject.Extensions.Logging.Serilog.Infrastructure;
using Ninject.Modules;
using RepositoryReaders.Directory;
using RepositoryReaders.Text;
using RepositoryReaders.Xml;
using RepositoryScanner.Scanning.Analysis;
using RepositoryScanner.Scanning.FileExplorer;
using RepositoryScanner.Scanning.Structure;
using RepositoryScanner.Scanning.StructureParsing.Parsers;
using RepositoryScanner.Scanning.StructureParsing.Parsers.Projects;
using RepositoryScanner.Scanning.StructureParsing.Parsers.Solutions;
using Serilog;
using ILogger = Ninject.Extensions.Logging.ILogger;

namespace RepositoryScanner.Scanning
{
    class ScanningModule : NinjectModule
    {
        public override void Load()
        {
            var dateOfScan = DateTime.Now;
            Log.Logger = new LoggerConfiguration().WriteTo.Async(x => x.File($"log_{dateOfScan.ToString("yyyy_MM_d__HH_mm_ss")}.txt")).CreateLogger();
            Bind<IConfigurableAnalyzerCollection>().To<ConfigurableAnalyzerCollection>();
            Bind<IRepositoryVisitorFilter>().To<DefaultRepositoryVisitorFilter>();
            Bind<IParser<Solution>>().To<SolutionParser>();
            Bind<IRepositoryRegistry>().To<ConfigurableRepositoryRegistry>();
            Bind<IXmlReader>().To<DefaultXmlReader>();
            Bind<IParserFactory<IParser<Project>>>().To<ProjectParserFactory>();
            Bind<RepositoryVisitor>().To<RepositoryVisitor>();
            Bind<ProblemFinder>().To<ProblemFinder>();
            Bind<IFileReader>().To<DefaultFileReader>();
            Bind<IDirectoryReader>().To<DefaultDirectoryReader>();
            Bind<ILogger>().ToMethod((context) => new SerilogLogger("Scanner"));
        }
    }
}
