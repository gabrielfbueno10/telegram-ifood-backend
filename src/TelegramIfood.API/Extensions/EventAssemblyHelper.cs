using MediatR;
using System.Reflection;
using TelegramIfood.Events;

namespace TelegramIfood.API.Extensions;

public static class EventAssemblyHelper
{
    public static Type[] Types = typeof(AssemblyEventMarker).Assembly.GetTypes();
    public static Dictionary<string, Type> Events
    {
        get => _events is null ? (_events = TypesBuild()) : _events;
    }
    private static Dictionary<string, Type> _events;
    private static Dictionary<string, Type> TypesBuild()
    {
        var result = new Dictionary<string, Type>();

        foreach (var type in Types)
            if (!result.ContainsKey(type.Name))
                result.Add(type.Name, type);

        return result;
    }

    public static Dictionary<string, EventType> EventTypes
    {
        get => _eventTypes is null ? (_eventTypes = BuildEventTypes()) : _eventTypes;
    }
    private static Dictionary<string, EventType> _eventTypes;
    private static Dictionary<string, EventType> BuildEventTypes()
    {
        var result = new Dictionary<string, EventType>();

        foreach (var type in Types)
            if (!result.ContainsKey(type.Name))
                result.Add(type.Name, type.GetInterfaces().Contains(typeof(INotification)) ? EventType.Event : EventType.Command);

        return result;
    }
}


public enum EventType
{
    Event,
    Command
}