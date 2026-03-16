using System.Text.Json;
using Il2Cpp;
using Il2CppViNL;
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
            
            var localPos = Globals.LocalPlayer?.transform.position;

            var groupPayload = new List<WebSocketGroupMember>();
            
            var group = Globals.LocalPlayer?.Group.Current;
            if (group != null)
            {
                var members = group.members;
                foreach (var member in members)
                {
                    if (member.EntityNetworkId.Value == Globals.LocalPlayer?.NetworkId.Value)
                    {
                        continue;
                    }
                    
                    var entity = member.GetEntity(group.scene);
                    if (entity == null)
                    {
                        continue;
                    }
                    
                    var player = entity.Cast<EntityPlayerGameObject>();
                    var playerPos = player?.transform.position;
                    groupPayload.Add(new WebSocketGroupMember
                    {
                        Heading = player != null ? player.transform.eulerAngles.y : null,
                        LocX = playerPos != null ? MathF.Round(playerPos.Value.x, 2) : null,
                        LocZ = playerPos != null ? MathF.Round(playerPos.Value.z, 2) : null,
                        Name = player != null ? player.info.DisplayName : null
                    });
                }
            }
            
            var response = new WebSocketGetLocationResponse
            {
                Loc = new WebSocketLocation
                {
                    ClientState = Globals.LocalPlayer != null ? "LoggedIn" : "NotLoggedIn",
                    LocX = localPos != null ? MathF.Round(localPos.Value.x, 2) : null,
                    LocZ = localPos != null ? MathF.Round(localPos.Value.z, 2) : null,
                    Heading = Globals.LocalPlayer != null ? Globals.LocalPlayer.transform.eulerAngles.y : null
                },
                Group = groupPayload
            };

            Send(JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
            }));
        }
    }
}