using System;
using Ninject;
using Ninject.Extensions.Logging.Serilog.Infrastructure;
using RepositoryReaders.Directory;
using RepositoryReaders.Text;
using RepositoryScanComparer.ScanComparison;

namespace RepositoryScanComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel(new ScanComparisonModule());

            Console.WriteLine("############ STARTING UP... ############");


            Console.WriteLine("############ COMPARING WITH PREVIOUS... ############");
            var scanComparer = kernel.Get<ScanComparer>();
            scanComparer.CompareCurrentWithPrevious();


            Console.WriteLine("############ DONE ############");
            Console.ReadLine();
        }
    }
}
