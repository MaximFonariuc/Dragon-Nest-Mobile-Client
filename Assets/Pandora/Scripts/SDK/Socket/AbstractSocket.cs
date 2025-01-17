using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Packet = System.Collections.Generic.Dictionary<string, object>;

namespace com.tencent.pandora
{
    /// <summary>
    /// Socket连接步骤
    /// 1.Dns查询IPAddress
    /// 2.连接Socket
    /// 数据结构：
    /// 1.4字节的包头，内容为包体长度值
    /// 2.包体内容
    /// </summary>
    internal abstract class AbstractTcpClient : MonoBehaviour
    {
        //包头长度size(uint)
        protected const int HEADER_SIZE = 4;
        protected const int SEND_BUFFER_SIZE = 256 * 1024;
        protected static Packet EMPTY_PACKET = new Packet();

        private object _lockObj = new object();

        protected Socket _socket;
        protected string _host;
        protected string _address;
        protected int _port;
        //接受最大包的字节长度为4M
        protected const int MAX_SHARED_BUFFER_LENGTH = 4 * 1024 * 1024;
        private byte[] _sharedBuffer = new byte[1024];
        private byte[] _bodyBuffer;
        private int _bodyLength = 0;
        private int _readBodyLength = 0;
        private byte[] _headerBuffer;

        private Queue<Packet> _sendQueue;
        private Coroutine _waitCoroutine;
        private Coroutine _sendCoroutine;
        private Coroutine _receiveCoroutine;

        volatile private SocketState _state = SocketState.Disconnect;
        private Action _callback; //不管socket成功失败都给上层一个回调
        public bool PauseSending { get; set; }
        /// <summary>
        /// 当Broker域名解析失败或IP连接不上时，使用替代IP地址
        /// </summary>
        public string AlternateIp1 { get; set; }
        public string AlternateIp2 { get; set; }
        private bool _canConnectAlternateIp;

        public void Reconnect()
        {
            Connect(_host, _port, null);
        }

        public void Connect(string host, int port, Action callback)
        {
            if (this.State == SocketState.Connecting)
            {
                return;
            }

            Close();
            _host = host;
            _port = port;
            _callback = callback;
            _sendQueue = new Queue<Packet>();
            this.State = SocketState.Connecting;
            _canConnectAlternateIp = true;
            Logger.LogInfo("开始连接Socket : " + _host + ":" + _port);
            SafeStartCoroutine(ref _waitCoroutine, WaitSocketConnect());
            Dns.BeginGetHostAddresses(host, OnGetHostAddress, host);
        }

        private void OnGetHostAddress(IAsyncResult result)
        {
            try
            {
                IPAddress[] addresses = Dns.EndGetHostAddresses(result);
                if (addresses != null && addresses.Length != 0)
                {
                    int index = (new System.Random()).Next(addresses.Length);
                    IPAddress address = addresses[index];
                    BeginConnectHost(address, _port);
                }
                else
                {
                    InnerHandleSocketConnectFailed();
                }
            }
            catch
            {
                InnerHandleSocketConnectFailed();
            }
        }

        private void BeginConnectHost(IPAddress address, int port)
        {
            try
            {
                _address = address.ToString();
                //Debug.Log("连接Socket: " + _host + " : " + _port.ToString());
                _socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _socket.SendBufferSize = SEND_BUFFER_SIZE;
                _socket.BeginConnect(address, port, OnConnectHost, _socket);
            }
            catch
            {
                //Debug.Log("Socket连接失败： " + _host + " : " + _port.ToString());
            }
        }

        private void OnConnectHost(IAsyncResult result)
        {
            try
            {
                Socket socket = result.AsyncState as Socket;
                if (socket.Connected == true)
                {
                    _headerBuffer = new byte[HEADER_SIZE];
                    _bodyLength = 0;
                    _readBodyLength = 0;
                    this.State = SocketState.Success;
                }
                else
                {
                    InnerHandleSocketConnectFailed();
                }
            }
            catch
            {
                InnerHandleSocketConnectFailed();
                //Debug.Log("Socket连接失败： " + _host + " : " + _port.ToString());
            }
        }

        private void InnerHandleSocketConnectFailed()
        {
            if (this.CanConnectAlternateIp == true)
            {
                try
                {
                    ConnectAlternateIp();
                }
                catch
                {
                    this.State = SocketState.Failed;
                }
            }
            else
            {
                this.State = SocketState.Failed;
            }
        }

        #region 连接后备Ip
        //一次连接过程只走一次后备ip途径
        private bool CanConnectAlternateIp
        {
            get
            {
                bool result = _canConnectAlternateIp == true &&
                             (string.IsNullOrEmpty(this.AlternateIp1) == false || string.IsNullOrEmpty(this.AlternateIp2) == false);
                _canConnectAlternateIp = false;
                return result;
            }
        }

        private void ConnectAlternateIp()
        {
            IPAddress address = GetTargetIpAddress();
            BeginConnectHost(address, _port);
        }

        //根据所处网络类型获取ipv4或ipv6地址
        private IPAddress GetTargetIpAddress()
        {
            AddressFamily family = GetLocalIpAddressFamily();
            if (family == AddressFamily.InterNetwork)
            {
                return IPAddress.Parse(SelectAlternateIp());
            }
            //将Ipv4转换为Ipv6
            return IPAddress.Parse("64:ff9b::" + SelectAlternateIp());
        }

        private string SelectAlternateIp()
        {
            List<string> ipList = new List<string>();
            if (string.IsNullOrEmpty(this.AlternateIp1) == false)
            {
                ipList.Add(this.AlternateIp1);
            }
            if (string.IsNullOrEmpty(this.AlternateIp2) == false)
            {
                ipList.Add(this.AlternateIp2);
            }
            int index = new System.Random().Next(0, ipList.Count);
            return ipList[index];
        }

        private AddressFamily GetLocalIpAddressFamily()
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            if (ipEntry.AddressList.Length == 0)
            {
                return AddressFamily.InterNetwork;
            }
            for(int i = 0; i < ipEntry.AddressList.Length; i++)
            {
                IPAddress address = ipEntry.AddressList[i];
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return AddressFamily.InterNetwork;
                }
            }
            return AddressFamily.InterNetworkV6;
        }
        #endregion
        /// <summary>
        /// 因为不能在Socket的异步线程中调用主线程的接口
        /// 所以必须创建一个协程以检查连接成功并初始化相关协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitSocketConnect()
        {
            while(true)
            {
                if(this.State == SocketState.Success)
                {
                    SafeStartCoroutine(ref _receiveCoroutine, DaemonReceive());
                    SafeStartCoroutine(ref _sendCoroutine, DaemonSend());
                    OnConnected();
                    if(_callback != null)
                    {
                        _callback();
                    }
                    yield break;
                }
                if(this.State == SocketState.Failed)
                {
                    OnConnectFailed();
                    if (_callback != null)
                    {
                        _callback();
                    }
                    yield break;
                }
                yield return null;
            }
        }

        protected IEnumerator DaemonReceive()
        {
            while (true)
            {
                try
                {
                    if (_socket == null || _socket.Connected == false)
                    {
                        Close();
                        yield break;
                    }
                    if (this.State == SocketState.Success)
                    {
                        while (_socket.Available > 0)
                        {
                            if (_bodyLength == 0 && _socket.Available >= HEADER_SIZE)
                            {
                                //读取包头
                                _socket.Receive(_headerBuffer);
                                _bodyLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_headerBuffer, 0));
                                _bodyBuffer = GetBodyBuffer(_bodyLength);
                                _readBodyLength = 0;
                            }
                            int size = _bodyLength - _readBodyLength;
                            #region 读取Socket中的数据
                            if (size > 0)
                            {
                                try
                                {
                                    //读取包体
                                    int readLength = _socket.Receive(_bodyBuffer, _readBodyLength, size, SocketFlags.None);
                                    if (readLength > 0)
                                    {
                                        _readBodyLength += readLength;
                                    }
                                }
                                catch
                                {
                                    Logger.LogError("从Socket中读取数据过程中发生异常， 包体数据长度_bodyLength： " + _bodyLength.ToString());
                                    Close();//发生读取Socket数据发生异常后，主动断开连接
                                    yield break;
                                }
                            }
                            #endregion
                            #region 处理一个完成包
                            if (_readBodyLength == _bodyLength)
                            {
                                try
                                {
                                    OnReceived(_bodyBuffer, _bodyLength);
                                }
                                catch (Exception e)
                                {
                                    Logger.LogError(this.ToString() + "  处理回包发生异常 " + e.Message);
                                }
                                //读完一个完整包
                                _bodyLength = 0;
                                _readBodyLength = 0;
                            }
                            #endregion
                        }
                    }
                }
                catch(Exception e)
                {
                    Logger.LogError("从Socket中读取数据过程中发生异常: " + e.Message);
                    Close();
                    yield break;
                }
                yield return null;
            }
        }

        private byte[] GetBodyBuffer(int bodyLen)
        {
            if(_sharedBuffer.Length < bodyLen)
            {
                int len = _sharedBuffer.Length;
                while(len < bodyLen)
                {
                    len = len * 2;
                    if(len > MAX_SHARED_BUFFER_LENGTH)
                    {
                        len = MAX_SHARED_BUFFER_LENGTH;
                        break;
                    }
                }
                _sharedBuffer = new byte[len];
                return _sharedBuffer;
            }
            return _sharedBuffer;
        }

        protected IEnumerator DaemonSend()
        {
            while(true)
            {
                if(_socket == null || _socket.Connected == false)
                {
                    Close();
                    yield break;
                }
                if (this.State == SocketState.Success && PauseSending == false)
                {
                    while (_sendQueue.Count > 0)
                    {
                        Packet packet = _sendQueue.Dequeue();
                        try
                        {
                            _socket.Send(SerializePacket(packet));
                        }
                        catch(Exception e)
                        {
                            Logger.LogError(this.ToString() + " 发送数据过程中发生异常 " + e.Message);
                            Close();
                            yield break;
                        }
                    }
                }
                yield return null;
            }
        }

        protected virtual byte[] SerializePacket(Packet packet)
        {
            packet["send_timestamp"] = TimeHelper.NowMilliseconds().ToString(); //发送时间戳
            string json = MiniJSON.Json.Serialize(packet);
            // 数据压缩
            byte[] compressedData = Zlib.Compress(json);
            // 加上头4字节包长 包长字节序转换
            byte[] header = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(compressedData.Length));
            // 组包
            byte[] data = new byte[header.Length + compressedData.Length];
            Array.Copy(header, 0, data, 0, header.Length);
            Array.Copy(compressedData, 0, data, header.Length, compressedData.Length);
            return data;
        }

        /// <summary>
        /// 成功连接时的回调
        /// </summary>
        protected abstract void OnConnected();

        /// <summary>
        /// 连接失败时回调
        /// </summary>
        protected virtual void OnConnectFailed()
        {
            _sendQueue.Clear();
            Logger.Log("<color=#ff0000>Broker Socket</color> 连接失败: " + _host + " : " + _port.ToString());
        }
        /// <summary>
        /// 处理回包
        /// </summary>
        /// <param name="content"></param>
        protected abstract void OnReceived(byte[] content, int length);
        
        public void Send(Packet packet)
        {
            if(this.State == SocketState.Disconnect || this.State == SocketState.Failed)
            {
                Reconnect();
            }
            _sendQueue.Enqueue(packet);
        }

        protected SocketState State
        {
            get
            {
                lock(_lockObj)
                {
                    return _state;
                }
            }
            set
            {
                lock(_lockObj)
                {
                    _state = value;
                }
            }
        }

        public virtual void Close()
        {
            this.State = SocketState.Disconnect;
            if(_socket != null)
            {
                _socket.Close();
                _socket = null;
            }
            SafeStopCoroutine(ref _waitCoroutine);
            SafeStopCoroutine(ref _receiveCoroutine);
            SafeStopCoroutine(ref _sendCoroutine);
        }

        protected void SafeStartCoroutine(ref Coroutine coroutineRef, IEnumerator enumerator)
        {
            if(coroutineRef != null)
            {
                StopCoroutine(coroutineRef);
            }
            coroutineRef = StartCoroutine(enumerator);
        }

        protected void SafeStopCoroutine(ref Coroutine coroutineRef)
        {
            if(coroutineRef != null)
            {
                StopCoroutine(coroutineRef);
                coroutineRef = null;
            }
        }

        protected string GetNetworkType()
        {
            switch(Application.internetReachability)
            {
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    return "Mobile";
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    return "Lan";
            }
            return "None";
        }

        public override string ToString()
        {
            return string.Format("{0} {1}:{2}", "Socket", _host, _port.ToString());
        }

        public enum SocketState
        {
            Disconnect,
            Connecting,
            Success,
            Failed
        }

    }

}
