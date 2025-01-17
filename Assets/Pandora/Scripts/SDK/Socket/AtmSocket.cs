using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using UnityEngine;
using Packet = System.Collections.Generic.Dictionary<string, object>;

namespace com.tencent.pandora
{
    /// <summary>
    /// AtmSocket为上报日志所用，重要性略低一点
    /// 为尽量减少对游戏的干扰，所以AtmSocket不做心跳机制
    /// 为避免AtmSocket太长时间（超过15分钟）没有发送信息给服务器而导致链接断开，
    /// 设置为当距离上次成功收到回包时间超过5*60s后发送信息前，自动重连一次服务器。
    /// </summary>
    internal class AtmSocket : AbstractTcpClient
    {
        //距离上次发包时间超过该值后，自动重连
        public const int RECONNECT_INTERVAL = 10;

        //上报包字节长度最大为4K
        public const int MAX_PACKET_LENGTH = 4 * 1024;

        public const int C2S_LOG = 5000;
        private int _callId = 0;

        private float _lastReceivedTime;
        //可重用对象
        private static Dictionary<string, System.Object> _bodyDict = new Dictionary<string, object>();
        private static List<Dictionary<string, System.Object>> _extendObjList = new List<Dictionary<string, object>>();
        private static Dictionary<string, System.Object> _idObj = new Dictionary<string, object>();
        private static Dictionary<string, System.Object> _typeObj = new Dictionary<string, object>();
        private static Dictionary<string, System.Object> _valueObj = new Dictionary<string, object>();

        private string _systemInfo;

        /// <summary>
        /// type 为tnm2上报类型值
        /// 0：叠加类型
        /// 1：平均值类型
        /// 2：告警类型
        /// 默认为叠加类型
        /// </summary>
        /// <param name="error"></param>
        /// <param name="id"></param>
        public void Report(string error, int id = 0, int type = 0)
        {
            try
            {
                if(Pandora.Instance.IsDebug == true)
                {
                    Logger.Log("上报ATM: " + error + " CallId: " + _callId.ToString());
                }
                string body = GenerateBody(error, id, type);
                Packet packet = GeneratePacket(body);
                if(packet == EMPTY_PACKET)
                {
                    return;
                }
                if((Time.realtimeSinceStartup - _lastReceivedTime) > RECONNECT_INTERVAL)
                {
                    if(this.State == SocketState.Success)
                    {
                        Close();
                    }
                }
                Send(packet);
            }
            catch(Exception e)
            {
                Logger.LogError("上报流水日志失败：  " + error + " " + e.Message);
            }
        }

        private string GenerateBody(string message, int id, int type)
        {
            UserData userData = Pandora.Instance.GetUserData();
            _bodyDict["str_openid"] = userData.sOpenId;
            _bodyDict["spartition"] = userData.sPartition;
            _bodyDict["splatid"] = userData.sPlatID;
            _bodyDict["str_userip"] = "10.0.0.1";
            _bodyDict["str_respara"] = message + GetSystemInfo();
            _bodyDict["uint_report_type"] = 2;
            _bodyDict["uint_toreturncode"] = 1;
            _bodyDict["uint_log_level"] = 2;
            _bodyDict["str_openid"] = userData.sOpenId;
            _bodyDict["sarea"] = userData.sArea;
            _bodyDict["str_hardware_os"] = "unity";
            _bodyDict["str_sdk_version"] = userData.sSdkVersion;
            _bodyDict["uint_serialtime"] = TimeHelper.NowSeconds();
            _bodyDict["extend"] = GetExtendObjList(id, type, message);
            return MiniJSON.Json.Serialize(_bodyDict);
        }

        private List<Dictionary<string, System.Object>> GetExtendObjList(int id, int type, string content)
        {
            if(_extendObjList.Count == 0)
            {
                _idObj["name"] = "attr_id_0";
                _idObj["value"] = id;
                _extendObjList.Add(_idObj);

                _typeObj["name"] = "attr_type_0";
                _typeObj["value"] = type;
                _extendObjList.Add(_typeObj);

                _valueObj["name"] = "attr_value_0";
                _valueObj["value"] = content;
                _extendObjList.Add(_valueObj);
            }
            _idObj["value"] = id.ToString();
            _typeObj["value"] = type.ToString();

            _valueObj["value"] = "1";
            //类型2是字符型告警内容
            if(type == 2)
            {
                _valueObj["value"] = content + GetSystemInfo();
            }
            return _extendObjList;
        }

        private string GetSystemInfo()
        {
            if(string.IsNullOrEmpty(_systemInfo) == true)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("|IsProductEnvironment="); sb.Append(PandoraSettings.IsProductEnvironment);
                sb.Append("&Platform="); sb.Append(Application.platform.ToString());
                sb.Append("&DeviceModel="); sb.Append(SystemInfo.deviceModel);
                sb.Append("&DeviceType="); sb.Append(SystemInfo.deviceType);
                sb.Append("&OperatingSystem="); sb.Append(SystemInfo.operatingSystem);
                sb.Append("&ProcessorType="); sb.Append(SystemInfo.processorType);
                sb.Append("&SystemMemorySize="); sb.Append(SystemInfo.systemMemorySize);
                _systemInfo = sb.ToString();
            }
            return _systemInfo;
        }
        
        private Packet GeneratePacket(string body)
        {
            UserData userData = Pandora.Instance.GetUserData();
            if(string.IsNullOrEmpty(userData.sOpenId) == true)
            {
                return EMPTY_PACKET;
            }
            Packet packet = new Packet();
            packet["seq_id"] = _callId++;                     // 请求的序列号
            packet["cmd_id"] = C2S_LOG;                       // 上报命令字 5000
            packet["type"] = 1;                               // 1 表示请求类型
            packet["from_ip"] = "10.0.0.108";                 // 来源IP
            packet["process_id"] = 1;                         // 来源进程
            packet["mod_id"] = 10;                            // 来源模块编号 10 表示sdk模块
            packet["version"] = userData.sSdkVersion;         // 版本号
            packet["body"] = body;                            // 要上报json数据
            packet["app_id"] = userData.sAppId;               // 游戏appid
            packet["network_type"] = GetNetworkType();
            return packet;
        }

        protected override void OnConnected()
        {
            _lastReceivedTime = Time.realtimeSinceStartup;
            if(Pandora.Instance.IsDebug == true)
            {
                Logger.LogInfo("<color=#00ff00>Atm Socket</color> 连接成功: " + _host + _address + _port.ToString());
            }
        }

        protected override void OnReceived(byte[] content, int length)
        {
            _lastReceivedTime = Time.realtimeSinceStartup;
            if(Pandora.Instance.IsDebug == true)
            {
                Logger.Log("收到ATM上报回包： " + Zlib.Decompress(content, length));
            }
        }

        protected override byte[] SerializePacket(Packet packet)
        {
            byte[] data = base.SerializePacket(packet);
            if(data.Length > MAX_PACKET_LENGTH)
            {
                Logger.LogWarning("上报包字节长度超过4K，当前包长： " + data.Length);
            }
            return data;
        }

    }
}
