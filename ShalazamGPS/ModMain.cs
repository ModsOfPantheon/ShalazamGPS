using MelonLoader;
using ShalazamGPS.Sockets;
using UnityEngine;
using WebSocketSharp.Server;

namespace ShalazamGPS;

public class ModMain : MelonMod
{
    private WebSocketServer? _webSocketServer;
    private static string _websocketAddress;
    private static int _websocketPort;
    
    public const string PluginVersion = "1.1.0";
    
    public override void OnInitializeMelon()
    {
        var category = MelonPreferences.CreateCategory("ShalazamGPS");
        _websocketAddress = category.CreateEntry("WebsocketUrl", "ws://localhost").Value;
        _websocketPort = category.CreateEntry("SocketPort", 3000).Value;
        
        category.SaveToFile(false);

        var preferredWebsocketPort = _websocketPort;
        for (var i = 0; i < 100; i++)
        {
            try
            {
                _websocketPort = preferredWebsocketPort + i;

                _webSocketServer = new WebSocketServer($"{_websocketAddress}:{_websocketPort}");
                _webSocketServer.AddWebSocketService<WebSocketRequestBehaviour>("/");
                _webSocketServer.Start();

                if (_webSocketServer.IsListening)
                {
                    MelonLogger.Msg($"Started websocket server on port {_webSocketServer.Port}");
                    break;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        if (_webSocketServer == null || _webSocketServer.IsListening == false)
        {
            MelonLogger.Error("Failed to open websocket server :(");
        }
    }

    public override void OnApplicationQuit()
    {
        CloseWebsocket();
    }

    public override void OnDeinitializeMelon()
    {
        CloseWebsocket();
    }

    private void CloseWebsocket()
    {
        _webSocketServer?.RemoveWebSocketService("/");

        _webSocketServer?.Stop();
    }

    public static void OpenGps()
    {
        Application.OpenURL(
            $"https://shalazam.info/maps/1?gps_websocket_url={_websocketAddress}:{_websocketPort}&zoom=8&x={(int)Globals.LocalPlayer!.transform.position.x}&y={(int)Globals.LocalPlayer.transform.position.z}");
    }
}