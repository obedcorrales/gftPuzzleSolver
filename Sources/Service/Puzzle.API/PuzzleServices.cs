using System;
using System.ServiceProcess;
using Microsoft.Owin.Hosting;

namespace Puzzle.API
{
    partial class PuzzleServices : ServiceBase
    {
        private IDisposable _server = null;

        public PuzzleServices()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (!string.IsNullOrWhiteSpace(Program.BaseAddress))
                _server = WebApp.Start<Startup>(url: Program.BaseAddress);
        }

        protected override void OnStop()
        {
            if (_server != null)
                _server.Dispose();

            base.OnStop();
        }
    }
}
