using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

class SystemIdentifier
{
	public string CPUIdentifier { get; private set; }
	public string DiskSerialNumber { get; private set; }
	public string BIOSIdentifier { get; private set; }

	public SystemIdentifier()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			CPUIdentifier = GetWindowsCPUIdentifier();
			DiskSerialNumber = GetWindowsDiskSerialNumber();
			BIOSIdentifier = GetWindowsBIOSIdentifier();
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			CPUIdentifier = GetLinuxCPUIdentifier();
			DiskSerialNumber = GetLinuxDiskSerialNumber();
			BIOSIdentifier = GetLinuxBIOSIdentifier();
		}
		else
		{
			throw new PlatformNotSupportedException("Unsupported OS.");
		}
	}

	private string GetWindowsCPUIdentifier()
	{
		// Windows-specific code to get CPU identifier
		ManagementObjectSearcher cpuSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
		ManagementObjectCollection cpuInfo = cpuSearcher.Get();

		foreach (ManagementObject obj in cpuInfo)
		{
			return $"{obj["Name"]}_{obj["NumberOfCores"]}_{obj["NumberOfLogicalProcessors"]}";
		}

		return "";
	}

	private string GetWindowsDiskSerialNumber()
	{
		// Windows-specific code to get disk serial number
		ManagementObjectSearcher diskSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
		ManagementObjectCollection diskInfo = diskSearcher.Get();

		foreach (ManagementObject obj in diskInfo)
		{
			return obj["SerialNumber"].ToString();
		}

		return "";
	}

	private string GetWindowsBIOSIdentifier()
	{
		// Windows-specific code to get BIOS identifier
		ManagementObjectSearcher biosSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
		ManagementObjectCollection biosInfo = biosSearcher.Get();

		foreach (ManagementObject obj in biosInfo)
		{
			return obj["SerialNumber"].ToString();
		}

		return "";
	}

	private string GetLinuxCPUIdentifier()
	{
		// Linux-specific code to get CPU identifier
		// Execute shell commands and return the CPU name
		string commandOutput = ExecuteShellCommand("sudo lscpu");

		// Split the command output into lines
		string[] lines = commandOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

		foreach (string line in lines)
		{
			if (line.StartsWith("Model name:"))
			{
				string[] parts = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length == 2)
				{
					return parts[1].Trim();
				}
			}
		}

		return "";
	}

	private string GetLinuxDiskSerialNumber()
	{
		// Linux-specific code to get disk serial number
		// Example: Execute shell commands, e.g., "udevadm info --query=all --name=/dev/sda | grep SERIAL"
		string commandOutput = ExecuteShellCommand("sudo udevadm info --query=all --name=/dev/sda | grep ID_SERIAL");

		return commandOutput;
	}

	private string GetLinuxBIOSIdentifier()
	{
		// Linux-specific code to get BIOS identifier
		// Example: Execute shell commands, e.g., "dmidecode -s system-uuid"
		string commandOutput = ExecuteShellCommand("sudo dmidecode -s system-uuid");
		return commandOutput;
	}

	private string ExecuteShellCommand(string command)
	{
		Process process = new Process()
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "bash",
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true,
				Arguments = $"-c \"{command}\""
			}
		};

		process.Start();
		string output = process.StandardOutput.ReadToEnd();
		process.WaitForExit();
		return output.Trim();
	}

}

class Program
{
	static void Main(string[] args)
	{
		try
		{
			SystemIdentifier systemIdentifier = new SystemIdentifier();

			Console.WriteLine("CPU Identifier: " + systemIdentifier.CPUIdentifier);
			Console.WriteLine("Disk Serial Number: " + systemIdentifier.DiskSerialNumber);
			Console.WriteLine("BIOS Identifier: " + systemIdentifier.BIOSIdentifier);
		}
		catch (PlatformNotSupportedException ex)
		{
			Console.WriteLine("Error: " + ex.Message);
		}
	}
}
