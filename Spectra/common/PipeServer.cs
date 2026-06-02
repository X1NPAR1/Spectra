using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace Spectra.common
{
    sealed class PipeServer : IDisposable
    {
        public const string PipeName = "SpectraControl";

        private readonly IVibranceProxy _proxy;
        private volatile bool _running = true;
        private readonly Thread _thread;

        public PipeServer(IVibranceProxy proxy)
        {
            _proxy = proxy;
            _thread = new Thread(ServeLoop) { IsBackground = true, Name = "SpectraPipe" };
            _thread.Start();
        }

        private void ServeLoop()
        {
            while (_running)
            {
                NamedPipeServerStream pipe = null;
                try
                {
                    pipe = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1,
                        PipeTransmissionMode.Byte, PipeOptions.None, 512, 512);
                    pipe.WaitForConnection();
                    if (!_running) break;

                    using (var reader = new StreamReader(pipe))
                    using (var writer = new StreamWriter(pipe) { AutoFlush = true })
                    {
                        string cmd = reader.ReadLine() ?? "";
                        writer.WriteLine(Handle(cmd));
                    }
                }
                catch (IOException) { }
                catch (ObjectDisposedException) { break; }
                finally { pipe?.Dispose(); }
            }
        }

        private string Handle(string cmd)
        {
            var parts = cmd.Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "ERROR:empty";

            switch (parts[0].ToLowerInvariant())
            {
                case "status":
                {
                    var info = _proxy.GetVibranceInfo();
                    return string.Format("ok:{0}:{1}:{2}",
                        info.isInitialized, info.userVibranceSettingDefault, info.szGpuName ?? "");
                }
                case "get-vibrance":
                    return _proxy.GetVibranceInfo().userVibranceSettingDefault.ToString();

                case "set-vibrance" when parts.Length > 1:
                {
                    if (int.TryParse(parts[1], out int lvl))
                    { _proxy.SetVibranceWindowsLevel(lvl); return "ok"; }
                    return "ERROR:invalid_level";
                }
                case "get-gpu":
                    return _proxy.GetVibranceInfo().szGpuName ?? "unknown";

                case "ping":
                    return "pong";

                default:
                    return "ERROR:unknown_command";
            }
        }

        public void Dispose() { _running = false; }
    }
}
