using System;
using System.Linq;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Logging.Serilog.Infrastructure;
using Ninject.Modules;
using RepositoryScanner.V2.FileExplorer;
using RepositoryScanner.V2.Structure;
using RepositoryScanner.V2.StructureParsing;
using Serilog;
using ILogger = Ninject.Extensions.Logging.ILogger;

namespace RepositoryScanner.V2
{
    class ScanningModule : NinjectModule
    {
        public override void Load()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File("log.txt").CreateLogger();
            Bind<IFileVisitorFilter>().To<DefaultFileVisitorFilter>();
            Bind<IParser<Solution>>().To<SolutionParser>();
            Bind<FileVisitor>().To<FileVisitor>();
        }
    }
}
