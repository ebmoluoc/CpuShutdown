namespace CpuShutdown.UI.Tray.Services.UiAbout
{
    public interface IUiAbout
    {
        bool IsShown { get; }
        void Show();
    }
}
