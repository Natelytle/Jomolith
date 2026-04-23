// Copyright (c) 2026 Natelytle
// 
// This file is part of Jomolith.
// 
// Jomolith is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Jomolith is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public
// License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Jomolith. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace Jomolith.Core.Events;

public class EventBus
{
    private readonly Dictionary<Type, List<WeakReference>> subscribers = new Dictionary<Type, List<WeakReference>>();

    /// <summary>
    ///     Creates a weak subscription to this event, using the provided handler.
    /// </summary>
    /// <param name="handler">The method that will handle the event</param>
    /// <typeparam name="T">The event type to handle</typeparam>
    public void Subscribe<T>(Action<T> handler)
    {
        Type eventType = typeof(T);

        if (!subscribers.ContainsKey(eventType))
            subscribers[eventType] = new List<WeakReference>();

        subscribers[eventType].Add(new WeakReference(handler));
    }

    public void Publish<T>(T eventToPublish)
    {
        Type eventType = typeof(T);

        if (!subscribers.TryGetValue(eventType, out List<WeakReference>? eventSubs))
            return;

        for (int i = eventSubs.Count - 1; i >= 0; i--)
        {
            var subscriber = eventSubs[i];

            if (!subscriber.IsAlive)
            {
                eventSubs.RemoveAt(i);

                continue;
            }

            if (subscriber.Target is Action<T> handler)
                handler(eventToPublish);
        }
    }
}