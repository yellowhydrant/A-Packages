using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public interface ISaveable<T>
    {
        public string fileName { get; }
        public T SaveData();
        public void LoadData(T data);
    }

    public interface ISaveable
    {
        public string fileName { get; }
        public object SaveData();
        public void LoadData(object data);
    }
}
