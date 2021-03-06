using UnityEngine;

namespace A
{
    public abstract class ASingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object Lock = new object();

        public static T instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance != null) return _instance;

                    _instance = (T)FindObjectOfType(typeof(T));

                    if (_instance != null) return _instance;

                    Debug.LogErrorFormat("[Singleton]: Failed to find an instance of '{0}', please create an instance in the scene.",
                        typeof(T));

                    return null;
                }
            }
        }
    }
}