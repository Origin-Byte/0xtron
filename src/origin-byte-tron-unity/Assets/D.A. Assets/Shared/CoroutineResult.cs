using DA_Assets.FCU.Model;

namespace DA_Assets.Shared
{
    public class CoroutineResult<T>
    {
        public bool Success { get; set; }
        public T Result { get; set; }
        public FigmaError Error { get; set; }
    }

    public delegate void Return<T>(CoroutineResult<T> coroutineResult);
}