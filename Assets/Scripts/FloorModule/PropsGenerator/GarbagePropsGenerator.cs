using System.Collections.Generic;
using UnityEngine;

namespace FloorModule.PropsGenerator
{
    public class GarbagePropsGenerator : PropsGenerator
    {
        private enum GarbageId : byte
        {
            GARBAGE_BAG = 0,
            CAN,
            CRUMPLED_PAPER,
            BOTTLE,
            CIGARETTE
        }

        [SerializeField] private GameObject garbageBagPrefab;
        [SerializeField] private GameObject canPrefab;
        [SerializeField] private GameObject crumpledPaperPrefab;
        [SerializeField] private GameObject bottlePrefab;
        [SerializeField] private GameObject cigarettePrefab;

        private PropsRange _firstFloorRange =
            new PropsRange()
            {
                PositionY = new Vector2(-1.2f, -1.2f),
                PositionX = new Vector2(-3.3f, -0.622f),
                PositionZ = new Vector2(-1.635f, 1.677f),
                RotationY = new Vector2(0, 359)
            };

        protected override void InitSchemes()
        {
            Schemes = new Dictionary<byte, PropsScheme>
            {
                [(byte) GarbageId.GARBAGE_BAG] = new PropsScheme()
                {
                    Prefab = garbageBagPrefab,
                    AmountRange = new Vector2Int(0, 2),
                    Ranges = new[]
                    {
                        new PropsRange()
                        {
                            PositionX = new Vector2(-3.162f, -2.595f),
                            PositionZ = new Vector2(1.657f, 1.657f),
                            RotationY = new Vector2(0, 359)
                        }
                    }
                },
                [(byte) GarbageId.BOTTLE] = new PropsScheme()
                {
                    Prefab = bottlePrefab,
                    AmountRange = new Vector2Int(0, 5),
                    Ranges = new[]
                    {
                        new PropsRange()
                        {
                            PositionX = new Vector2(3.64f, 3.9f),
                            PositionZ = new Vector2(0.39f, -0.34f),
                        },
                        new PropsRange()
                        {
                            PositionY = new Vector2(0.817f, 0.817f),
                            PositionX = new Vector2(3.559f, 3.155f),
                            PositionZ = new Vector2(1.679f, -1.098f),
                        }
                    }
                },
                [(byte) GarbageId.CIGARETTE] = new PropsScheme()
                {
                    Prefab = cigarettePrefab,
                    AmountRange = new Vector2Int(2, 20),
                    Ranges = new[]
                    {
                        _firstFloorRange
                    }
                },
                [(byte) GarbageId.CRUMPLED_PAPER] = new PropsScheme()
                {
                    Prefab = crumpledPaperPrefab,
                    AmountRange = new Vector2Int(0, 2),
                    Ranges = new[]
                    {
                        _firstFloorRange
                    }
                },
                [(byte) GarbageId.CAN] = new PropsScheme()
                {
                    Prefab = canPrefab,
                    AmountRange = new Vector2Int(0, 2),
                    Ranges = new[]
                    {
                        _firstFloorRange
                    }
                }
            };
        }

        protected override void ApplyAdditionalSettingsToProp(GameObject currentInstance, GameObject prefab,
            PropsRange range)
        {
        }
    }
}