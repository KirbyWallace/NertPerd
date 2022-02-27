using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Media;


namespace NertPerd {

	class Program {

		[DllImport("user32.dll")]
		static extern int SetForegroundWindow(IntPtr point);

		static void Main(string[] args) {

			bool x = true;
			int minute = 0;
			int j = 0;

			try {
				Process[] tasklist = Process.GetProcesses();

				for (int i = 0; i < tasklist.Length; i++) {

					if (tasklist[i].ProcessName.ToLower().IndexOf("nertperd") >= 0) {

						j++;

						if (j >= 2) {
							Log.Write("Program Exit: Instance of program already exists.");
							System.Environment.Exit(1);
						}
					}
				}

				Console.WriteLine("Running... Close window to stop.\n");

				while (true) {

					Console.Write("\r" + DateTime.Now.ToString("HH:mm:ss"));

					minute = DateTime.Now.Minute;

					if (minute % 30 == 0) {

						// PING! and then begin.
						if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\ping.wav")) {
							new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "\\ping.wav").Play();
						}

						Process p = Process.Start("notepad.exe");

						// There is a slight delay between the time .start() actually starts the program,
						// and the time it responds with a process id.

						Thread.Sleep(1000);

						// Set the spawned app as active foreground application so that keystrokes that are
						// sent will be received by it, and not some other process, then send the key.

						IntPtr h = p.MainWindowHandle;
						SetForegroundWindow(h);
						SendKeys.SendWait(".");

						// Again, there is a slight delay between the time the key is sent, and the time it
						// is actually received and processed in the spawned application.  If you kill the 
						// process too soon, it will never process the key, and the computer will not become
						// un-idle (active).  And thats the whole point here for the computer to never look
						// like it is idle or inactive.

						Thread.Sleep(1000);

						p.Kill();

						while (DateTime.Now.Minute == minute) {
							Console.Write("\r" + DateTime.Now.ToString("HH:mm:ss"));
							Application.DoEvents();
						}
					}
				}

			}

			catch (Exception e) {
				Log.Write(e);
			}

		}
	}

	public static class Log {

		public static void Write(Exception ex) {
			StreamWriter sw = null;
			try {
				sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
				sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Source);
				sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Message);
				sw.WriteLine(DateTime.Now.ToString() + ": " + ex.StackTrace);
				sw.Flush();
				sw.Close();

				Console.WriteLine(DateTime.Now.ToString() + ": " + ex.Source);
				Console.WriteLine(DateTime.Now.ToString() + ": " + ex.Message);
				Console.WriteLine(DateTime.Now.ToString() + ": " + ex.StackTrace);

			}
			catch {

			}
		}

		public static void Write(string Message) {
			StreamWriter sw = null;
			try {

				sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
				sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
				sw.Flush();
				sw.Close();

				Console.WriteLine(DateTime.Now.ToString() + ": " + Message);

			}
			catch {

			}
		}
	}

}
