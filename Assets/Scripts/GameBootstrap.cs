using System;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

/// <summary>
/// 游戏启动类，根据平台选择是启动服务器还是客户端
/// 此类仅为示例大概率会被更改
/// </summary>
[UnityEngine.Scripting.Preserve]
public class GameBootstrap : ClientServerBootstrap {
    private static bool IsServerPlatform => Application.platform == RuntimePlatform.LinuxServer
                                            || Application.platform == RuntimePlatform.WindowsServer
                                            || Application.platform == RuntimePlatform.OSXServer;

    public override bool Initialize(string defaultWorldName) {
        AutoConnectPort = 14747;
        //-p 设置端口
        ProcessCommandLineArgs(defaultWorldName);
        

#if UNITY_EDITOR
        base.Initialize(defaultWorldName);
        return true;
#else
        if (IsServerPlatform) {
            return CreateServerWorld(defaultWorldName) != null;
        }
        return CreateClientWorld(defaultWorldName) != null;
#endif
    }

    private void ProcessCommandLineArgs(string defaultWorldName) {
        var args = System.Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length; i++) {
            if (args[i] == "-p") {
                i++;
                if (i >= args.Length) {
                    throw new Exception("Invalid command line arguments");
                }

                AutoConnectPort = ushort.Parse(args[i + 1]);
                Debug.Log("AutoConnectPort set: " + AutoConnectPort);
            }
        }
    }
}