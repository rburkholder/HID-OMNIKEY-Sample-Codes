﻿/*****************************************************************************************
    (c) 2017 HID Global Corporation/ASSA ABLOY AB.  All rights reserved.

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HidGlobal.OK.Readers.Components;


namespace HidGlobal.OK.Readers
{
    public class Reader : IReader
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Reader name seen by Windows Smart Card Resource Manager.
        /// </summary>
        public string PcscReaderName { get; protected set; }
        /// <summary>
        /// Handle to winscard context pointer.
        /// </summary>
        private IntPtr _contextHandle;
        /// <summary>
        /// Handle to winscard card pointer.
        /// </summary>
        private CardHandle _cardConnectionHandle;
        /// <summary>
        /// Last error code returned by winscard functions.
        /// </summary>
        public ErrorCodes CurrentErrorStatus { get; private set; }
        /// <summary>
        /// Smart card protocol used in current connection.
        /// </summary>
        public Protocol ActiveProtocol => _cardConnectionHandle.ActiveProtocol;
        /// <summary>
        /// Reader share mode used in current connection.
        /// </summary>
        public ReaderSharingMode ConnectionMode => _cardConnectionHandle.ActiveConnectionMode;
        /// <summary>
        /// Connection status.
        /// </summary>
        public bool IsConnected => _cardConnectionHandle.IsConnected();
        protected byte[] OctetStringToByteArray(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                log.Error("OctetStringToByteArray function parameter is null or whitespace.");
                return null;
            }
            // Remove delimeters
            hex = hex.Replace(" ", "");
            hex = hex.Replace("-", "");

            if (hex.Length % 2 != 0)
                hex = hex.Insert(0, "0");
            try
            {
                return Enumerable.Range(0, hex.Length / 2).Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16)).ToArray();
            }
            catch (Exception error)
            {
                log.Error(null, error);
                return null;
            }
        }
        protected string ByteArrayToOctetString(byte[] bytes)
        {
            if (bytes == null)
            {
                log.Error("ByteArrayToOctetString function parameter is null.");
                return null;
            }
            try
            {
                return bytes.Select(x => x.ToString("X2")).Aggregate((s1, s2) => s1 + s2);
            }
            catch (Exception error)
            {
                log.Error(null, error);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextHandle">Handle to winscard context pointer.</param>
        /// <param name="pcscName">Reader name seen by Windows Smart Card Resource Manager.</param>
        public Reader(IntPtr contextHandle, string pcscName)
        {
            PcscReaderName = pcscName;
            _contextHandle = contextHandle;
            _cardConnectionHandle = new CardHandle();
        }
        /// <summary>
        /// Establishes a connection between the calling application and a smart card.
        /// </summary>
        /// <param name="mode">Reader share mode to be used in current connection.</param>
        /// <param name="preferredProtocol">Smart card protocol to be used in current connection.</param>
        public void Connect(ReaderSharingMode mode, Protocol preferredProtocol)
        {
            CurrentErrorStatus = _cardConnectionHandle.Connect(_contextHandle, PcscReaderName, mode, preferredProtocol);            
        }
        /// <summary>
        /// Establishes a connection between the calling application and a smart card in Direct mode, 
        /// can be used to send control commands to reader without any smart card present.
        /// </summary>
        public void ConnectDirect()
        {
            CurrentErrorStatus = _cardConnectionHandle.Connect(_contextHandle, PcscReaderName, ReaderSharingMode.Direct, Protocol.None);
        }
        /// <summary>
        /// Reestablishes an existing connection from the calling application to the smart card.
        /// </summary>
        /// <param name="initialization">Type of initialization that should be performed on the card.</param>
        /// <param name="mode">Reader share mode to be used in current connection.</param>
        /// <param name="protocol">Smart card protocol to be used in current connection.</param>
        public void Reconnect(CardDisposition initialization, ReaderSharingMode mode, Protocol protocol)
        {
            CurrentErrorStatus = _cardConnectionHandle.Reconnect(initialization, mode, protocol);
        }
        /// <summary>
        /// Terminates a connection between the calling application and a smart card.
        /// </summary>
        /// <param name="disposition">Action to take on the card in the connected reader on close.</param>
        public void Disconnect(CardDisposition disposition = CardDisposition.Eject)
        {
            CurrentErrorStatus = _cardConnectionHandle.Disconnect(disposition);
        }
        /// <summary>
        ///  The function waits for the completion of all other transactions before it begins. After the transaction starts, 
        /// all other applications are blocked from accessing the smart card while the transaction is in progress.
        /// </summary>
        public void BeginTransaction()
        {
            CurrentErrorStatus = _cardConnectionHandle.BeginTransaction();
        }
        /// <summary>
        /// The SCardEndTransaction function completes a previously declared transaction, 
        /// allowing other applications to resume interactions with the card.
        /// </summary>
        /// <param name="disposition">Action to take on the card in the connected reader on close.</param>
        public void EndTransaction(CardDisposition disposition)
        {
            CurrentErrorStatus = _cardConnectionHandle.EndTransaction(disposition);
        }
        /// <summary>
        /// State of smart card in the reader.
        /// </summary>
        /// <returns></returns>
        public CardState ActiveCardState()
        {
            CheckStatus();
            return _cardConnectionHandle.ActiveCardState;
        }
        /// <summary>
        /// The CheckStatus function provides the current status of a smart card in a reader. 
        /// You can call it any time after a successful call to Connect and before a successful call to Disconnect. 
        /// It does not affect the state of the reader or reader driver.
        /// </summary>
        /// <returns></returns>
        public byte[] CheckStatus()
        {
            var atr = new byte[0];
            CurrentErrorStatus = _cardConnectionHandle.Status(out atr);
            return atr;
        }
        /// <summary>
        /// Gets direct control of the reader after Connect is called.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual byte[] Control(ReaderControlCode control, byte[] data)
        {
            var output = new byte[0];
            CurrentErrorStatus = _cardConnectionHandle.Control(control, data, out output);
            return output;
        }
        /// <summary>
        /// Gets direct control of the reader after Connect is called.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual string Control(ReaderControlCode control, string data)
        {
            var temp = OctetStringToByteArray(data);
            if (temp == null)
            {
                log.Error("Wrong data of Control(ReaderControlCode, string) function string argument");
                return null;
            }
            temp = Control(control, temp);
            return ByteArrayToOctetString(temp);
        }
        /// <summary>
        /// Sends a service request to a smart card.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Transmit(byte[] data)
        {
            var output = new byte[0];
            CurrentErrorStatus = _cardConnectionHandle.Transmit(data, out output);
            return output;
        }
        /// <summary>
        /// Sends a service request to a smart card.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Transmit(string data)
        {
            var temp = OctetStringToByteArray(data);
            if (temp == null)
            {
                log.Error("Wrong data of Transmit(string) function string argument");
                return null;
            }
            temp = Transmit(temp);
            return ByteArrayToOctetString(temp);
        }
        /// <summary>
        /// Gets the current reader's attributes from a given reader, driver, or smart card.
        /// </summary>
        /// <param name="attributeId">Identifier for the attribute to get.</param>
        /// <returns></returns>
        public byte[] GetAttribute(Attribiutes attributeId)
        {
            var output = new byte[0];
            var retCode = _cardConnectionHandle.GetAttribiute(attributeId, out output);
            CurrentErrorStatus = retCode;

            if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                return null;
            else
                return output;
        }
        /// <summary>
        /// Sets a given reader attribute.
        /// </summary>
        /// <param name="attributeId">Identifier for the attribute to set.</param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public bool SetAttribute(Attribiutes attributeId, byte[] attribute)
        {
            var retCode = _cardConnectionHandle.SetAttribiute(attributeId, attribute);
            CurrentErrorStatus = retCode;

            if (retCode != ErrorCodes.SCARD_S_SUCCESS)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Perform application-defined tasks associated with freeing, releasing or reseting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)_cardConnectionHandle).Dispose();
        }
    }
}
