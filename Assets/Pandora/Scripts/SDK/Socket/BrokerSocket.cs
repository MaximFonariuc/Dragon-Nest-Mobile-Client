using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Packet = System.Collections.Generic.Dictionary<string, object>;

namespace com.tencent.pandora
{
    internal class BrokerSocket : AbstractTcpClient
    {
        /// <summary>
        /// 服务器主动给客户端推送消息的类型
        /// </summary>
        public const int PUSH = 1;
        /// <summary>
        /// 服务器给客户端请求回包的消息类型
        /// </summary>
        public const int PULL = 2;
        /// <summary>
        /// 心跳包时间间隔
        /// </summary>
        public const int HEARTBEAT_INTERVAL = 30;

        /// <summary>
        /// 发送心跳后5s内没收到回包则认为断网，进入重连状态
        /// </summary>
        public const int HEARTEART_TIMEOUT = 5;
        
        /// <summary>
        /// 为防止服务器雪崩，在重连状态下，最多重连5次，后续走发消息时重连
        /// </summary>
        private int _retryCount = 0;
        /// <summary>
        /// 登录
        /// </summary>
        public const int C2S_LOGIN = 1001;
        public const int S2C_LOGIN = 1001;

        public const int C2S_HEARTBEAT = 1002;
        public const int S2C_HEARTBEAT = 1002;

        public const int S2C_PUSH = 1003;

        /// <summary>
        /// 拉取活动信息
        /// </summary>
        public const int C2S_ACTION = 9000;
        public const int S2C_ACTION = 9000;

        /// <summary>
        /// 经分上报
        /// </summary>
        public const int C2S_STATS = 5001;
        public const int S2C_STATS = 5001;

        private bool _hasHeartbeatResponse;
        private uint _heartbeatCallId = 0;
        private Coroutine _heartbeatCoroutine;
        private float _lastHeartbeatTime;
        private float _lastReconnectTime;

        private Coroutine _reconnectCoroutine;

        protected override void OnConnected()
        {
            if(Pandora.Instance.IsDebug == true)
            {
                Logger.LogInfo("<color=#00ff00>Broker Socket</color> 连接成功: " + _host + _address + _port.ToString());
            }
            SendLogin();
        }

        //注意：
        //此处的outer是指描述Broker此次请求返回的状态描述，可能出错，属于底层的错误。
        //outer的body字段是描述，活动请求返回的内容，可能含有业务层面上的出错信息，比如没有活动，此部分透传给Lua
        protected override void OnReceived(byte[] content, int length)
        {
            string json = Zlib.Decompress(content, length);
            if(Pandora.Instance.IsDebug == true)
            {
                Logger.Log("Broker 收到回包: ");
                Logger.Log(json);
            }
            Dictionary<string, System.Object> outerDict = MiniJSON.Json.Deserialize(json) as Dictionary<string, System.Object>;
            int type = (int)(long)outerDict["type"];
            int commandId = (int)(long)outerDict["cmd_id"];
            uint callId = (uint)(long)outerDict["seq_id"];

            if(type == PULL)
            {
                HandlePullMessage(callId, commandId, outerDict["body"] as string);
            }
            else if(type == PUSH)
            {
                HandlePushMessage(callId, commandId, outerDict["body"] as string);
            }
        }

        private void HandlePullMessage(uint callId, int commandId, string message)
        {
            _hasHeartbeatResponse = true; //收到任何服务器回包都算心跳有返回
            switch (commandId)
            {
                case S2C_LOGIN:
                    EnterHeartbeatState();
                    break;
                case S2C_HEARTBEAT:
                    break;
                case S2C_ACTION:
                    CSharpInterface.ExecuteLuaCallback(callId, message);
                    break;
            }
        }

        private void HandlePushMessage(uint callId, int commandId, string message)
        {
            switch (commandId)
            {
                case S2C_PUSH:
                    CSharpInterface.ExecuteLuaCallback(0, message);
                    break;
            }
        }

        private void EnterHeartbeatState()
        {
            SafeStopCoroutine(ref _reconnectCoroutine);
            _lastHeartbeatTime = Time.realtimeSinceStartup;
            SafeStartCoroutine(ref _heartbeatCoroutine, DaemonHeartbeat());
        }

        private void EnterReconnectState()
        {
            _retryCount = 0;
            SafeStopCoroutine(ref _heartbeatCoroutine);
            SafeStartCoroutine(ref _reconnectCoroutine, DaemonReconnect());
        }

        public void SendLogin()
        {
            string body = GenerateBody();
            Packet packet = GeneratePacket(_heartbeatCallId++, body, C2S_LOGIN);
            if(packet == EMPTY_PACKET)
            {
                return;
            }
            if (Pandora.Instance.IsDebug == true)
            {
                Logger.Log("Broker 发送信息，命令字： " + C2S_LOGIN + " 　内容：　");
                Logger.Log(body);
            }
            Send(packet);
        }

        public void SendHeartbeat()
        {
            string body = GenerateBody();
            Packet packet = GeneratePacket(_heartbeatCallId++, body, C2S_HEARTBEAT);
            if(packet == EMPTY_PACKET)
            {
                return;
            }
            if (Pandora.Instance.IsDebug == true)
            {
                Logger.Log("Broker 发送信息，命令字： " + C2S_HEARTBEAT + " 　内容：　");
                Logger.Log(body);
            }
            Send(packet);
        }

        private string GenerateBody()
        {
            UserData userData = Pandora.Instance.GetUserData();
            Dictionary<string, System.Object> dict = new Dictionary<string, System.Object>();
            dict["open_id"] = userData.sOpenId;
            dict["app_id"] = userData.sAppId;
            dict["sarea"] = userData.sArea;
            dict["splatid"] = userData.sPlatID;
            dict["spartition"] = userData.sPartition;
            dict["access_token"] = userData.sAccessToken;
            dict["acctype"] = userData.sAcountType;
            return MiniJSON.Json.Serialize(dict);
        }

        private IEnumerator DaemonHeartbeat()
        {
            while(true)
            {
                float delta = Time.realtimeSinceStartup - _lastHeartbeatTime;
                if(_hasHeartbeatResponse == false && delta >= HEARTEART_TIMEOUT)
                {
                    EnterReconnectState();
                    yield break;
                }
                else if(delta >= HEARTBEAT_INTERVAL)
                {
                    _lastHeartbeatTime = Time.realtimeSinceStartup;
                    _hasHeartbeatResponse = false;
                    SendHeartbeat();
                }
                yield return null;
            }
        }

        private IEnumerator DaemonReconnect()
        {
            while(true)
            {
                float delta = Time.realtimeSinceStartup - _lastReconnectTime;
                if(delta > GetReconnectInterval(_retryCount))
                {
                    _retryCount = (_retryCount + 1) % 4;
                    _lastReconnectTime = Time.realtimeSinceStartup;
                    Reconnect();
                }
                yield return null;
            }
        }

        private int GetReconnectInterval(int retryCount)
        {
            int baseValue = (1 << (retryCount + 3));
            return baseValue + UnityEngine.Random.Range(0, baseValue);
        }

        public void Send(uint callId, string requestBody, int commandId)
        {
            Packet packet = GeneratePacket(callId, requestBody, commandId);
            if(packet == EMPTY_PACKET)
            {
                return;
            }
            if (Pandora.Instance.IsDebug == true)
            {
                Logger.Log("Broker 发送信息，CommandId： " + commandId + " 　CallId：　" + callId.ToString());
                Logger.Log(requestBody);
            }
            Send(packet);
        }

        private Packet GeneratePacket(uint callId, string body, int commandId, int type = 1, int moduleId = 10)
        {
            UserData userData = Pandora.Instance.GetUserData();
            if(string.IsNullOrEmpty(userData.sOpenId) == true)
            {
                return EMPTY_PACKET;
            }
            Packet dict = new Packet();
            dict["seq_id"] = callId;                        // 请求的序列号
            dict["cmd_id"] = commandId;                     // 发送broker的命令字，目前有两类: 5001 经分上报  9000 lua请求
            dict["type"] = type;                            // 1 表示请求类型, 2 表示响应类型，默认为请求
            dict["from_ip"] = "10.0.0.108";                 // 来源IP
            dict["process_id"] = 1;                         // 来源进程
            dict["mod_id"] = moduleId;                      // 来源模块编号 10 表示sdk模块
            dict["version"] = userData.sSdkVersion;         // 版本号
            dict["body"] = body;                            // 要下发的json数据
            dict["app_id"] = userData.sAppId;               // 游戏appid
            dict["network_type"] = GetNetworkType();        //网络类型
            //dict["send_timestamp"] = TimeHelper.NowMilliseconds().ToString(); //发送时间戳，调整到发送时刻赋值
            return dict;
        }

        public override void Close()
        {
            SafeStopCoroutine(ref _heartbeatCoroutine);
            SafeStopCoroutine(ref _reconnectCoroutine);
            base.Close();

        }

    }
}
