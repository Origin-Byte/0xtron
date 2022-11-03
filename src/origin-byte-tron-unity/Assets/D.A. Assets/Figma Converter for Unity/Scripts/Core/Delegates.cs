using System.Collections;
using UnityEngine;

namespace DA_Assets.FCU.Core
{
    public delegate object DynamicCoroutine(IEnumerator routine);
    public delegate bool GetGameViewSize(out Vector2 size);
    public delegate bool SetGameViewSize(Vector2 size);
}