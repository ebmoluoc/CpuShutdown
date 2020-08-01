namespace CpuShutdown.Services.ArgsReader
{
    public interface IArgsReader
    {
        string[] Args { get; set; }
        string ProjectGuid { get; }
        string PipeHandle { get; }
    }
}
