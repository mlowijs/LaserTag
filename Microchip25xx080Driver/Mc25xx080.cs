using Microsoft.SPOT.Hardware;
using System;

namespace Microchip25xx080Driver
{
    public class Mc25xx080
    {
        private SPI _spi;
        private OutputPort _hold;
        private readonly int _pageSize;

        public Mc25xx080(int pageSize, Cpu.Pin csPin, Cpu.Pin holdPin, SPI.SPI_module spiModule)
        {
            _pageSize = pageSize;

            _spi = new SPI(new SPI.Configuration(csPin, false, 100, 200, false, true, 5000, spiModule));
            _hold = new OutputPort(holdPin, true);
        }


        public byte[] Read(short startAddress, int length)
        {
            var readBuffer = SpiTransfer(Instruction.Read, startAddress, new byte[length]);

            var data = new byte[length];
            Array.Copy(readBuffer, 3, data, 0, length);

            return data;
        }

        public byte Read(short address)
        {
            return Read(address, 1)[0];
        }

        public void Write(short startAddress, params byte[] data)
        {
            if (data.Length > _pageSize - (startAddress % _pageSize))
                return;

            _spi.Write(new byte[] { (byte)Instruction.WriteEnable });
            SpiTransfer(Instruction.Write, startAddress, data);
        }


        private byte[] SpiTransfer(Instruction instruction, short address, params byte[] data)
        {
            var writeBuffer = new byte[data.Length + 3];
            writeBuffer[0] = (byte)instruction;
            writeBuffer[1] = (byte)(address >> 8);
            writeBuffer[2] = (byte)address;
            Array.Copy(data, 0, writeBuffer, 3, data.Length);

            var readBuffer = new byte[writeBuffer.Length];
            _spi.WriteRead(writeBuffer, readBuffer);

            return readBuffer;
        }
    }
}
