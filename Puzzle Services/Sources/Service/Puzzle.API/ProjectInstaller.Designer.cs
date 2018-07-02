namespace Puzzle.API
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PuzzleserviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.PuzzleServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // PuzzleserviceProcessInstaller
            // 
            this.PuzzleserviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            this.PuzzleserviceProcessInstaller.Password = null;
            this.PuzzleserviceProcessInstaller.Username = null;
            // 
            // PuzzleServiceInstaller
            // 
            this.PuzzleServiceInstaller.DelayedAutoStart = true;
            this.PuzzleServiceInstaller.Description = "Services for GFT Puzzle Test";
            this.PuzzleServiceInstaller.DisplayName = "GFT Test Puzzle Service";
            this.PuzzleServiceInstaller.ServiceName = "GFT Test Puzzle Services";
            this.PuzzleServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.PuzzleserviceProcessInstaller,
            this.PuzzleServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller PuzzleserviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller PuzzleServiceInstaller;
    }
}