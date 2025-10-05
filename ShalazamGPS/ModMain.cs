using MelonLoader;
using ShalazamGPS.Sockets;
using UnityEngine;
using WebSocketSharp.Server;

namespace ShalazamGPS;

public class ModMain : MelonMod
{
    private WebSocketServer? _webSocketServer;
    public static string WebsocketAddress;
    public static int WebsocketPort;
    public static int RefreshInterval;
    
    public const string PluginVersion = "1.0.0";
    
    public override void OnInitializeMelon()
    {
        var category = MelonPreferences.CreateCategory("ShalazamGPS");
        WebsocketAddress = category.CreateEntry("WebsocketUrl", "ws://localhost").Value;
        WebsocketPort = category.CreateEntry("SocketPort", 3000).Value;
        RefreshInterval = category.CreateEntry("RefreshInterval", 100).Value;
        
        category.SaveToFile(false);

        int preferredWebsocketPort = WebsocketPort;
        for (var i = 0; i < 100; i++)
        {
            try
            {
                WebsocketPort = preferredWebsocketPort + i;

                _webSocketServer = new WebSocketServer($"{WebsocketAddress}:{WebsocketPort}");
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

    public static void OpenGps()
    {
        Application.OpenURL(
            $"https://shalazam.info/maps/1?gps_websocket_url={WebsocketAddress}:{WebsocketPort}&gps_websocket_interval={RefreshInterval}&zoom=8&x={(int)Globals.LocalPlayer!.transform.position.x}&y={(int)Globals.LocalPlayer.transform.position.z}");
    }
}