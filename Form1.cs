using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        SerialMonitor monitor;
        public Form1()
        {
            InitializeComponent();
            monitor = new SerialMonitor("COM12", "COM2");
        }
    }

    /// <summary>
    /// シリアル仲介
    /// </summary>
    public class SerialMonitor
    {
        SerialPort portIn;
        SerialPort portOut;
        Task mainTask;

        /// <summary>
        /// コンストラクタ処理
        /// </summary>
        /// <param name="portnameIn"></param>
        /// <param name="portnameOut"></param>
        public SerialMonitor(string portnameIn, string portnameOut)
        {
            portIn = new SerialPort()
            {
                PortName = portnameIn,
                BaudRate = 115200,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
                Handshake = Handshake.None,
            };
            portOut = new SerialPort()
            {
                PortName = portnameOut,
                BaudRate = 115200,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
                Handshake = Handshake.None,
            };
            portIn.Open();
            portOut.Open();

            mainTask = Task.Run(() =>
            {
                while (true)
                {
                    PortInMain();
                    PortOutMain();
                    Thread.Sleep(1);
                }
            });
        }

        /// <summary>
        /// 解放処理
        /// </summary>
        public void Dispose()
        {
            mainTask.Dispose();
            portIn.Close();
            portOut.Close();
        }

        public void PortInMain()
        {
            if (0 < portIn.BytesToRead)
            {
                lock (this)
                {
                    Console.WriteLine("IN->OUT");
                    while (0 < portIn.BytesToRead)
                    {
                        var dat = portIn.ReadByte();
                        Console.Write(" " + dat.ToString("X2"));
                        portOut.Write(new byte[] { (byte)dat }, 0, 1);
                    }
                    Console.WriteLine();
                }
            }
        }

        public void PortOutMain()
        {
            if (0 < portOut.BytesToRead)
            {
                lock (this)
                {
                    Console.WriteLine("IN<-OUT");
                    while (0 < portOut.BytesToRead)
                    {
                        var dat = portOut.ReadByte();
                        Console.Write(" " + dat.ToString("X2"));
                        portIn.Write(new byte[] { (byte)dat }, 0, 1);
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
