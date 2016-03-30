using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Nrf8001Driver.Commands;
using Nrf8001Driver.Events;
using Nrf8001Driver.Extensions;

namespace Nrf8001Driver
{
    public delegate void AciEventReceivedEventHandler(AciEvent aciEvent);
    public delegate void DataReceivedEventHandler(DataReceivedEvent dataReceivedEvent);

    public class Nrf8001
    {
        private static readonly byte[] ReadEventLengthBuffer = new byte[2];

        private OutputPort _req, _rst;
        private InterruptPort _rdy;
        private SPI _spi;
        private Queue _eventQueue;

        private byte[][] _setupData;
        private int _setupIndex = 0;

        public event AciEventReceivedEventHandler AciEventReceived;
        public event DataReceivedEventHandler DataReceived;

        public bool Bonded { get; protected set; }
        /// <summary>
        /// Gets the device state of the nRF8001.
        /// </summary>
        /// <remarks>Section 22</remarks>
        public Nrf8001State State { get; protected set; }

        /// <summary>
        /// Gets the amount of data credits available.
        /// </summary>
        /// <remarks>Section 21.2</remarks>
        public byte DataCreditsAvailable { get; protected set; }
        /// <summary>
        /// Gets the bitmap containing all open pipes.
        /// </summary>
        /// <remarks>Section 26.8</remarks>
        protected ulong OpenPipesBitmap { get; set; }
        /// <summary>
        /// Gets the bitmap containing all closed pipes that need opening.
        /// </summary>
        /// <remarks>Section 26.8</remarks>
        protected ulong ClosedPipesBitmap { get; set; }

        /// <summary>
        /// Creates a new nRF8001 device interface.
        /// </summary>
        /// <param name="rstPin">The application controller pin that the nRF8001's RST pin is connected to.</param>
        /// <param name="reqPin">The application controller pin that the nRF8001's REQn pin is connected to.</param>
        /// <param name="rdyPin">The application controller pin that the nRF8001's RDYn pin is connected to.</param>
        /// <param name="spiModule">The SPI module to use for communication with the nRF8001.</param>
        public Nrf8001(Cpu.Pin rstPin, Cpu.Pin reqPin, Cpu.Pin rdyPin, SPI.SPI_module spiModule)
        {
            _rst = new OutputPort(rstPin, false);
            _req = new OutputPort(reqPin, true);
            _rdy = new InterruptPort(rdyPin, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            _spi = new SPI(new SPI.Configuration(Cpu.Pin.GPIO_NONE, false, 0, 0, false, true, 100, spiModule));

            State = Nrf8001State.Unknown;
            Reset();

            _rdy.OnInterrupt += OnRdyInterrupt;
        }

        /// <summary>
        /// Resets the nRF8001 by toggling its RST pin.
        /// </summary>
        public void Reset()
        {           
            _rst.Write(false);

            Bonded = false;
            State = Nrf8001State.Resetting;
            _eventQueue = new Queue();

            Thread.Sleep(50);

            _rst.Write(true);
        }

        /// <summary>
        /// Handles the first event in the nRF8001's event queue. Call this method as often as possible from the application controller.
        /// </summary>
        /// <returns>The first event in the event queue.</returns>
        public void ProcessEvents()
        {
            if (_eventQueue.Count == 0)
                return;

            ProcessEvent((byte[])_eventQueue.Dequeue());
        }

        /// <summary>
        /// Sets up the nRF8001 with the setup data from a services.h file generated by nRFGo Studio.
        /// </summary>
        /// <param name="setupData">The setup data.</param>
        public void Setup(byte[][] setupData)
        {
            _setupData = setupData;
            _setupIndex = 0;

            while (State != Nrf8001State.Setup)
                ProcessEvents();

            Setup(_setupData[_setupIndex++]);

            while (State != Nrf8001State.Standby)
                ProcessEvents();
        }

        /// <summary>
        /// Starts advertising and establishes a connection to a peer device.
        /// </summary>
        /// <remarks>Section 24.14</remarks>
        /// <param name="timeout">Advertisement timeout, in seconds.</param>
        /// <param name="interval">Advertisement interval, in periods of 0.625 milliseconds.</param>
        public void AwaitConnection(ushort timeout, ushort interval)
        {
            Connect(timeout, interval);

            while (true)
            {
                ProcessEvents();

                if (State == Nrf8001State.Connected)
                    return;
                else if (State == Nrf8001State.Standby)
                    Connect(timeout, interval);
            }
        }

        /// <summary>
        /// Starts advertising to setup a trusted relationship with a peer device.
        /// </summary>
        /// <param name="timeout">Advertisement timeout, in seconds.</param>
        /// <param name="interval">Advertisement interval, in periods of 0.625 milliseconds.</param>
        public void AwaitBond(ushort timeout, ushort interval)
        {
            Bond(timeout, interval);

            while (true)
            {
                ProcessEvents();

                if (Bonded)
                    return;
                else if (State == Nrf8001State.Standby)
                    Bond(timeout, interval);
            }
        }

        /// <summary>
        /// Checks whether the specified service pipe is open for data transfer.
        /// </summary>
        /// <param name="servicePipeId">The ID of the service pipe.</param>
        /// <returns>True if the service pipe is open, otherwise false.</returns>
        public bool IsServicePipeOpen(byte servicePipeId)
        {
            return (OpenPipesBitmap >> servicePipeId & 0x01) != 0;
        }

        #region ACI Commands
        /// <summary>
        /// Activates Sleep mode.
        /// </summary>
        /// <remarks>Section 24.4</remarks>
        public void Sleep()
        {
            if (State != Nrf8001State.Standby)
                throw new InvalidOperationException("nRF8001 is not in Standby mode.");

            AciSend(AciOpCode.Sleep);

            // Sleep does not return a CommandResponse event.
            State = Nrf8001State.Sleep;
        }

        /// <summary>
        /// Wakes up from Sleep mode.
        /// </summary>
        /// <remarks>Section 24.5</remarks>
        public void Wakeup()
        {
            if (State != Nrf8001State.Sleep)
                throw new InvalidOperationException("nRF8001 is not in Sleep mode.");

            AciSend(AciOpCode.Wakeup);
        }

        private void Setup(byte[] data)
        {
            AciSend(AciOpCode.Setup, data);
        }

        private void Connect(ushort timeout, ushort interval)
        {
            if (State != Nrf8001State.Standby)
                throw new InvalidOperationException("nRF8001 is not in Standby mode.");

            if (timeout < 0x0001 || timeout > 0x3FFF)
                throw new ArgumentOutOfRangeException("timeout", "Timeout must be between 0x0001 and 0x3FFF, inclusive.");

            if (interval < 0x0020 || interval > 0x4000)
                throw new ArgumentOutOfRangeException("interval", "Interval must be between 0x0020 and 0x4000, inclusive.");

            AciSend(AciOpCode.Connect, (byte)(timeout), (byte)(timeout >> 8), // Timeout
                                       (byte)(interval), (byte)(interval >> 8)); // Interval

            State = Nrf8001State.Connecting;
        }
        
        private void Bond(ushort timeout, ushort interval)
        {
            if (State != Nrf8001State.Standby)
                throw new InvalidOperationException("nRF8001 is not in Standby mode.");

            if (timeout < 0x0001 || timeout > 0x00B4)
                throw new ArgumentOutOfRangeException("timeout", "Timeout must be between 0x0001 and 0x00B4, inclusive.");

            if (interval < 0x0020 || interval > 0x4000)
                throw new ArgumentOutOfRangeException("interval", "Interval must be between 0x0020 and 0x4000, inclusive.");

            AciSend(AciOpCode.Bond, (byte)(timeout), (byte)(timeout >> 8), // Timeout
                                    (byte)(interval), (byte)(interval >> 8)); // Interval

            State = Nrf8001State.Bonding;
        }

        public void OpenRemotePipe(byte pipeId)
        {
            if (State != Nrf8001State.Standby)
                throw new InvalidOperationException("nRF8001 is not in Standby mode.");

            if (pipeId < 1 || pipeId > 62)
                throw new ArgumentOutOfRangeException("timeout", "Timeout must be between 1 and 62, inclusive.");

            AciSend(AciOpCode.OpenRemotePipe, pipeId);
        }

        /// <summary>
        /// Sends data to a peer device through a transmit service pipe.
        /// </summary>
        /// <remarks>Section 25.2</remarks>
        /// <param name="servicePipeId">The ID of the service pipe to send data through.</param>
        /// <param name="data">The data to send.</param>
        public void SendData(byte servicePipeId, params byte[] data)
        {
            if (servicePipeId < 1 || servicePipeId > 62)
                throw new ArgumentOutOfRangeException("pipe", "Service pipe ID must be between 1 and 62, inclusive.");

            if (data.Length < 1 || data.Length > 20)
                throw new ArgumentOutOfRangeException("data", "Data length must be between 1 and 20, inclusive.");

            if (State != Nrf8001State.Connected)
                throw new InvalidOperationException("nRF8001 is not connected.");

            if (DataCreditsAvailable < 1)
                throw new InvalidOperationException("There are no data credits available.");

            if (!IsServicePipeOpen(servicePipeId))
                throw new InvalidOperationException("Service pipe is not open.");

            AciSend(AciOpCode.SendData, servicePipeId, data);
        }
        #endregion

        #region ACI Interface
        protected void AciSend(AciOpCode opCode, params byte[] data)
        {
            if (data.Length > 30)
                throw new ArgumentOutOfRangeException("data", "The maximum amount of data bytes is 30.");

            // Create ACI packet
            var packet = new byte[data.Length + 2];
            packet[0] = (byte)(data.Length + 1);
            packet[1] = (byte)opCode;
            Array.Copy(data, 0, packet, 2, data.Length);

            // Request transfer
            _rdy.DisableInterrupt();
            _req.Write(false);

            // Wait for RDY to go low
            while (_rdy.Read()) ;

            _spi.WriteLsb(packet);

            _req.Write(true);

            // Wait for RDY to go high
            while (!_rdy.Read()) ;
            _rdy.EnableInterrupt();
        }

        protected void AciSend(AciOpCode opCode, byte arg0, params byte[] data)
        {
            var buffer = new byte[data.Length + 1];

            buffer[0] = arg0;
            Array.Copy(data, 0, buffer, 1, data.Length);

            AciSend(opCode, buffer);
        }

        protected byte[] AciReceive()
        {
            // Create a new read buffer
            var readBuffer = new byte[2];

            // Start SPI communication
            _req.Write(false);

            _spi.WriteReadLsb(ReadEventLengthBuffer, readBuffer);

            // Check event packet length
            if (readBuffer[1] > 0 && readBuffer[1] <= 30)
            {
                readBuffer = new byte[readBuffer[1]];
                _spi.WriteReadLsb(new byte[readBuffer.Length], readBuffer);
            }
            else
            {
                readBuffer = null;
            }

            // SPI communication done
            _req.Write(true);

            return readBuffer;
        }
        #endregion

        #region ACI Events
        private void OnRdyInterrupt(uint data1, uint data2, DateTime time)
        {
            var content = AciReceive();

            if (content != null)
                _eventQueue.Enqueue(content);
        }

        private void ProcessEvent(byte[] content)
        {
            var eventType = (AciEventType)content[0];            
            AciEvent aciEvent = null;

            switch (eventType)
            {
                case AciEventType.BondStatus:
                    aciEvent = new BondStatusEvent(content);
                    HandleBondStatusEvent((BondStatusEvent)aciEvent);
                    break;

                case AciEventType.CommandResponse:
                    aciEvent = new CommandResponseEvent(content);
                    HandleCommandResponseEvent((CommandResponseEvent)aciEvent);
                    break;

                case AciEventType.Connected:
                    aciEvent = new AciEvent(content);
                    State = Nrf8001State.Connected;
                    break;

                case AciEventType.DataCredit:
                    aciEvent = new DataCreditEvent(content);
                    HandleDataCreditEvent((DataCreditEvent)aciEvent);
                    break;

                case AciEventType.DataReceived:
                    aciEvent = new DataReceivedEvent(content);
                    HandleDataReceivedEvent((DataReceivedEvent)aciEvent);
                    break;

                case AciEventType.DeviceStarted:
                    aciEvent = new DeviceStartedEvent(content);
                    HandleDeviceStartedEvent((DeviceStartedEvent)aciEvent);
                    break;

                case AciEventType.Disconnected:
                    aciEvent = new AciEvent(content);
                    State = Nrf8001State.Standby;
                    break;

                case AciEventType.PipeStatus:
                    aciEvent = new AciEvent(content);
                    OpenPipesBitmap = aciEvent.Content.ToUnsignedLong(1);
                    ClosedPipesBitmap = aciEvent.Content.ToUnsignedLong(9);
                    break;                

                default:
                    aciEvent = new AciEvent(content);
                    break;
            }

            Debug.Print("Event: " + eventType.GetName());

            if (AciEventReceived != null)
                AciEventReceived(aciEvent);
        }

        private void HandleCommandResponseEvent(CommandResponseEvent aciEvent)
        {
            if (aciEvent.Command == AciOpCode.Setup)
            {
                if (aciEvent.StatusCode == AciStatusCode.TransactionContinue)
                    Setup(_setupData[_setupIndex++]);
                else if (aciEvent.StatusCode != AciStatusCode.TransactionComplete)
                    throw new Nrf8001Exception("Setup data invalid.");
            }
        }

        private void HandleBondStatusEvent(BondStatusEvent aciEvent)
        {
            if (aciEvent.StatusCode == BondStatusCode.Success)
                Bonded = true;
        }

        private void HandleDataCreditEvent(DataCreditEvent aciEvent)
        {
            DataCreditsAvailable = aciEvent.DataCreditsAvailable;
        }

        private void HandleDataReceivedEvent(DataReceivedEvent aciEvent)
        {
            if (DataReceived != null)
                DataReceived(aciEvent);
        }

        private void HandleDeviceStartedEvent(DeviceStartedEvent aciEvent)
        {
            State = aciEvent.State;
            DataCreditsAvailable = aciEvent.DataCreditsAvailable;
        }
        #endregion
    }
}
