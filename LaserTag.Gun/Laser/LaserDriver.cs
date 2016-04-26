using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.IO.Ports;

namespace LaserTag.Gun.Laser
{
    public delegate void PacketReceivedEventHandler(LaserPacket laserPacket);

    public class LaserDriver
    {
        private readonly int BaudRate = 600;
        private readonly Parity Parity = Parity.Odd;
        private readonly int DataBits = 8;
        private readonly StopBits StopBits = StopBits.One;

        private SerialPort _serialPort;
        private OutputPort _redDotSight;
        private byte _sendSeqNumber = 1, _recvSeqNumber;
        private byte _recvId;

        public event PacketReceivedEventHandler PacketReceived;

        public LaserDriver(string serialPortName)
        {
            _serialPort = new SerialPort(serialPortName, BaudRate, Parity, DataBits, StopBits);
            _serialPort.DataReceived += OnDataReceived;
            _serialPort.Open();
        }


        public void SendPacket(byte senderId)
        {
            var packet = new LaserPacket(senderId, _sendSeqNumber++);
            var packetBytes = packet.ToBytes();

            //for (var i = 0; i < 3; i++)
            //{
                _serialPort.Write(packetBytes, 0, packetBytes.Length);
                _serialPort.Flush();
            //}
        }


        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var firstByte = _serialPort.ReadByte();

            Debug.Print("F = " + firstByte);

            if (firstByte != LaserPacket.LeadingByte)
            {
                _serialPort.DiscardInBuffer();
                return;
            }

            var data = new byte[4];
            var read = _serialPort.Read(data, 1, data.Length - 1);

            data[0] = (byte)firstByte;

            if (!IsPacketDataValid(data, read))
                return;

            var packet = new LaserPacket(data);

            if (!IsPacketNew(packet))
                return;

            PacketReceived?.Invoke(packet);
        }

        private bool IsPacketDataValid(byte[] data, int read)
        {
            return read == data.Length - 1
                && data[LaserPacket.TrailingByteIndex] == LaserPacket.TrailingByte;
        }

        private bool IsPacketNew(LaserPacket packet)
        {
            if (packet.SenderId == _recvId && packet.SequenceNumber == _recvSeqNumber)
                return false;

            _recvId = packet.SenderId;
            _recvSeqNumber = packet.SequenceNumber;

            return true;
        }
    }
}
