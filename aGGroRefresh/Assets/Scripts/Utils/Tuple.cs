using System;
using System.ComponentModel;
using UnityEngine;

namespace Utils
{
    [System.Serializable]
    public class Tuple<K, V>
    {
        public K key;
        public V value;

        public Tuple(K key, V value)
        {
            this.key = key;
            this.value = value;
        }

        public Tuple(){}
    }
}