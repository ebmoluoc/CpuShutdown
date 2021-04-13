namespace CpuShutdown.UI.Tray.Services.UiDialogs
{
    public interface IUiDialog
    {
        bool IsShown { get; }
        void Show();
    }
}
