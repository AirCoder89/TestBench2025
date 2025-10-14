using System.Collections.Generic;
using UnityEngine;

namespace TestBench2025.Core.Systems
{
    public class ObjectPool<T> where T : Component
    {
        private readonly Queue<T> _pool = new Queue<T>();
        private readonly T _prefab;
        private readonly Transform _parent;

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            WarmUp(initialSize);
        }

        private void WarmUp(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var obj = CreateNew();
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        private T CreateNew()
        {
            var instance = Object.Instantiate(_prefab, _parent);
            instance.gameObject.SetActive(false);
            return instance;
        }

        public T Get()
        {
            if (_pool.Count == 0)
                _pool.Enqueue(CreateNew());

            var obj = _pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Return(T obj)
        {
            if (obj == null) return;
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }

        public void Clear()
        {
            _pool.Clear();
        }
    }
}