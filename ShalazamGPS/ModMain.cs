using MelonLoader;
using ShalazamGPS.Sockets;
using WebSocketSharp.Server;

namespace ShalazamGPS;

public class ModMain : MelonMod
{
    private WebSocketServer? _webSocketServer;
    public static string WebsocketAddress;
    public static int WebsocketPort;
    
    public const string PluginVersion = "1.0.0";
    
    public override void OnInitializeMelon()
    {
        var category = MelonPreferences.CreateCategory("ShalazamGPS");
        WebsocketAddress = category.CreateEntry("WebsocketUrl", "ws://localhost").Value;
        WebsocketPort = category.CreateEntry("SocketPort", 3000).Value;
        
        category.SaveToFile(false);
        
        for (var i = 0; i < 100; i++)
        {
            try
            {
                _webSocketServer = new WebSocketServer($"{WebsocketAddress}:{WebsocketPort + i}");
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
}