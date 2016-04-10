using Microsoft.SPOT.Hardware;
using NetmfHelpers;
using System;

namespace Microchip25xx080Driver
{
    public class Mc25xx080
    {
        private readonly ushort _pageSize;

        private MultiSyncedSPI _spi;

        public Mc25xx080(ushort pageSize, Cpu.Pin csPin, SPI.SPI_module spiModule)
        {
            _pageSize = pageSize;

            _spi = new MultiSyncedSPI(new SPI.Configuration(csPin, false, 100, 200, false, true, 5000, spiModule));
        }


        public byte[] Read(ushort startAddress, int length)
        {
            var readBuffer = SpiTransfer(Instruction.Read, startAddress, new byte[length]);

            var data = new byte[length];
            Array.Copy(readBuffer, 3, data, 0, length);

            return data;
        }

        public byte Read(ushort address)
        {
            return Read(address, 1)[0];
        }

        public void Write(ushort startAddress, params byte[] data)
        {
            if (data.Length > _pageSize - (startAddress % _pageSize))
                return;

            _spi.Write(new byte[] { (byte)Instruction.WriteEnable });
            SpiTransfer(Instruction.Write, startAddress, data);
        }

        public void Erase(ushort page)
        {
            if (page % _pageSize != 0)
                return;

            var data = new byte[_pageSize];
            for (int i = 0; i < data.Length; i++)
                data[i] = 0xFF;

            Write((ushort)(page * _pageSize), data);
        }


        private byte[] SpiTransfer(Instruction instruction, ushort address, params byte[] data)
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
