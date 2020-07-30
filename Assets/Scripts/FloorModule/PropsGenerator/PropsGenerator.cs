using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FloorModule.PropsGenerator
{
    public abstract class PropsGenerator : MonoBehaviour
    {
        private const byte AttemptNumber = 5;

        private readonly Dictionary<byte, GameObject[]> _instances =
            new Dictionary<byte, GameObject[]>();

        protected Dictionary<byte, PropsScheme> Schemes;

        protected abstract void InitSchemes();

        protected void Awake()
        {
            InitSchemes();

            if (Schemes == null)
                throw new Exception("Schemes must be initialized");
        }

        public void GenerateProps()
        {
            foreach (var idSchemePair in Schemes)
            {
                byte id = idSchemePair.Key;
                PropsScheme scheme = idSchemePair.Value;

                GameObject prefab = scheme.Prefab;

                int amount = Random.Range(scheme.AmountRange.x, scheme.AmountRange.y + 1);

                if (!_instances.ContainsKey(id))
                    _instances.Add(id, new GameObject[scheme.AmountRange.y]);

                bool outOfAttempts = false;

                for (int currentInstanceIndex = 0; currentInstanceIndex < _instances[id].Length; currentInstanceIndex++)
                {
                    GameObject currentInstance = _instances[id][currentInstanceIndex];

                    if (currentInstanceIndex > amount - 1 || outOfAttempts)
                    {
                        if (currentInstance)
                            currentInstance.SetActive(false);

                        continue;
                    }

                    if (!currentInstance)
                    {
                        currentInstance = Instantiate(prefab, transform);
                        _instances[id][currentInstanceIndex] = currentInstance;
                    }

                    byte attemptCount = 0;
                    while (true)
                    {
                        attemptCount++;
                        PropsRange range = scheme.Ranges[Random.Range(0, scheme.Ranges.Length)];

                        Vector3 oldPos = prefab.transform.localPosition;
                        Vector3 oldRotation = prefab.transform.eulerAngles;

                        currentInstance.transform.localPosition =
                            new Vector3(
                                range.PositionX.HasValue
                                    ? Random.Range(range.PositionX.Value.x, range.PositionX.Value.y)
                                    : oldPos.x,
                                range.PositionY.HasValue
                                    ? Random.Range(range.PositionY.Value.x, range.PositionY.Value.y)
                                    : oldPos.y,
                                range.PositionZ.HasValue
                                    ? Random.Range(range.PositionZ.Value.x, range.PositionZ.Value.y)
                                    : oldPos.z);

                        currentInstance.transform.eulerAngles =
                            new Vector3(
                                range.RotationX.HasValue
                                    ? Random.Range(range.RotationX.Value.x, range.RotationX.Value.y)
                                    : oldRotation.x,
                                range.RotationY.HasValue
                                    ? Random.Range(range.RotationY.Value.x, range.RotationY.Value.y)
                                    : oldRotation.y,
                                range.RotationZ.HasValue
                                    ? Random.Range(range.RotationZ.Value.x, range.RotationZ.Value.y)
                                    : oldRotation.z);

                        ApplyAdditionalSettingsToProp(currentInstance, prefab, range);

                        if (IntersectionTest())
                        {
                            if (attemptCount < AttemptNumber) continue;

                            outOfAttempts = true;
                            currentInstance.SetActive(false);
                            break;
                        }

                        currentInstance.SetActive(true);
                        break;
                    }
                }
            }
        }

        private bool IntersectionTest()
        {
            var colliders = transform.GetComponentsInChildren<BoxCollider>();

            if (colliders.Length == 0)
                return false;

            return (from collider1 in colliders
                from collider2 in colliders
                where collider1 != collider2
                where collider1.bounds.Intersects(collider2.bounds)
                select collider1).Any();
        }

        protected abstract void ApplyAdditionalSettingsToProp(GameObject currentInstance, GameObject prefab,
            PropsRange range);
    }
}