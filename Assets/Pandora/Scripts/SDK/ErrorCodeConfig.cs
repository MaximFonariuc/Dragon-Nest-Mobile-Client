using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.tencent.pandora
{
    internal class ErrorCodeConfig
    {
        /// <summary>
        /// tnm2累加类型
        /// </summary>
        public const int TNM2_TYPE_ACCUMULATION = 0;
        /// <summary>
        /// tnm2平均值类型
        /// </summary>
        public const int TNM2_TYPE_AVERAGE = 1;
        /// <summary>
        /// tnm2字符型告警类型
        /// </summary>
        public const int TNM2_TYPE_LITERALS = 2;


        #region 累加类型上报代码
        /// <summary>
        /// Assetbundle资源解包失败
        /// </summary>
        public const int ASSET_PARSE_FAILED = 10217582;
        /// <summary>
        /// 资源Md5码校验失败
        /// </summary>
        public const int MD5_VALIDATE_FAILED = 10217583;
        /// <summary>
        /// 文件写入本地失败
        /// </summary>
#if UNITY_IOS
        public const int FILE_WRITE_FAILED = 10217585;
#else
        public const int FILE_WRITE_FAILED = 10217584;
#endif
        /// <summary>
        /// 资源加载失败
        /// </summary>
        public const int ASSET_LOAD_FAILED = 10217586;
        /// <summary>
        /// 资源Meta文件读取失败
        /// </summary>
        public const int META_READ_FAILED = 10217587;
        /// <summary>
        /// 资源Meta文件更新失败
        /// </summary>
#if UNITY_IOS
        public const int META_WRITE_FAILED = 10217589;
#else
        public const int META_WRITE_FAILED = 10217588;
#endif
        /// <summary>
        /// 访问失败且超过最大重试次数
        /// </summary>
        public const int CGI_TIMEOUT = 10217590;
        /// <summary>
        /// Lua执行游戏传递的消息失败
        /// </summary>
        public const int GAME_2_PANDORA_EXCEPTION = 10217591;
        /// <summary>
        /// 执行Lua回调发生异常
        /// </summary>
        public const int EXECUTE_LUA_CALLBACK_EXCEPTION = 10217592;
        /// <summary>
        /// Lua脚本执行发生异常
        /// </summary>
        public const int LUA_SCRIPT_EXCEPTION = 10217593;
        /// <summary>
        /// Lua脚本文件解析发生异常
        /// </summary>
        public const int LUA_DO_FILE_EXCEPTION = 10217594;
        /// <summary>
        /// 开始执行模块入口Lua文件
        /// </summary>
        public const int EXECUTE_ENTRY_LUA = 10217595;
        /// <summary>
        /// 游戏执行Lua消息发生异常
        /// </summary>
        public const int PANDORA_2_GAME_EXCEPTION = 10217596;
        /// <summary>
        /// 创建面板时已存在同名面板
        /// </summary>
        public const int SAME_PANEL_EXISTS = 10217597;
        /// <summary>
        /// 面板的父节点不存在
        /// </summary>
        public const int PANEL_PARENT_INEXISTS = 10217598;
        /// <summary>
        /// Cookie写入失败
        /// </summary>
        public const int COOKIE_WRITE_FAILED = 10217599;
        /// <summary>
        /// Cookie读取失败
        /// </summary>
        public const int COOKIE_READ_FAILED = 10217600;
        /// <summary>
        /// 删除文件失败
        /// </summary>
        public const int DELETE_FILE_FAILED = 10217601;
        /// <summary>
        /// 记录用户主动重连请求
        /// </summary>
        public const int START_RELOAD = 10217602;
#endregion

#region 字符型告警上报代码
        /// <summary>
        /// Lua脚本报错详情，使用tnm2字符型告警内容上报
        /// </summary>
        public const int LUA_SCRIPT_EXCEPTION_DETAIL = 10217603;
        /// <summary>
        /// CGI超时错误详情
        /// </summary>
        public const int CGI_TIMEOUT_DETAIL = 10217604;

        /// <summary>
        /// 游戏处理Pandora消息回调函数异常详情
        /// </summary>
        public const int PANDORA_2_GAME_EXCEPTION_DETAIL = 10259462;
        #endregion
    }
}
