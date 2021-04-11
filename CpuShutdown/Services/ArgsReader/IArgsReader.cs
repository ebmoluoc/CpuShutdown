namespace CpuShutdown.Services.ArgsReader
{
    public interface IArgsReader
    {
        string[] Args { get; set; }
        string PipeHandle { get; }
    }
}
