using System;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Scripting;

/// <summary>
///     游戏启动类，根据平台选择是启动服务器还是客户端
///     此类仅为示例大概率会被更改
/// </summary>
[Preserve]
public class GameBootstrap : ClientServerBootstrap {
    private static bool IsServerPlatform => Application.platform == RuntimePlatform.LinuxServer
                                            || Application.platform == RuntimePlatform.WindowsServer
                                            || Application.platform == RuntimePlatform.OSXServer;

    public override bool Initialize(string defaultWorldName) {
        AutoConnectPort = 14747;
        //      DefaultConnectAddress = NetworkEndpoint.Parse("10.31.7.7", AutoConnectPort);
        //-p 设置端口


#if UNITY_EDITOR
        return base.Initialize(defaultWorldName);

#else
   ProcessCommandLineArgs(defaultWorldName);
        if (IsServerPlatform) {
            return CreateServerWorld(defaultWorldName) != null;
        }
        return CreateClientWorld(defaultWorldName) != null;
#endif
    }

    private void ProcessCommandLineArgs(string defaultWorldName) {
        var args = Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length; i++)
            if (args[i] == "-p") {
                i++;
                if (i >= args.Length) throw new Exception("Invalid command line arguments");

                AutoConnectPort = ushort.Parse(args[i]);
                Debug.Log("AutoConnectPort set: " + AutoConnectPort);
            }
            else if (args[i] == "-ip") {
                i++;
                if (i >= args.Length) throw new Exception("Invalid command line arguments");

                DefaultConnectAddress = NetworkEndpoint.Parse(args[i], AutoConnectPort);
                Debug.Log("defaultWorldName set: " + defaultWorldName);
            }
    }
}