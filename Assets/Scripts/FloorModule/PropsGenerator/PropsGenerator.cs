using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FloorModule.PropsGenerator
{
    public abstract class PropsGenerator : MonoBehaviour
    {
        [SerializeField] protected GameObject[] propsPrefabs;
        protected Dictionary<byte, PropsScheme> Schemes;

        private readonly Dictionary<byte, GameObject[]> _instances =
            new Dictionary<byte, GameObject[]>();

        protected abstract void InitScheme();

        protected void Awake()
        {
            InitScheme();

            if (Schemes == null)
                throw new Exception("Schemes must be initialized");
        }

        public virtual void GenerateProps()
        {
            foreach (var idSchemePair in Schemes)
            {
                byte id = idSchemePair.Key;
                PropsScheme scheme = idSchemePair.Value;

                GameObject prefab = scheme.Prefab;

                int amount = Random.Range(scheme.AmountRange.x, scheme.AmountRange.y + 1);

                if (!_instances.ContainsKey(id))
                {
                    _instances.Add(id, new GameObject[scheme.AmountRange.y]);
                }

                for (int currentProjIndex = 0; currentProjIndex < _instances[id].Length; currentProjIndex++)
                {
                    GameObject currentInstance = _instances[id][currentProjIndex];

                    if (currentProjIndex > amount - 1)
                    {
                        if (currentInstance)
                            currentInstance.SetActive(false);

                        continue;
                    }

                    if (!currentInstance)
                    {
                        currentInstance = Instantiate(prefab, transform);
                        _instances[id][currentProjIndex] = currentInstance;
                    }
                    else
                    {
                        currentInstance.SetActive(true);
                    }

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
                }
            }
        }

        protected abstract void ApplyAdditionalSettingsToProp(GameObject currentInstance, GameObject prefab,
            PropsRange range);
    }
}