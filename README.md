# DiskOfflaner

DiskOfflaner is a simple console application for Windows that allows you to list all physical disks and partitions, and toggle their Online/Offline state.

## Features

- List all available disks with size, health, and online/offline status.
- Show partitions under each disk in a tree-like view.
- Mark partitions without drive letters as `[_:]`.
- Warn before changing the state of critical system disks.
- Colorized console output for easier reading.
- Supports interactive mode and command-line arguments.

## Requirements

- Windows 10, Windows 11, or Windows Server.
- .NET 6.0 SDK or later.

## Build Instructions

Open a terminal inside the project folder and run:

```

dotnet build -c Release

``` 
The output executable will be located at:
```

bin\Release\netX.0\diskOfflaner.exe

```

## Usage
Interactive Mode:

```

diskOfflaner.exe

```

Output:

```
Disk 1: ST10000VN0004-XXX - Online - Health: 0 - Size: 9314 GB
 └─ Partition 1: 6205.44 GB (GPT: {GUID_ID}) [E:]
 └─ Partition 2: 3108.56 GB (GPT: {GUID_ID}) [F:]
Disk 2: HGST HUH72-XXX - Online - Health: 0 - Size: 11176 GB
 └─ Partition 1: 5951.84 GB (GPT: {GUID_ID}) [Q:]
 └─ Partition 2: 5224.15 GB (GPT: {GUID_ID}) [R:]
Disk 3: NVMe Samsung SSD 970 - Online - Health: 0 - Size: 465.76 GB
 └─ Partition 1: 0.1 GB (GPT: {GUID_ID}) [:]
 └─ Partition 2: 0.02 GB (GPT: {GUID_ID}) [:]
 └─ Partition 3: 464.92 GB (GPT: {GUID_ID}) [C:]
 └─ Partition 4: 0.73 GB (GPT: {GUID_ID}) [:]
Disk 4: Seagate Expansion Desk - Online - Health: 0 - Size: 3726.02 GB
 └─ Partition 1: 3726.02 GB (GPT: {GUID_ID}) [P:]
Disk 0: WDC WD100PURZ-XXX - Online - Health: 0 - Size: 9314 GB
 └─ Partition 1: 5273.63 GB (GPT: {GUID_ID}) [G:]
 └─ Partition 2: 4040.37 GB (GPT: {GUID_ID}) [H:]
```

Direct Command Mode (toggle Disk 2 for example):
```

diskOfflaner.exe 2

```
Output:
```

C:\>diskOfflaner.exe 2
Disk 2 is currently Online. Bringing it Offline...

```

## Notes
Use with caution. Taking a critical or system disk offline can cause Windows to become unstable.

Always confirm when the program warns about critical disks.

Requires administrative privileges.
