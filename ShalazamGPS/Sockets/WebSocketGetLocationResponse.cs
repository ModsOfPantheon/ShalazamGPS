namespace ShalazamGPS.Sockets;

public class WebSocketGetLocationResponse
{
    public WebSocketLocation Loc { get; set; }
    public IEnumerable<WebSocketGroupMember> Group { get; set; }
}

public class WebSocketGroupMember
{
    public float? LocX { get; set; }
    public float? LocY { get; set; }
    public float? LocZ { get; set; }
    public float? Heading { get; set; }
    public string? Name { get; set; }
}