using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

public static class EventFactory
{
    private static Dictionary<AllEvents, Type> EventDictionary;
    private static bool isEventsInitialized => EventDictionary != null;

    public static void InitializeCardFactory()
    {
        //if effect factory is not yet initialized, proceed
        if (isEventsInitialized)
        {
            return;
        }

        //scan through projects for all classes that are of type Card but not abstract
        //only children of Card wil be taken
        var effectTypes = Assembly.GetAssembly(typeof(EventAbstractClass)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(EventAbstractClass)));

        //Dictionary for getting the effects by enum
        EventDictionary = new Dictionary<AllEvents, Type>();

        //input Card types in dictionary with their AllCards enum as teir key
        foreach (var type in effectTypes)
        {
            //will ignore jigsaws and enemyActions sice they are derived from BaseCardEffect
            var tempEffect = Activator.CreateInstance(type) as EventAbstractClass;
            EventDictionary.Add(tempEffect.eventEnum, type);
        }

    }


    public static EventAbstractClass GetEvent(AllEvents eventKey)
    {
        //tried putting Initialize trigger in CombatManager
        //InitializeCardFactory();

        //does not activate effects that has Jigsaw key
        //JigsawFactory has the effect for that
        if (EventDictionary.ContainsKey(eventKey))
        {
            Type type = EventDictionary[eventKey];
            var eventType = Activator.CreateInstance(type) as EventAbstractClass;
            return eventType;
        }
        return null;
    }
}
