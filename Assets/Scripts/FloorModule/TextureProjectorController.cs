using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace FloorModule
{
    public class TextureProjectorController : MonoBehaviour
    {
        enum TextureProjectorId
        {
            DIRT_1,
            DIRT_2,
            DIRT_3,
            DIRT_4,
            DIRT_5,
            FOOTPRINTS_1
        }

        [SerializeField] private GameObject dirt1ProjectorPrefab;
        [SerializeField] private GameObject dirt3ProjectorPrefab;
        [SerializeField] private GameObject footpreints1ProjectorPrefab;


        private Dictionary<TextureProjectorId, TextureProjectorScheme> _schemes;

        private Dictionary<TextureProjectorId, GameObject[]> _instances =
            new Dictionary<TextureProjectorId, GameObject[]>();

        private void Awake()
        {
            _schemes = new Dictionary<TextureProjectorId, TextureProjectorScheme>
            {
                [TextureProjectorId.DIRT_1] = new TextureProjectorScheme()
                {
                    Prefab = dirt1ProjectorPrefab,
                    Ranges = new[]
                    {
                        new TextureProjectorRange()
                        {
                            PositionX = new Vector2(-3, -1),
                            FieldOfView = new Vector2(30, 40),
                            AspectRatio = new Vector2(.5f, 1f)
                        },
                        new TextureProjectorRange()
                        {
                            PositionY = new Vector2(.2f, .2f),
                            PositionX = new Vector2(2.1f, 3.2f),
                            AspectRatio = new Vector2(.5f, 1f)
                        },
                        new TextureProjectorRange()
                        {
                            RotationY = new Vector2(180, 180),
                            PositionX = new Vector2(-3, -1),
                            FieldOfView = new Vector2(30, 40),
                            AspectRatio = new Vector2(.5f, 1f)
                        },
                        new TextureProjectorRange()
                        {
                            RotationY = new Vector2(180, 180),
                            PositionY = new Vector2(.2f, .2f),
                            PositionX = new Vector2(2.1f, 3.2f),
                            AspectRatio = new Vector2(.5f, 1f)
                        },
                    },
                    AmountRange = new Vector2Int(5, 10)
                },
                [TextureProjectorId.DIRT_3] = new TextureProjectorScheme()
                {
                    Prefab = dirt3ProjectorPrefab,
                    Ranges = new []
                    {
                        new TextureProjectorRange()
                        {
                            PositionX = new Vector2(-2.5f, 3f),
                            PositionY = new Vector2(-.5f, 2f),
                            RotationZ = new Vector2(0, 359),
                            FieldOfView = new Vector2(10, 28),
                            AspectRatio = new Vector2(.8f, 1f)
                        },
                    },
                    AmountRange = new Vector2Int(3, 8)
                },
                [TextureProjectorId.FOOTPRINTS_1] = new TextureProjectorScheme()
                {
                    Prefab = footpreints1ProjectorPrefab,
                    Ranges = new []
                    {
                        new TextureProjectorRange()
                        {
                            PositionX = new Vector2(-3, -.8f),
                            PositionZ = new Vector2(-.5f, .5f),
                            RotationZ = new Vector2(0, 359)
                        },
                    },
                    AmountRange = new Vector2Int(1, 3)
                }
            };
        }

        public void GenerateRandomTextureProjectors()
        {
            foreach (var idSchemePair in _schemes)
            {
                TextureProjectorId id = idSchemePair.Key;
                TextureProjectorScheme scheme = idSchemePair.Value;

                GameObject projPrefab = scheme.Prefab;

                int amount = Random.Range(scheme.AmountRange.x, scheme.AmountRange.y + 1);

                if (!_instances.ContainsKey(id))
                {
                    _instances.Add(id, new GameObject[scheme.AmountRange.y]);
                }

                for (int currentProjIndex = 0; currentProjIndex < _instances[id].Length; currentProjIndex++)
                {
                    GameObject projInstance = _instances[id][currentProjIndex];

                    if (currentProjIndex > amount - 1)
                    {
                        if (projInstance)
                            projInstance.SetActive(false);

                        continue;
                    }

                    if (!projInstance)
                    {
                        projInstance = Instantiate(projPrefab, transform);
                        _instances[id][currentProjIndex] = projInstance;
                    }
                    else
                    {
                        projInstance.SetActive(true);
                    }

                    Projector projectorComponent = projInstance.GetComponent<Projector>();
                    TextureProjectorRange range = scheme.Ranges[Random.Range(0, scheme.Ranges.Length)];

                    Vector3 oldPos = projInstance.transform.localPosition;
                    Vector3 oldRotation = projInstance.transform.eulerAngles;

                    projInstance.transform.localPosition =
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

                    projInstance.transform.eulerAngles =
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

                    if (range.FieldOfView.HasValue)
                    {
                        projectorComponent.fieldOfView =
                            Random.Range(range.FieldOfView.Value.x, range.FieldOfView.Value.y);
                    }

                    if (range.AspectRatio.HasValue)
                    {
                        projectorComponent.aspectRatio =
                            Random.Range(range.AspectRatio.Value.x, range.AspectRatio.Value.y);
                    }
                }
            }
        }
    }
}