using CpuShutdown.Settings;
using System;
using System.Windows.Forms;

namespace CpuShutdown.UI.Tray.Services.UiDialogs
{

    public partial class AboutForm : Form
    {

        public AboutForm()
        {
            InitializeComponent();

            Text = $"About {AppSettings.ApplicationName}";
            Icon = Properties.Resources.IconApplication;

            labelName.Text = AppSettings.ApplicationName;
            labelVersion.Text = $"Version {AppSettings.ApplicationVersion}";
            textBoxLicense.Text = AppSettings.ApplicationLicense;

            buttonOk.Click += (object sender, EventArgs e) => Close();
        }


        protected override void OnLoad(EventArgs e)
        {
            Helpers.RemoveSystemMenuMaximize(Handle);
            Helpers.RemoveSystemMenuSize(Handle);

            base.OnLoad(e);
        }

    }

}
