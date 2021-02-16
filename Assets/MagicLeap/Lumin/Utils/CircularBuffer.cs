// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "CircularBuffer.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System.Linq;
    using System.Collections.Generic;
    public class CircularBuffer<T>
    {
        private T defaultObject;
        private List<T> buffer = null;
        private int bufferIndex = 0;
        private CircularBuffer()
        {
        }
       
        public static CircularBuffer<T> Create(T defaultObject, int amount)
        {
            CircularBuffer<T> circularBuffer = new CircularBuffer<T>();
            circularBuffer.defaultObject = defaultObject;
            circularBuffer.buffer = Enumerable.Repeat(defaultObject, amount).ToList();
            return circularBuffer;
        }

        public static CircularBuffer<T> Create(T defaultObject, params T[] objects)
        {
            CircularBuffer<T> circularBuffer = new CircularBuffer<T>();
            circularBuffer.defaultObject = defaultObject;
            circularBuffer.buffer = objects.ToList();
            return circularBuffer;
        }

        public T Get()
        {
            if (bufferIndex >= buffer.Count)
            {
                bufferIndex = 0;
            }

            return buffer[bufferIndex++];
        }

        public void Add(params T[] objects)
        {
            foreach (T obj in objects)
            {
                this.buffer.Add(obj);
            }
        }
    }
}
