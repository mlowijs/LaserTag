namespace LaserTag.Gun.Laser
{
    public class LaserPacket
    {
        public const byte LeadingByte = 0xAA;
        public const byte TrailingByte = 0x55;

        public const int LeadingByteIndex = 0;
        public const int SenderIdIndex = 1;
        public const int SequenceNumberIndex = 2;
        public const int TrailingByteIndex = 3;

        public byte SenderId { get; set; }
        public byte SequenceNumber { get; set; }

        public LaserPacket(byte senderId, byte seqNumber)
        {
            SenderId = senderId;
            SequenceNumber = seqNumber;
        }

        public LaserPacket(byte[] data)
            : this(data[SenderIdIndex], data[SequenceNumberIndex])
        {
        }


        public byte[] ToBytes()
        {
            return new byte[] { LeadingByte, 0, SenderId, SequenceNumber, TrailingByte };
        }                                 // ^ Dummy byte because of SerialPort bug
    }
}
