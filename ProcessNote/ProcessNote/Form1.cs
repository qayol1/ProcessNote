﻿using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ProcessNote
{
    public partial class Form1 : Form
    {

        PerformanceCounter cpuCounter;
        Timer bigT,smallT;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowProcesses();
            ShowCpuUsage();
            ShowMemoryUsage();

            bigT = new Timer();
            bigT.Interval = 10000;
            bigT.Tick += new EventHandler(bigT_Tick);
            bigT.Start();

            smallT = new Timer();
            smallT.Interval = 100;
            smallT.Tick += new EventHandler(smallT_Tick);
            smallT.Start();
            
        }

        void bigT_Tick(object sender, EventArgs e)
        {
            ShowProcesses();
        }

        void smallT_Tick(object sender, EventArgs e)
        {
            ShowCpuUsage();
            ShowMemoryUsage();
        }

        void ShowProcesses()
        {
            listBox1.Items.Clear();
            Process[] localAll = Process.GetProcesses();
            foreach (Process element in localAll)
            {
                listBox1.Items.Add("ID: "+element.Id+" | Name: "+element.ProcessName);
            }
        }

        void ShowCpuUsage()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            System.Threading.Thread.Sleep(100);
            label1.Text = String.Format("{0:0.00}", cpuCounter.NextValue()) + "%";
        }

        void ShowMemoryUsage()
        {
            Int64 phav = PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
            Int64 tot = PerformanceInfo.GetTotalMemoryInMiB();
            decimal percentFree = ((decimal)phav / (decimal)tot) * 100;
            decimal percentOccupied = 100 - percentFree;
            Console.WriteLine("Available Physical Memory (MiB) " + phav.ToString());
            Console.WriteLine("Total Memory (MiB) " + tot.ToString());
            Console.WriteLine("Free (%) " + percentFree.ToString());
            Console.WriteLine("Occupied (%) " + percentOccupied.ToString());
            label4.Text = String.Format("{0:0.00}", percentOccupied) + "%";
        }
    }

    public static class PerformanceInfo
    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static Int64 GetPhysicalAvailableMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }

        public static Int64 GetTotalMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }
    }
}
