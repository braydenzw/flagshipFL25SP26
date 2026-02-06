using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    // Extension methods to easily allow Enqueueing and Dequeueing to the TimeTravel list
    public static void Enqueue(this LinkedList<PlayerState> list, PlayerState playerState)
    {
        list.AddLast(playerState);
    }

    public static void Dequeue(this LinkedList<PlayerState> list)
    {
        list.RemoveFirst();
    }
    
    // 
    public static void ClearAfterPoint(this LinkedList<PlayerState> list, int idx)
    {
        while (list.Count > idx)
        {
            list.RemoveLast();
        }

    }
    
    
}
