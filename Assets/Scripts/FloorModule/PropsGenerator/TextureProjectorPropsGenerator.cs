using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace FloorModule.PropsGenerator
{
    public class TextureProjectorPropsGenerator : PropsGenerator
    {
        enum TextureProjectorId : byte
        {
            DIRT_1 = 0,
            DIRT_2,
            DIRT_3,
            DIRT_4,
            DIRT_5,
            FOOTPRINTS_1
        }

        [SerializeField] private GameObject dirt1ProjectorPrefab;
        [SerializeField] private GameObject dirt3ProjectorPrefab;
        [SerializeField] private GameObject footprints1ProjectorPrefab;

        protected override void InitScheme()
        {
            Schemes = new Dictionary<byte, PropsScheme>
            {
                [(byte) TextureProjectorId.DIRT_1] = new PropsScheme()
                {
                    Prefab = dirt1ProjectorPrefab,
                    Ranges = new PropsRange[]
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
                [(byte) TextureProjectorId.DIRT_3] = new PropsScheme()
                {
                    Prefab = dirt3ProjectorPrefab,
                    Ranges = new PropsRange[]
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
                [(byte) TextureProjectorId.FOOTPRINTS_1] = new PropsScheme()
                {
                    Prefab = footprints1ProjectorPrefab,
                    Ranges = new PropsRange[]
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
    }
}