using System;
using System.IO;
using System.Security.Cryptography;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.Extensions.Logging.Serilog.Infrastructure;
using Ninject.Parameters;
using RepositoryScanner.V2;
using RepositoryScanner.V2.Analysis;
using RepositoryScanner.V2.FileExplorer;
using RepositoryScanner.V2.Structure;

namespace RepositoryScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel(new ScanningModule());
            var designPath = @"C:\Source\Design";
            var commonPath = @"C:\Source\Common";
            var unityPath = @"C:\Source\Unity";
            var fileVisitor = kernel.Get<FileVisitor>();

            fileVisitor.Visit(designPath);
            fileVisitor.Visit(commonPath);
            fileVisitor.Visit(unityPath);

            var codeBase = fileVisitor.GetCodeBase();

            var problemFinder = new ProblemFinder(codeBase);

            var logger = new SerilogLogger("Scanner");

            problemFinder.AddAnalyzer(new FileReferencedByMultipleProjectsAnalyzer());
            problemFinder.AddAnalyzer(new FileNeverReferencedAnalyzer());
            problemFinder.AddAnalyzer(new EmptyProjectAnalyzer());
            problemFinder.AddAnalyzer(new EmptySolutionAnalyzer());
            problemFinder.AddAnalyzer(new ReferencedFileNotFoundAnalyzer());
            problemFinder.AddAnalyzer(new ProjectNotReferencedInAnySolutionAnalyzer());
            problemFinder.AddAnalyzer(new ProjectReferencesFileInAnotherRepositoryAnalyzer());

            var totalProblems = 0;
            foreach (var problem in problemFinder.Execute())
            {
                logger.Error($"{problem.Name} => {problem.Description}");
                totalProblems++;
            }

            logger.Info($"Total problems found: {totalProblems}.");

            Console.WriteLine("############ DONE ############");
            Console.ReadLine();


            #region OLD

            //    //TODO : Take those from user (cmd args or Winforms)
            //    const string commonRepositoryName = "Common";
            //    const string commonRepositoryFolderPath = @"D:\iBWave\src\Common";
            //    const string designRepositoryName = "Design";
            //    const string designRepositoryFolderPath = @"D:\iBWave\src\Design";
            //    const string unityRepositoryName = "Unity";
            //    const string unityRepositoryFolderPath = @"D:\iBWave\src\Unity";
            //    const string outputFolderPath = @"C:\Users\averbuk\Documents\iBwave\General Business\2019\Repositories mess";

            //    TryCleanOutput(outputFolderPath);

            //    var commonScanResult = V1.RepositoryScanner.ScanRepository(commonRepositoryName, commonRepositoryFolderPath);
            //    var designScanResult = V1.RepositoryScanner.ScanRepository(designRepositoryName, designRepositoryFolderPath);
            //    var unityScanResult = V1.RepositoryScanner.ScanRepository(unityRepositoryName, unityRepositoryFolderPath);
            //    var scanResults = new[] {commonScanResult, designScanResult, unityScanResult};

            //    V1.RepositoryScanner.SetFileInProjectRepositoryProperties(scanResults);

            //    V1.RepositoryScanner.SaveRawResults(commonScanResult, outputFolderPath);
            //    V1.RepositoryScanner.SaveRawResults(designScanResult, outputFolderPath);
            //    V1.RepositoryScanner.SaveRawResults(unityScanResult, outputFolderPath);

            //    V1.RepositoryScanner.SaveSolutionResults(scanResults, outputFolderPath);
            //    V1.RepositoryScanner.SaveProjectsReport(scanResults, outputFolderPath);
            //    V1.RepositoryScanner.SaveFilesReport(scanResults, outputFolderPath);

            //    Console.WriteLine("Press any key to quit");
            //    Console.ReadKey();
            //}

            //private static void TryCleanOutput(string directoryToClean)
            //{
            //    try
            //    {
            //        var di = new DirectoryInfo(directoryToClean);

            //        foreach (var file in di.GetFiles())
            //        {
            //            file.Delete(); 
            //        }

            //        foreach (var dir in di.GetDirectories())
            //        {
            //            dir.Delete(true); 
            //        }
            //    }
            //    catch{}

            #endregion
        }
    }
}
