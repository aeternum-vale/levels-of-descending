using System.Collections.Generic;
using UnityEngine;

namespace FloorModule.PropsGenerator
{
    public class TextureProjectorPropsGenerator : PropsGenerator
    {
        [SerializeField] private GameObject dirt1ProjectorPrefab;
        [SerializeField] private GameObject dirt2ProjectorPrefab;
        [SerializeField] private GameObject dirt3ProjectorPrefab;
        [SerializeField] private GameObject dirt4ProjectorPrefab;
        [SerializeField] private GameObject dirt5ProjectorPrefab;
        [SerializeField] private GameObject footprints1ProjectorPrefab;

        protected override void InitSchemes()
        {
            Schemes = new Dictionary<byte, PropsScheme>
            {
                [(byte) TextureProjectorId.DIRT_1] = new PropsScheme
                {
                    Prefab = dirt1ProjectorPrefab,
                    Ranges = new PropsRange[]
                    {
                        new TextureProjectorRange
                        {
                            PositionX = new Vector2(-3, -1),
                            FieldOfView = new Vector2(30, 40),
                            AspectRatio = new Vector2(.5f, 1f)
                        },
                        new TextureProjectorRange
                        {
                            PositionY = new Vector2(.2f, .2f),
                            PositionX = new Vector2(2.1f, 3.2f),
                            AspectRatio = new Vector2(.5f, 1f)
                        },
                        new TextureProjectorRange
                        {
                            RotationY = new Vector2(180, 180),
                            PositionX = new Vector2(-3, -1),
                            PositionY = new Vector2(2.179f, 2.179f),
                            FieldOfView = new Vector2(30, 40),
                            AspectRatio = new Vector2(.5f, 1f)
                        },
                        new TextureProjectorRange
                        {
                            RotationY = new Vector2(180, 180),
                            PositionY = new Vector2(.2f, .2f),
                            PositionX = new Vector2(2.1f, 3.2f),
                            AspectRatio = new Vector2(.5f, 1f)
                        }
                    },
                    AmountRange = new Vector2Int(1, 3)
                },

                [(byte) TextureProjectorId.DIRT_2] = new PropsScheme
                {
                    Prefab = dirt2ProjectorPrefab,
                    Ranges = new PropsRange[]
                    {
                        new TextureProjectorRange
                        {
                            PositionX = new Vector2(-2.7f, -0.77f),

                            FieldOfView = new Vector2(31.48f, 46f),
                            AspectRatio = new Vector2(.5f, 1f)
                        },
                        new TextureProjectorRange
                        {
                            PositionY = new Vector2(.2f, .2f),
                            PositionX = new Vector2(2.1f, 3.15f),

                            AspectRatio = new Vector2(.5f, 1f)
                        },
                        new TextureProjectorRange
                        {
                            RotationY = new Vector2(180, 180),

                            PositionX = new Vector2(-2.7f, -0.77f),

                            AspectRatio = new Vector2(.5f, 1f)
                        },
                        new TextureProjectorRange
                        {
                            RotationY = new Vector2(180, 180),

                            PositionY = new Vector2(0.367f, 0.367f),
                            PositionX = new Vector2(2.147f, 2.7f),

                            AspectRatio = new Vector2(.5f, 1f)
                        },
                        new TextureProjectorRange
                        {
                            RotationY = new Vector2(-90, -90),

                            PositionX = new Vector2(-1.61f, -1.61f),
                            PositionZ = new Vector2(-1.059f, 1.212f),

                            AspectRatio = new Vector2(.5f, 1f)
                        },
                        new TextureProjectorRange
                        {
                            RotationY = new Vector2(90, 90),

                            PositionX = new Vector2(1.961f, 1.961f),
                            PositionY = new Vector2(0.4f, 0.4f),
                            PositionZ = new Vector2(1.177f, -0.602f),

                            AspectRatio = new Vector2(.5f, 1f)
                        }
                    },
                    AmountRange = new Vector2Int(2, 5)
                },

                [(byte) TextureProjectorId.DIRT_3] = new PropsScheme
                {
                    Prefab = dirt3ProjectorPrefab,
                    Ranges = new PropsRange[]
                    {
                        new TextureProjectorRange
                        {
                            PositionX = new Vector2(-2.5f, 3f),
                            PositionY = new Vector2(-.5f, 2f),
                            RotationZ = new Vector2(0, 359),
                            FieldOfView = new Vector2(10, 28),
                            AspectRatio = new Vector2(.8f, 1f)
                        },
                        new TextureProjectorRange
                        {
                            RotationY = new Vector2(180, 180),

                            PositionX = new Vector2(-2.5f, 3f),
                            PositionY = new Vector2(-.5f, 2f),
                            RotationZ = new Vector2(0, 359),
                            FieldOfView = new Vector2(10, 28),
                            AspectRatio = new Vector2(.8f, 1f)
                        }
                    },
                    AmountRange = new Vector2Int(3, 8)
                },

                [(byte) TextureProjectorId.DIRT_4] = new PropsScheme
                {
                    Prefab = dirt4ProjectorPrefab,
                    Ranges = new PropsRange[]
                    {
                        new TextureProjectorRange
                        {
                            AspectRatio = new Vector2(4, 5)
                        }
                    },
                    AmountRange = new Vector2Int(0, 1)
                },

                [(byte) TextureProjectorId.DIRT_5] = new PropsScheme
                {
                    Prefab = dirt5ProjectorPrefab,
                    Ranges = new PropsRange[]
                    {
                        new TextureProjectorRange
                        {
                            PositionX = new Vector2(-2.5f, 3f),
                            PositionY = new Vector2(-.5f, 2f),
                            RotationZ = new Vector2(0, 359),
                            FieldOfView = new Vector2(10, 28),
                            AspectRatio = new Vector2(.8f, 1f)
                        },

                        new TextureProjectorRange
                        {
                            RotationY = new Vector2(180, 180),

                            PositionX = new Vector2(-2.5f, 3f),
                            PositionY = new Vector2(-.5f, 2f),
                            RotationZ = new Vector2(0, 359),
                            FieldOfView = new Vector2(10, 28),
                            AspectRatio = new Vector2(.8f, 1f)
                        }
                    },
                    AmountRange = new Vector2Int(0, 3)
                },

                [(byte) TextureProjectorId.FOOTPRINTS_1] = new PropsScheme
                {
                    Prefab = footprints1ProjectorPrefab,
                    Ranges = new PropsRange[]
                    {
                        new TextureProjectorRange
                        {
                            PositionX = new Vector2(-3, -.8f),
                            PositionZ = new Vector2(-.5f, .5f),
                            RotationZ = new Vector2(0, 359)
                        }
                    },
                    AmountRange = new Vector2Int(1, 3)
                }
            };
        }

        protected override void ApplyAdditionalSettingsToProp(GameObject currentInstance, GameObject prefab,
            PropsRange range)
        {
            TextureProjectorRange tpRange = (TextureProjectorRange) range;

            Projector projectorComponent = currentInstance.GetComponent<Projector>();
            Projector projectorPrefabComponent = prefab.GetComponent<Projector>();

            projectorComponent.fieldOfView = tpRange.FieldOfView.HasValue
                ? Random.Range(tpRange.FieldOfView.Value.x, tpRange.FieldOfView.Value.y)
                : projectorPrefabComponent.fieldOfView;


            projectorComponent.aspectRatio = tpRange.AspectRatio.HasValue
                ? Random.Range(tpRange.AspectRatio.Value.x, tpRange.AspectRatio.Value.y)
                : projectorPrefabComponent.aspectRatio;
        }

        private enum TextureProjectorId : byte
        {
            DIRT_1 = 0,
            DIRT_2,
            DIRT_3,
            DIRT_4,
            DIRT_5,
            FOOTPRINTS_1
        }
    }
}