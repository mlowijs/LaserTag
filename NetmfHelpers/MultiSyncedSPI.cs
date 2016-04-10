using Microsoft.SPOT.Hardware;
using System.Collections;

namespace NetmfHelpers
{
    public class MultiSyncedSPI
    {
        private static Hashtable _spis;
        private static Hashtable _syncObjects;

        private SPI.SPI_module _module;
        private SPI.Configuration _config;

        public MultiSyncedSPI(SPI.Configuration config)
        {
            _config = config;
            _module = config.SPI_mod;

            if (_spis == null)
            {
                _spis = new Hashtable();
                _syncObjects = new Hashtable();
            }

            if (!_spis.Contains(_module))
            {
                _spis[_module] = new SPI(_config);
                _syncObjects[_module] = new object();
            }
        }


        public void Write(byte[] writeBuffer)
        {
            var spi = (SPI)_spis[_module];

            lock (_syncObjects[_module])
            {
                spi.Config = _config;

                spi.Write(writeBuffer);
            }
        }

        public void Write(ushort[] writeBuffer)
        {
            var spi = (SPI)_spis[_module];

            lock (_syncObjects[_module])
            {
                spi.Config = _config;

                spi.Write(writeBuffer);
            }
        }

        public void WriteLsb(byte[] writeBuffer)
        {
            InvertBytes(writeBuffer);

            Write(writeBuffer);
        }


        public void WriteRead(byte[] writeBuffer, byte[] readBuffer)
        {
            var spi = (SPI)_spis[_module];

            lock (_syncObjects[_module])
            {
                spi.Config = _config;

                spi.WriteRead(writeBuffer, readBuffer);
            }
        }

        public void WriteRead(ushort[] writeBuffer, ushort[] readBuffer)
        {
            var spi = (SPI)_spis[_module];

            lock (_syncObjects[_module])
            {
                spi.Config = _config;

                spi.WriteRead(writeBuffer, readBuffer);
            }
        }

        public void WriteReadLsb(byte[] writeBuffer, byte[] readBuffer)
        {
            InvertBytes(writeBuffer);

            WriteRead(writeBuffer, readBuffer);

            InvertBytes(readBuffer);
        }


        private void InvertBytes(byte[] bytes)
        {
            // Iterate over all bytes
            for (var i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0 || bytes[i] == 0xFF)
                    continue;

                byte output = 0;

                for (var j = 0; j < 8; j++)
                {
                    output <<= 1;
                    output |= (byte)(bytes[i] & 0x01);
                    bytes[i] >>= 1;
                }

                bytes[i] = output;
            }
        }
    }
}
