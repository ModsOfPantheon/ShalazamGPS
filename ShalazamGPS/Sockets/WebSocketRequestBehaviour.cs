using System.Text.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ShalazamGPS.Sockets;

public class WebSocketRequestBehaviour : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {
        var request = JsonSerializer.Deserialize<WebSocketRequest>(e.Data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
        });
        
        if (request.Request == "gps")
        {
            if (Globals.LocalPlayer == null)
            {
                return;
            }
            
            var playerPos = Globals.LocalPlayer?.transform.position;
            
            var response = new WebSocketGetLocationResponse
            {
                Loc = new WebSocketLocation
                {
                    ClientState = Globals.LocalPlayer != null ? "LoggedIn" : "NotLoggedIn",
                    LocX = playerPos != null ? MathF.Round(playerPos.Value.x, 2) : null,
                    LocY = playerPos != null ? MathF.Round(playerPos.Value.y, 2) : null,
                    LocZ = playerPos != null ? MathF.Round(playerPos.Value.z, 2) : null,
                    Heading = Globals.LocalPlayer != null ? Globals.LocalPlayer.transform.eulerAngles.y : null
                }
            };

            Send(JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
            }));
        }
    }
}