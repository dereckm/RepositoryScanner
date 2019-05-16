namespace RepositoryScanner.Scanning.Analysis
{
    public enum ProblemType
    {
        EmptyProject,
        EmptySolution,
        FileNotReferenced,
        MultipleProjectReferenceSameFile,
        ProjectNotReferenced,
        ProjectReferenceFileInDifferentRepository,
        ProjectReferencedFileNotFound,
        MissingObfuscationTagOnClass,
    }

    public class Problem
    {
        public Problem(ProblemType problemType, string name, string description)
        {
            ProblemType = problemType;
            Name = name;
            Description = description;
        }

        public ProblemType ProblemType { get; }
        public string Name { get; }
        public string Description { get; }
    }
}
