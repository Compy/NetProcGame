﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO;

namespace NetProcGame.game
{
    public class UIProcessListener
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern SafeFileHandle CreateNamedPipe(
           String pipeName,
           uint dwOpenMode,
           uint dwPipeMode,
           uint nMaxInstances,
           uint nOutBufferSize,
           uint nInBufferSize,
           uint nDefaultTimeOut,
           IntPtr lpSecurityAttributes);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int ConnectNamedPipe(
           SafeFileHandle hNamedPipe,
           IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DisconnectNamedPipe(SafeFileHandle hHandle);

        [StructLayoutAttribute(LayoutKind.Sequential)]
        struct SECURITY_DESCRIPTOR
        {
            public byte revision;
            public byte size;
            public short control;
            public IntPtr owner;
            public IntPtr group;
            public IntPtr sacl;
            public IntPtr dacl;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        private const uint SECURITY_DESCRIPTOR_REVISION = 1;

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool InitializeSecurityDescriptor(ref SECURITY_DESCRIPTOR sd, uint dwRevision);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool SetSecurityDescriptorDacl(ref SECURITY_DESCRIPTOR sd, bool daclPresent, IntPtr dacl, bool daclDefaulted);

        public class Client
        {
            public SafeFileHandle handle;
            public FileStream stream;
        }

        /// <summary>
        /// Handles messages received from a client pipe
        /// </summary>
        /// <param name="message">The byte message received</param>
        public delegate void MessageReceivedHandler(byte[] message);

        /// <summary>
        /// Event is called whenever a message is received from a client pipe
        /// </summary>
        public event MessageReceivedHandler MessageReceived;


        /// <summary>
        /// Handles client disconnected messages
        /// </summary>
        public delegate void ClientDisconnectedHandler();

        /// <summary>
        /// Event is called when a client pipe is severed.
        /// </summary>
        public event ClientDisconnectedHandler ClientDisconnected;

        const int BUFFER_SIZE = 4096;

        Thread listenThread;
        //readonly List<Client> clients = new List<Client>();
        Client myClient;
        object syncObject = new object();

        /// <summary>
        /// The total number of PipeClients connected to this server
        /// </summary>
        public int TotalConnectedClients
        {
            get
            {
                return (myClient == null) ? 0 : 1;
            }
        }

        /// <summary>
        /// The name of the pipe this server is connected to
        /// </summary>
        public string PipeName { get; private set; }

        /// <summary>
        /// Is the server currently running
        /// </summary>
        public bool Running { get; private set; }

        /// <summary>
        /// Starts the pipe server on a particular name.
        /// </summary>
        /// <param name="pipename">The name of the pipe.</param>
        public void Start(string pipename)
        {
            PipeName = pipename;

            //start the listening thread
            listenThread = new Thread(ListenForClients)
            {
                IsBackground = true
            };

            listenThread.Start();

            Running = true;
        }

        void ListenForClients()
        {
            SECURITY_DESCRIPTOR sd = new SECURITY_DESCRIPTOR();

            // set the Security Descriptor to be completely permissive
            InitializeSecurityDescriptor(ref sd, SECURITY_DESCRIPTOR_REVISION);
            SetSecurityDescriptorDacl(ref sd, true, IntPtr.Zero, false);

            IntPtr ptrSD = Marshal.AllocCoTaskMem(Marshal.SizeOf(sd));
            Marshal.StructureToPtr(sd, ptrSD, false);

            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES
            {
                nLength = Marshal.SizeOf(sd),
                lpSecurityDescriptor = ptrSD,
                bInheritHandle = 1
            };

            IntPtr ptrSA = Marshal.AllocCoTaskMem(Marshal.SizeOf(sa));
            Marshal.StructureToPtr(sa, ptrSA, false);


            while (true)
            {
                // Creates an instance of a named pipe for one client
                SafeFileHandle clientHandle =
                    CreateNamedPipe(
                        PipeName,

                        // DUPLEX | FILE_FLAG_OVERLAPPED = 0x00000003 | 0x40000000;
                        0x40000003,
                        0,
                        255,
                        BUFFER_SIZE,
                        BUFFER_SIZE,
                        0,
                        ptrSA);

                //could not create named pipe instance
                if (clientHandle.IsInvalid)
                    continue;

                int success = ConnectNamedPipe(clientHandle, IntPtr.Zero);

                //could not connect client
                if (success == 0)
                {
                    // close the handle, and wait for the next client
                    clientHandle.Close();
                    continue;
                }

                Client client = new Client
                {
                    handle = clientHandle
                };

                myClient = client;

                Thread readThread = new Thread(Read)
                {
                    IsBackground = true
                };
                readThread.Start(client);
            }

            // free up the ptrs (never reached due to infinite loop)
            Marshal.FreeCoTaskMem(ptrSD);
            Marshal.FreeCoTaskMem(ptrSA);
        }

        void Read(object clientObj)
        {
            Client client = (Client)clientObj;
            client.stream = new FileStream(client.handle, FileAccess.ReadWrite, BUFFER_SIZE, true);
            byte[] buffer = new byte[BUFFER_SIZE];

            while (true)
            {
                int bytesRead = 0;

                using (MemoryStream ms = new MemoryStream())
                {
                    try
                    {
                        // read the total stream length
                        int totalSize = client.stream.Read(buffer, 0, 4);

                        // client has disconnected
                        if (totalSize == 0)
                            break;

                        totalSize = BitConverter.ToInt32(buffer, 0);

                        do
                        {
                            int numBytes = client.stream.Read(buffer, 0, Math.Min(totalSize - bytesRead, BUFFER_SIZE));

                            ms.Write(buffer, 0, numBytes);

                            bytesRead += numBytes;

                        } while (bytesRead < totalSize);

                    }
                    catch
                    {
                        //read error has occurred
                        break;
                    }

                    //client has disconnected
                    if (bytesRead == 0)
                        break;

                    //fire message received event
                    if (MessageReceived != null)
                        MessageReceived(ms.ToArray());
                }
            }
            lock (syncObject)
            {
                //clean up resources
                DisconnectNamedPipe(client.handle);
                client.stream.Close();
                client.handle.Close();

                myClient = null;
            }

            // invoke the event, a client disconnected
            if (ClientDisconnected != null)
                ClientDisconnected();
        }

        /// <summary>
        /// Sends a message to all connected clients.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void SendMessage(byte[] message)
        {
            if (myClient == null) return;
            lock (syncObject)
            {
                //get the entire stream length
                byte[] messageLength = BitConverter.GetBytes(message.Length);
                // length
                myClient.stream.Write(messageLength, 0, 4);

                // data
                myClient.stream.Write(message, 0, message.Length);
                myClient.stream.Flush();
            }
        }
    }
}
