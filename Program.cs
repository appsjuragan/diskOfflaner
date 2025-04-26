// File: Program.cs

using System;
using System.Management;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "Disk Offlaner";

        if (args.Length > 0)
        {
            if (int.TryParse(args[0], out int diskNumber))
            {
                ToggleDiskOnlineOffline(diskNumber);
                return;
            }
        }

        RunInteractive();
    }

    static void RunInteractive()
    {
        while (true)
        {
            Console.Clear();
            ListDisksAndPartitions();

            Console.Write("\nSelect Disk Number to toggle ONLINE/OFFLINE (or 'q' to quit): ");
            string input = Console.ReadLine();

            if (input.Trim().ToLower() == "q")
                break;

            if (int.TryParse(input, out int diskNumber))
            {
                ToggleDiskOnlineOffline(diskNumber);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid input!");
                Console.ResetColor();
            }

            Console.WriteLine("Press any key to refresh...");
            Console.ReadKey();
        }
    }

    static void ListDisksAndPartitions()
    {
        try
        {
            var scope = new ManagementScope("\\\\.\\root\\Microsoft\\Windows\\Storage");
            var query = new ObjectQuery("SELECT * FROM MSFT_Disk");

            using (var searcher = new ManagementObjectSearcher(scope, query))
            {
                Console.WriteLine("Available Drives:\n");

                foreach (ManagementObject disk in searcher.Get())
                {
                    int number = Convert.ToInt32(disk["Number"]);
                    string model = (disk["FriendlyName"] ?? "Unknown").ToString();
                    bool isOffline = (bool)(disk["IsOffline"] ?? false);
                    string healthStatus = (disk["HealthStatus"] ?? "Unknown").ToString();
                    ulong sizeBytes = (ulong)(disk["Size"] ?? 0UL);
                    double sizeGB = Math.Round(sizeBytes / 1073741824.0, 2);

                    Console.ForegroundColor = isOffline ? ConsoleColor.DarkGray : ConsoleColor.Cyan;
                    Console.WriteLine($"Disk {number}: {model} - {(isOffline ? "Offline" : "Online")} - Health: {healthStatus} - Size: {sizeGB} GB");
                    Console.ResetColor();

                    // List partitions
                    ListPartitions(number);
                }
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error listing disks: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ListPartitions(int diskNumber)
    {
        try
        {
            var scope = new ManagementScope("\\\\.\\root\\Microsoft\\Windows\\Storage");
            var query = new ObjectQuery($"SELECT * FROM MSFT_Partition WHERE DiskNumber = {diskNumber}");

            using (var searcher = new ManagementObjectSearcher(scope, query))
            {
                var partitions = searcher.Get();

                if (partitions.Count == 0)
                {
                    Console.WriteLine(" └─ (No partitions)");
                    return;
                }

                foreach (ManagementObject partition in partitions)
                {
                    ulong sizeBytes = (ulong)(partition["Size"] ?? 0UL);
                    double sizeGB = Math.Round(sizeBytes / 1073741824.0, 2);
                    int partitionNumber = Convert.ToInt32(partition["PartitionNumber"]);
                    string gptType = (partition["GptType"] ?? "").ToString();

                    string label = "_"; // Default if no drive letter

                    var driveLetter = partition["DriveLetter"]?.ToString();
                    if (!string.IsNullOrEmpty(driveLetter))
                        label = driveLetter.TrimEnd(':');

                    Console.WriteLine($" └─ Partition {partitionNumber}: {sizeGB} GB (GPT: {gptType}) [{label}:]");
                }
            }
        }
        catch
        {
            Console.WriteLine(" └─ (Unable to retrieve partition information)");
        }
    }

    static bool ToggleDiskOnlineOffline(int diskNumber)
    {
        try
        {
            var scope = new ManagementScope("\\\\.\\root\\Microsoft\\Windows\\Storage");
            var query = new ObjectQuery($"SELECT * FROM MSFT_Disk WHERE Number = {diskNumber}");

            using (var searcher = new ManagementObjectSearcher(scope, query))
            {
                foreach (ManagementObject disk in searcher.Get())
                {
                    bool isOffline = (bool)(disk["IsOffline"] ?? false);
                    bool isSystem = (bool)(disk["IsSystem"] ?? false);
                    bool isBoot = (bool)(disk["IsBoot"] ?? false);

                    string devicePath = disk.Path.Path;

                    using (var diskObj = new ManagementObject(devicePath))
                    {
                        if (isOffline)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Disk {diskNumber} is currently Offline. Bringing it Online...");
                            Console.ResetColor();

                            diskObj.InvokeMethod("Online", null);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Disk {diskNumber} is now Online.");
                            Console.ResetColor();
                        }
                        else
                        {
                            if (isSystem || isBoot)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("WARNING: This disk contains System or Boot partitions!");
                                Console.Write("Are you sure you want to OFFLINE it? (y/N): ");
                                Console.ResetColor();
                                var confirm = Console.ReadLine();
                                if (confirm?.ToLower() != "y")
                                {
                                    Console.WriteLine("Cancelled by user.");
                                    return false;
                                }
                            }

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Disk {diskNumber} is currently Online. Bringing it Offline...");
                            Console.ResetColor();

                            diskObj.InvokeMethod("Offline", null);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Disk {diskNumber} is now Offline.");
                            Console.ResetColor();
                        }
                    }

                    return true;
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Disk not found.");
            Console.ResetColor();
            return false;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine($"Failed to change disk state: {ex.Message}");
            Console.ResetColor();
            return false;
        }
    }
}