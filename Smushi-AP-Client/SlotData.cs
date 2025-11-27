using System;
using System.Collections.Generic;

namespace Smushi_AP_Client
{
    public enum Goal
    {
        SmushiGoHome,
        SmushiSaveTree
    }
    
    public class SlotData
    {
        public readonly Goal Goal = Goal.SmushiGoHome;
        public SlotData(Dictionary<string, object> slotDict)
        {
            foreach (var x in slotDict) 
                Console.WriteLine($"{x.Key} {x.Value}");
            Goal = (Goal)(long)slotDict["Goal"];
        }
    }
}