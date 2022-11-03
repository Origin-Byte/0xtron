using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.Shared
{
    public class SlowCycles
    {
        public static IEnumerator ForEach<T>(IList<T> source, float iterationTimeout, Action<T> body)
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (iterationTimeout != 0)
                {
                    yield return new WaitForSecondsRealtime(iterationTimeout);
                }

                body.Invoke(source[i]);
            }
        }
    }
}