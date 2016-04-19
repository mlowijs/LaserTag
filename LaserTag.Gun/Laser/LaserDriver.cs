using Microsoft.SPOT.Hardware;
using System.IO.Ports;

namespace LaserTag.Gun.Laser
{
    public delegate void PacketReceivedEventHandler(LaserPacket laserPacket);

    public class LaserDriver
    {
        private readonly int BaudRate = 2400;
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

            for (var i = 0; i < 3; i++)
            {
                _serialPort.Write(packetBytes, 0, packetBytes.Length);
                _serialPort.Flush();
            }
        }


        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var data = new byte[4];
            var read = _serialPort.Read(data, 0, data.Length);

            _serialPort.DiscardInBuffer();

            if (!IsPacketDataValid(data, read))
                return;

            var packet = new LaserPacket(data);

            if (!IsPacketNew(packet))
                return;

            if (PacketReceived != null)
                PacketReceived(packet);
        }

        private bool IsPacketDataValid(byte[] data, int read)
        {
            return data[LaserPacket.LeadingByteIndex] == LaserPacket.LeadingByte
                && read == data.Length
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
