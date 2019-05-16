using System;
using System.Diagnostics;
using Ninject;
using RepositoryScanner.Scanning;
using RepositoryScanner.Scanning.Analysis;
using RepositoryScanner.Scanning.FileExplorer;
using ILogger = Ninject.Extensions.Logging.ILogger;

namespace RepositoryScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("############ STARTING UP... ############");
            IKernel kernel = new StandardKernel(new ScanningModule());
            ILogger logger = kernel.Get<ILogger>();

            Console.WriteLine("############ SCANNING FILES... ############");
            var fileVisitor = kernel.Get<RepositoryVisitor>();
            fileVisitor.Visit();

            var codeBase = fileVisitor.GetCodeBase();

            var problemFinder = kernel.Get<ProblemFinder>();

            Console.WriteLine("############ ANALYZING... ############");
            var totalProblems = 0;
            foreach (var problem in problemFinder.FindProblems(codeBase))
            {
                logger.Error($"{problem.Name} => {problem.Description}");
                totalProblems++;
            }
            stopwatch.Stop();

            if (totalProblems > 0)
            {
                logger.Info($"Total problems found: {totalProblems}.");
                logger.Info($"Found in: {stopwatch.Elapsed.TotalSeconds}s.");
            }

            Console.WriteLine("############ DONE ############");
            Console.WriteLine($"############ EXECUTION TIME : {stopwatch.Elapsed.TotalSeconds}s ############");

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
