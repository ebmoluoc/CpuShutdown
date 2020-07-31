namespace CpuShutdown.Services.UiSettings
{
    public interface IUiSettings
    {
        bool IsShown { get; }
        void Show();
    }
}
