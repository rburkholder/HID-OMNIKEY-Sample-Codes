/*****************************************************************************************
    (c) 2017-2018 HID Global Corporation/ASSA ABLOY AB.  All rights reserved.

      Redistribution and use in source and binary forms, with or without modification,
      are permitted provided that the following conditions are met:
         - Redistributions of source code must retain the above copyright notice,
           this list of conditions and the following disclaimer.
         - Redistributions in binary form must reproduce the above copyright notice,
           this list of conditions and the following disclaimer in the documentation
           and/or other materials provided with the distribution.
           THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
           AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
           THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
           ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
           FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
           (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
           LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
           ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
           (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
           THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*****************************************************************************************/
using HidGlobal.OK.Readers;
using HidGlobal.OK.Readers.AViatoR.Components;
using HidGlobal.OK.Readers.Components;
using HidGlobal.OK.SampleCodes.Utilities;
using System;
using System.Linq;

namespace HidGlobal.OK.SampleCodes.AViatoR
{
    class ExampleWithMifareClassic
    {
        public class infoKeySlot
        {
            public byte idSlot;
            public string keyValue;
            public infoKeySlot(byte idSlot, string keyValue) { this.idSlot = idSlot; this.keyValue = keyValue; }
        }
        public class LoadKeyExample
        {
            private void LoadKeyCommand(ISmartCardReader smartCardReader, string description, byte keySlot, LoadKeyCommand.KeyType keyType, LoadKeyCommand.Persistence persistence, LoadKeyCommand.Transmission transmission, LoadKeyCommand.KeyLength keyLength, string key)
            {
                var loadKeyCommand = new Readers.AViatoR.Components.LoadKeyCommand();

                string input = loadKeyCommand.GetApdu(keySlot, keyType, persistence, transmission, keyLength, key);
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                ConsoleWriter.Instance.PrintCommand(description + key, input, output);
            }
            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);

                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to { reader.PcscReaderName}");

                    ReaderHelper.ConnectToReader(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    LoadKeyCommand(reader, "Load Mifare Key: ", 0x00,
                        Readers.AViatoR.Components.LoadKeyCommand.KeyType.CardKey,
                        Readers.AViatoR.Components.LoadKeyCommand.Persistence.Persistent,
                        Readers.AViatoR.Components.LoadKeyCommand.Transmission.Plain,
                        Readers.AViatoR.Components.LoadKeyCommand.KeyLength._6Bytes, "000000000000");

                    LoadKeyCommand(reader, "Load Mifare Key: ", 0x01,
                        Readers.AViatoR.Components.LoadKeyCommand.KeyType.CardKey,
                        Readers.AViatoR.Components.LoadKeyCommand.Persistence.Persistent,
                        Readers.AViatoR.Components.LoadKeyCommand.Transmission.Plain,
                        Readers.AViatoR.Components.LoadKeyCommand.KeyLength._6Bytes, "FFFFFFFFFFFF");

                    LoadKeyCommand(reader, "Load Mifare Key: ", 0x02,
                        Readers.AViatoR.Components.LoadKeyCommand.KeyType.CardKey,
                        Readers.AViatoR.Components.LoadKeyCommand.Persistence.Persistent,
                        Readers.AViatoR.Components.LoadKeyCommand.Transmission.Plain,
                        Readers.AViatoR.Components.LoadKeyCommand.KeyLength._6Bytes, "3520648DF6FA");

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class ReadBinaryMifareClassic1kExample
        {
            private void LoadKeyCommand(ISmartCardReader smartCardReader, string description, byte keySlot, LoadKeyCommand.KeyType keyType, LoadKeyCommand.Persistence persistence, LoadKeyCommand.Transmission transmission, LoadKeyCommand.KeyLength keyLength, string key)
            {
                var loadKeyCommand = new Readers.AViatoR.Components.LoadKeyCommand();

                string input = loadKeyCommand.GetApdu(keySlot, keyType, persistence, transmission, keyLength, key);
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                //ConsoleWriter.Instance.PrintCommand(description + key, input, output);
                ConsoleWriter.Instance.PrintMessage("load mifare key in slot 0x" + keySlot.ToString("X2") + " key " + key + " result " + output);
            }
            public byte HexToByte(char conv)
            {
                byte result = 0;
                if (('0' <= conv) && ('9' >= conv)) { result = (byte)(conv - '0'); }
                if (('a' <= conv) && ('f' >= conv)) { result = (byte)(conv - 'a' + 10); }
                if (('A' <= conv) && ('F' >= conv)) { result = (byte)(conv - 'A' + 10); }
                return result;
            }

            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);

                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    // https://smartlockpicking.com/slides/Confidence_A_2018_Practical_Guide_To_Hacking_RFID_NFC.pdf

                    infoKeySlot[] keySlots = new infoKeySlot[] {
                        new infoKeySlot(0x08, "FFFFFFFFFFFF"),
                        new infoKeySlot(0x09, "A0A1A2A3A4A5"),
                        new infoKeySlot(0x0a, "0000014B5C31"),
                        new infoKeySlot(0x0b, "b578f38a5c61 "),
                        new infoKeySlot(0x0c, "3520648DF6FA"),
                        //new infoKeySlot(0x0d, "B0B1B2B3B4B5"),
                        //new infoKeySlot(0x0c, "EC0A9B1A9E06"), // 4k?
                        //new infoKeySlot(0x0d, "6C94E1CED026"), // 4k?
                        //new infoKeySlot(0x0e, "0F230695923F"), // 4k?
                        //new infoKeySlot(0x0f, "0000014B5C31"), // 4k?

                        new infoKeySlot(0x10, "b708bea4f08b"), // cracker
                        new infoKeySlot(0x11, "96007e10d60f"), // cracker
                        new infoKeySlot(0x12, "3c00006ee21b"), // cracker
                        //new infoKeySlot(0x10, "96007e10d60f "),

                        //new infoKeySlot(0x11, "560f7cff2d81"),
                        //new infoKeySlot(0x12, "a0a1a2a3a4a5"),
                        //new infoKeySlot(0x14, "0000014b5c31"),

                        //new infoKeySlot(0x15, "96a301bce267"),
                        //new infoKeySlot(0x16, "8fd0a4f256e9"),
                        //new infoKeySlot(0x17, "b578f38a5c61"),
                        //new infoKeySlot(0x18, "96a301bce267"),


                        //new infoKeySlot(0x10, "A1B1C1D1E1F1"),
                        //new infoKeySlot(0x11, "000000000000")

                    };

                    foreach (infoKeySlot slot in keySlots)
                    {
                        LoadKeyCommand(reader, "Load Mifare Key: ", slot.idSlot,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyType.CardKey,
                            Readers.AViatoR.Components.LoadKeyCommand.Persistence.Persistent,
                            Readers.AViatoR.Components.LoadKeyCommand.Transmission.Plain,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyLength._6Bytes, slot.keyValue);
                    }

                    var generalAuthenticateCommand = new Readers.AViatoR.Components.GeneralAuthenticateCommand();
                    //var readBinaryCommand = new Readers.AViatoR.Components.ReadBinaryCommand();

                    GeneralAuthenticateCommand.MifareKeyType[] keyTypes = { GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, GeneralAuthenticateCommand.MifareKeyType.MifareKeyB };

                    byte[] sectors = { 0, 1, 2, 3, 4, 5, 6 };
                    //byte[] sectors = { 0 };
                    foreach (byte sector in sectors)
                    {

                        bool bAuthenticated = false;
                        byte baseBlock = (byte)(sector * 0x04);
                        infoKeySlot[] keyValues = new infoKeySlot[]{
                            new infoKeySlot( 0x00, "unknown" ),
                            new infoKeySlot( 0x00, "unknown" )
                        };

                        // determine which keys auth for the sector
                        for (int ixKeyType = 0; ixKeyType < 2; ixKeyType++)
                        {
                            //ConsoleWriter.Instance.PrintMessage("sector=" + sector.ToString("X2") + " debug ixKeyType=" + ixKeyType.ToString());
                            GeneralAuthenticateCommand.MifareKeyType keyType = keyTypes[ixKeyType];
                            foreach (infoKeySlot keySlot in keySlots)
                            {
                                string input = generalAuthenticateCommand.GetMifareApdu((byte)(baseBlock), keyType, keySlot.idSlot);
                                string output = reader.Transmit(input);
                                //ConsoleWriter.Instance.PrintMessage("sector=" + sector.ToString("X2") + " debug ixKeyType=" + ixKeyType.ToString() + "," + output);
                                if ("9000" == output)
                                {
                                    keyValues[ixKeyType] = keySlot;
                                    bAuthenticated = true;
                                    break;
                                }
                            }
                        }
                        ConsoleWriter.Instance.PrintMessage("sector 0x" + sector.ToString("X2") + " auth "
                            + "keyA=(" + keyValues[0].idSlot.ToString("X2") + ")" + keyValues[0].keyValue + ","
                            + "keyB=(" + keyValues[1].idSlot.ToString("X2") + ")" + keyValues[1].keyValue
                            );

                        if (!bAuthenticated)
                        {
                            //ConsoleWriter.Instance.PrintMessage("sector 0x" + sector.ToString("X2") + " block 0x" + baseBlock.ToString("X2") + ":  no key slot worked");
                        }
                        else
                        {
                            // cycle through the blocks in each sector
                            for (byte ixBlock = 0; ixBlock < 0x04; ixBlock++)
                            {
                                byte block = (byte)(baseBlock + ixBlock);
                                string content = "can't read";
                                bool bRead = false;
                                int[] setOfKeyIndexes = { 1, 0 };
                                foreach (int ixKeyValue in setOfKeyIndexes)
                                {
                                    var keySlot = keyValues[ixKeyValue];
                                    if (!bRead && (0x00 != keySlot.idSlot))
                                    {
                                        // clear auth against another sector
                                        string inputAuth = generalAuthenticateCommand.GetMifareApdu((byte)((sector + 1) * 4), keyTypes[ixKeyValue], keySlot.idSlot);
                                        string outputAuth = reader.Transmit(inputAuth);
                                        // auth against sector we want
                                        inputAuth = generalAuthenticateCommand.GetMifareApdu(block, keyTypes[ixKeyValue], keySlot.idSlot);
                                        outputAuth = reader.Transmit(inputAuth);
                                        //ConsoleWriter.Instance.PrintMessage("sector=0x" + sector.ToString("X2") + " debug1 blk:" + block.ToString("X2") + ", ix:" + ixKeyValue + ", slot:" + keySlot.idSlot.ToString("X2") + "," + outputAuth);
                                        if ("9000" != outputAuth)
                                        {
                                        }
                                        else
                                        {
                                            var readBinaryCommand = new Readers.AViatoR.Components.ReadBinaryCommand();
                                            string inputRead = readBinaryCommand.GetMifareReadApdu(block, 0x00);
                                            string outputRead = reader.Transmit(inputRead);
                                            //ConsoleWriter.Instance.PrintMessage("sector=0x" + sector.ToString("X2") + " debug2 " + outputRead);

                                            if (36 == outputRead.Length)
                                            {
                                                bRead = true;
                                                content = outputRead.Substring(0, 32);
                                                string mad = "";
                                                if (0x00 == sector)
                                                { // decode MAD (mifare application directory)
                                                    switch (ixBlock)
                                                    {
                                                        case 0x00: // manufacturer block
                                                            string uid = content.Substring(0, 8);
                                                            string cksum = content.Substring(8, 2);
                                                            string ctl = content.Substring(10, 6);
                                                            string other = content.Substring(16);
                                                            content = uid + "," + cksum + "," + ctl + "," + other;
                                                            break;
                                                        case 0x01: // data block 1
                                                            string content_ = content;
                                                            for (int ixSector = 0; ixSector < 8; ixSector++)
                                                            {
                                                                if (0 == ixSector)
                                                                {
                                                                    content = "cksm:" + content_.Substring(0, 4);
                                                                }
                                                                else
                                                                { // 8 bit function cluster code (01-07: misc app), 8 bit application code
                                                                    content += "," + ixSector.ToString() + ":" + content_.Substring(4 * ixSector, 4);
                                                                }
                                                            }
                                                            break;
                                                        case 0x02: // data block 2
                                                            break;
                                                        case 0x03: // keys, acl, user
                                                            string user = content.Substring(18, 2);
                                                            byte gpb1 = HexToByte(user[0]);
                                                            byte gpb2 = HexToByte(user[1]);
                                                            byte gpb = (byte)((gpb1 << 4) | gpb2);
                                                            if (0x80 == (gpb & 0x80))
                                                            { // DA = 1, MAD available
                                                                if (0x40 == (gpb & 0x40))
                                                                { // MA = 1, multi-application card, 0 for mono
                                                                    switch (gpb & 0x03)
                                                                    {
                                                                        case 0x01:
                                                                            mad = ":madv1"; // mifare 1k
                                                                            break;
                                                                        case 0x02:
                                                                            mad = ":madv2"; // mifare 4k
                                                                            break;
                                                                    }
                                                                }
                                                            }
                                                            break;
                                                    }
                                                }
                                                if (0x03 == ixBlock)
                                                { // decode keys and ACL

                                                    string keyA = content.Substring(0, 12);
                                                    string acl = content.Substring(12, 6);
                                                    string user = content.Substring(18, 2);
                                                    string keyB = content.Substring(20, 12);
                                                    string state = "";
                                                    byte[] bytes = { HexToByte(acl[0]), HexToByte(acl[1]), HexToByte(acl[2]), HexToByte(acl[3]), HexToByte(acl[4]), HexToByte(acl[5]) };

                                                    if ((0x0f == (bytes[0] ^ bytes[5])) && (0x0f == (bytes[1] ^ bytes[2])) && (0x0f == (bytes[3] ^ bytes[4])))
                                                    { // assert acl correctness
                                                        byte bitsC1 = bytes[2];
                                                        byte bitsC2 = bytes[5];
                                                        byte bitsC3 = bytes[4];
                                                        //state = bitsC1.ToString("X1") + bitsC2.ToString("X1") + bitsC3.ToString("X1");
                                                        byte bitsC1_ = 0;
                                                        byte bitsC2_ = 0;
                                                        byte bitsC3_ = 0;
                                                        int right = 0;
                                                        for (byte op = 1; op < 0x10; op = (byte)(op << 1))
                                                        {
                                                            bitsC1_ = (byte)((bitsC1 & op) << 2);
                                                            bitsC2_ = (byte)((bitsC2 & op) << 1);
                                                            bitsC3_ = (byte)(bitsC3 & op);
                                                            byte C = (byte)((bitsC1_ | bitsC2_ | bitsC3_) >> right);
                                                            right++;
                                                            state += C.ToString("X1");
                                                        }
                                                    }
                                                    content = keyA + "," + acl + "," + user + "," + keyB + "-" + state + mad;
                                                }
                                            }
                                        }
                                    }
                                }
                                ConsoleWriter.Instance.PrintMessage("sector 0x" + sector.ToString("X2") + " block 0x" + block.ToString("X2") + ": " + content);
                            } // for each block in sector
                        } // process when authenticated
                    } // for each sector

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class UpdateBinaryMifareClassic1kExample
        {
            private void LoadKeyCommand(ISmartCardReader smartCardReader, string description, byte keySlot, LoadKeyCommand.KeyType keyType, LoadKeyCommand.Persistence persistence, LoadKeyCommand.Transmission transmission, LoadKeyCommand.KeyLength keyLength, string key)
            {
                var loadKeyCommand = new Readers.AViatoR.Components.LoadKeyCommand();

                string input = loadKeyCommand.GetApdu(keySlot, keyType, persistence, transmission, keyLength, key);
                string output = ReaderHelper.SendCommand(smartCardReader, input);
                ConsoleWriter.Instance.PrintCommand(description + key, input, output);
            }
            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);

                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    infoKeySlot[] keySlots = new infoKeySlot[] {
                        new infoKeySlot(0x08, "FFFFFFFFFFFF"),
                        new infoKeySlot(0x09, "A0A1A2A3A4A5"), // sector 0x00 keyA
                        new infoKeySlot(0x0a, "B578F38A5C61"), // sector 0x00 keyB
                        new infoKeySlot(0x0b, "96007e10d60f"), // sector 0x01 keyA
                        new infoKeySlot(0x0c, "3c00006ee21b"), // sector 0x01 keyB
                        new infoKeySlot(0x0d, "0000014B5C31"), // sector 0x02 keyB
                        new infoKeySlot(0x0e, "b708bea4f08b"), // sector 0x06 keyB
                    };

                    foreach (infoKeySlot slot in keySlots)
                    {
                        LoadKeyCommand(reader, "Load Mifare Key: ", slot.idSlot,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyType.CardKey,
                            Readers.AViatoR.Components.LoadKeyCommand.Persistence.Persistent,
                            Readers.AViatoR.Components.LoadKeyCommand.Transmission.Plain,
                            Readers.AViatoR.Components.LoadKeyCommand.KeyLength._6Bytes, slot.keyValue);
                    }

                    //byte keySlot2 = 0x0a;
                    int ixBlkFFFF = 0;
                    int ixBlkA0A1 = 1;
                    int ixBlkB578 = 2;
                    int ixBlk9600 = 3;
                    int ixBlk3c00 = 4;
                    int ixBlk0000 = 5;
                    int ixBlkb708 = 6;

                    byte blkManu = 0x00;

                    //var generalAuthenticateCommand = new Readers.AViatoR.Components.GeneralAuthenticateCommand();
                    string input;
                    string output;

                    if (false)
                    {
                        // authenticate to idBlock, read idBlock
                        var generalAuthenticateCommand = new Readers.AViatoR.Components.GeneralAuthenticateCommand();
                        input = generalAuthenticateCommand.GetMifareApdu(blkManu, GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, keySlots[ixBlkA0A1].idSlot);
                        output = reader.Transmit(input);
                        ConsoleWriter.Instance.PrintCommand("Authenticate with key from slot #" + keySlots[ixBlkA0A1].idSlot.ToString("X2"), input, output);
                        ReaderHelper.ReadBinaryMifareCommand(reader, "Read Binary block #", blkManu, 0x0); // read manufacturer block
/*
                        // authenticate and write sector 0x00, blocks 0x01, 0x03
                        input = generalAuthenticateCommand.GetMifareApdu(0x03, GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, keySlots[0].idSlot);
                        output = reader.Transmit(input);
                        ConsoleWriter.Instance.PrintCommand("Authenticate with key from slot #" + keySlots[0].idSlot.ToString("X2"), input, output);
                        ReaderHelper.UpdateBinaryCommand(reader, "Update Binary sector 0x00, block #", UpdateBinaryCommand.Type.Plain, 0x01, "D5010670057007700770077009700000");
                        ReaderHelper.UpdateBinaryCommand(reader, "Update Binary sector 0x00, block #", UpdateBinaryCommand.Type.Plain, 0x03, "A0A1A2A3A4A5787788C1B578F38A5C61");
                        //ReaderHelper.UpdateBinaryCommand(reader, "Update Binary block nr ", UpdateBinaryCommand.Type.Plain, 0x03, "FFFFFFFFFFFFFF078069FFFFFFFFFFFF");
                        //"FFFFFFFFFFFF FF0780 69 FFFFFFFFFFFF"
                        // ReaderHelper.UpdateBinaryCommand(reader, "Update Binary block nr ", UpdateBinaryCommand.Type.Plain, 0x05, "0000000000007F078800000000000000 9000");

                        // authenticate and write sector 0x02, block 0x0b
                        input = generalAuthenticateCommand.GetMifareApdu(0x0b, GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, keySlots[0].idSlot);
                        output = reader.Transmit(input);
                        ConsoleWriter.Instance.PrintCommand("Authenticate with key from slot #" + keySlots[0].idSlot.ToString("X2"), input, output);
                        ReaderHelper.UpdateBinaryCommand(reader, "Update Binary sector 0x02, block #", UpdateBinaryCommand.Type.Plain, 0x0b, "A0A1A2A3A4A50F00FFAA0000014B5C31");
*/
                    }

                    if (true)
                    {
                        // authenticate and write sector 0x02, block 0x0b
                        var generalAuthenticateCommand = new Readers.AViatoR.Components.GeneralAuthenticateCommand();
                        input = generalAuthenticateCommand.GetMifareApdu(0x08, GeneralAuthenticateCommand.MifareKeyType.MifareKeyB, keySlots[2].idSlot);
                        output = reader.Transmit(input);
                        ConsoleWriter.Instance.PrintCommand("Authenticate with key from slot #" + keySlots[2].idSlot.ToString("X2"), input, output);
                        ReaderHelper.UpdateBinaryCommand(reader, "Update Binary sector 0x02, block #", UpdateBinaryCommand.Type.Plain, 0x08, "00000000000000000000000000000000");
                    }

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class IncrementMifareClassic1kExample
        {
            string GetMifareValueTypeData(int value, byte blockNumber)
            {
                var valueBytes = BitConverter.GetBytes(value);
                var invertedValueBytes = BitConverter.GetBytes(~value);
                if (!BitConverter.IsLittleEndian)
                {
                    valueBytes = valueBytes.Reverse().ToArray();
                    invertedValueBytes = invertedValueBytes.Reverse().ToArray();
                }
                string lsbFirstValue = BitConverter.ToString(valueBytes).Replace("-", "");
                string lsbFirstInvertedValue = BitConverter.ToString(invertedValueBytes).Replace("-", "");

                return lsbFirstValue + lsbFirstInvertedValue + lsbFirstValue + $"{blockNumber:X2}" +
                       ((byte)~blockNumber).ToString("X2") + $"{blockNumber:X2}" +
                       ((byte)~blockNumber).ToString("X2");
            }
            void SendIncrementCommand(ISmartCardReader smartCardReader, string description, int value, byte blockNumber)
            {
                var incrementCommand = new IncrementCommand();
                string input = incrementCommand.GetApdu(blockNumber, value);
                string output = ReaderHelper.SendCommand(smartCardReader, input);

                ConsoleWriter.Instance.PrintCommand(description + blockNumber.ToString("X2"), input, output);
            }
            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    ReaderHelper.GeneralAuthenticateMifare(reader, "Authenticate with key from slot nr ", 0x04,
                        GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, 0x02);
                    // Update block 4 with write operation in value block format:
                    // 4 byte value LSByte first, 4 byte bit inverted represetaton of value LSByte first, 4 byte value LSByte first, 1 byte block address, 1 byte bit inverted block address, 1 byte block address, 1 byte bit inverted block address
                    string valueTypeData = GetMifareValueTypeData(1234567, 0x04);
                    ReaderHelper.UpdateBinaryCommand(reader, "Create value type in block nr ",
                        UpdateBinaryCommand.Type.Plain, 0x04, valueTypeData);

                    SendIncrementCommand(reader, "Increment value in block nr: ", 1, 0x04);

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class DecrementMifareClassic1kExample
        {
            string GetMifareValueTypeData(int value, byte blockNumber)
            {
                var valueBytes = BitConverter.GetBytes(value);
                var invertedValueBytes = BitConverter.GetBytes(~value);
                if (!BitConverter.IsLittleEndian)
                {
                    valueBytes = valueBytes.Reverse().ToArray();
                    invertedValueBytes = invertedValueBytes.Reverse().ToArray();
                }
                string lsbFirstValue = BitConverter.ToString(valueBytes).Replace("-", "");
                string lsbFirstInvertedValue = BitConverter.ToString(invertedValueBytes).Replace("-", "");

                return lsbFirstValue + lsbFirstInvertedValue + lsbFirstValue + $"{blockNumber:X2}" +
                       ((byte)~blockNumber).ToString("X2") + $"{blockNumber:X2}" +
                       ((byte)~blockNumber).ToString("X2");
            }
            void SendDecrementCommand(ISmartCardReader smartCardReader, string description, int value, byte blockNumber)
            {
                var decrementCommand = new DecrementCommand();
                string input = decrementCommand.GetApdu(blockNumber, value);
                string output = ReaderHelper.SendCommand(smartCardReader, input);

                ConsoleWriter.Instance.PrintCommand(description + blockNumber.ToString("X2"), input, output);
            }
            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    ReaderHelper.GeneralAuthenticateMifare(reader, "Authenticate with key from slot nr ", 0x04,
                        GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, 0x02);
                    // Update block 4 with write operation in value block format:
                    // 4 byte value LSByte first, 4 byte bit inverted represetaton of value LSByte first, 4 byte value LSByte first, 1 byte block address, 1 byte bit inverted block address, 1 byte block address, 1 byte bit inverted block address
                    string valueTypeData = GetMifareValueTypeData(1234567, 0x04);
                    ReaderHelper.UpdateBinaryCommand(reader, "Create value type in block nr ",
                        UpdateBinaryCommand.Type.Plain, 0x04, valueTypeData);

                    SendDecrementCommand(reader, "Decrement value in block nr: ", 1, 0x04);

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class IncrementMifareClassic1kForOK5023Example
        {
            string GetMifareValueTypeData(int value, byte blockNumber)
            {
                var valueBytes = BitConverter.GetBytes(value);
                var invertedValueBytes = BitConverter.GetBytes(~value);
                if (!BitConverter.IsLittleEndian)
                {
                    valueBytes = valueBytes.Reverse().ToArray();
                    invertedValueBytes = invertedValueBytes.Reverse().ToArray();
                }
                string lsbFirstValue = BitConverter.ToString(valueBytes).Replace("-", "");
                string lsbFirstInvertedValue = BitConverter.ToString(invertedValueBytes).Replace("-", "");

                return lsbFirstValue + lsbFirstInvertedValue + lsbFirstValue + $"{blockNumber:X2}" +
                       ((byte)~blockNumber).ToString("X2") + $"{blockNumber:X2}" +
                       ((byte)~blockNumber).ToString("X2");
            }
            void SendIncrementCommand(ISmartCardReader smartCardReader, string description, int value, byte blockNumber)
            {
                var incrementCommand = new IncrementDecrementCommand();
                string input = incrementCommand.GetApdu(IncrementDecrementCommand.OperationType.Increment, blockNumber, value);
                string output = ReaderHelper.SendCommand(smartCardReader, input);

                ConsoleWriter.Instance.PrintCommand(description + blockNumber.ToString("X2"), input, output);
            }
            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);
                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    ReaderHelper.GeneralAuthenticateMifare(reader, "Authenticate with key from slot nr ", 0x04,
                        GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, 0x02);
                    // Update block 4 with write operation in value block format:
                    // 4 byte value LSByte first, 4 byte bit inverted represetaton of value LSByte first, 4 byte value LSByte first, 1 byte block address, 1 byte bit inverted block address, 1 byte block address, 1 byte bit inverted block address
                    string valueTypeData = GetMifareValueTypeData(1234567, 0x04);
                    ReaderHelper.UpdateBinaryCommand(reader, "Create value type in block nr ",
                        UpdateBinaryCommand.Type.Plain, 0x04, valueTypeData);

                    SendIncrementCommand(reader, "Increment value in block nr: ", 1, 0x04);

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
        public class DecrementMifareClassic1kForOK5023Example
        {
            string GetMifareValueTypeData(int value, byte blockNumber)
            {
                var valueBytes = BitConverter.GetBytes(value);
                var invertedValueBytes = BitConverter.GetBytes(~value);
                if (!BitConverter.IsLittleEndian)
                {
                    valueBytes = valueBytes.Reverse().ToArray();
                    invertedValueBytes = invertedValueBytes.Reverse().ToArray();
                }
                string lsbFirstValue = BitConverter.ToString(valueBytes).Replace("-", "");
                string lsbFirstInvertedValue = BitConverter.ToString(invertedValueBytes).Replace("-", "");

                return lsbFirstValue + lsbFirstInvertedValue + lsbFirstValue + $"{blockNumber:X2}" +
                       ((byte)~blockNumber).ToString("X2") + $"{blockNumber:X2}" +
                       ((byte)~blockNumber).ToString("X2");
            }
            void SendDecrementCommand(ISmartCardReader smartCardReader, string description, int value, byte blockNumber)
            {
                var decrementCommand = new IncrementDecrementCommand();
                string input = decrementCommand.GetApdu(IncrementDecrementCommand.OperationType.Decrement, blockNumber, value);
                string output = ReaderHelper.SendCommand(smartCardReader, input);

                ConsoleWriter.Instance.PrintCommand(description + blockNumber.ToString("X2"), input, output);
            }
            public void Run(string readerName)
            {
                var reader = new SmartCardReader(readerName);

                try
                {
                    ConsoleWriter.Instance.PrintSplitter();
                    ConsoleWriter.Instance.PrintTask($"Connecting to {reader.PcscReaderName}");

                    ReaderHelper.ConnectToReaderWithCard(reader);

                    ConsoleWriter.Instance.PrintMessage($"Connected\nConnection Mode: {reader.ConnectionMode}");
                    ConsoleWriter.Instance.PrintSplitter();

                    ReaderHelper.GeneralAuthenticateMifare(reader, "Authenticate with key from slot nr ", 0x04,
                        GeneralAuthenticateCommand.MifareKeyType.MifareKeyA, 0x02);
                    // Update block 4 with write operation in value block format:
                    // 4 byte value LSByte first, 4 byte bit inverted represetaton of value LSByte first, 4 byte value LSByte first, 1 byte block address, 1 byte bit inverted block address, 1 byte block address, 1 byte bit inverted block address
                    string valueTypeData = GetMifareValueTypeData(1234567, 0x04);
                    ReaderHelper.UpdateBinaryCommand(reader, "Create value type in block nr ",
                        UpdateBinaryCommand.Type.Plain, 0x04, valueTypeData);

                    SendDecrementCommand(reader, "Decrement value in block nr: ", 1, 0x04);

                    ConsoleWriter.Instance.PrintSplitter();
                }
                catch (Exception e)
                {
                    ConsoleWriter.Instance.PrintError(e.Message);
                }
                finally
                {
                    if (reader.IsConnected)
                    {
                        reader.Disconnect(CardDisposition.Unpower);
                        ConsoleWriter.Instance.PrintMessage("Reader connection closed");
                    }
                    ConsoleWriter.Instance.PrintSplitter();
                }
            }
        }
    }
}
