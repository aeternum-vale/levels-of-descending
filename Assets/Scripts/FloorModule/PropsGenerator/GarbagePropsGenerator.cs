using System.Collections.Generic;
using UnityEngine;

namespace FloorModule.PropsGenerator
{
    public class GarbagePropsGenerator : PropsGenerator
    {
        private readonly PropsRange _firstFloorRange =
            new PropsRange
            {
                PositionY = new Vector2(-1.2f, -1.2f),
                PositionX = new Vector2(-3.3f, -0.622f),
                PositionZ = new Vector2(-1.635f, 1.677f),
                RotationY = new Vector2(0, 359)
            };

        private readonly PropsRange _secondFloorRange1 =
            new PropsRange
            {
                PositionY = new Vector2(0.815f, 0.815f),
                PositionX = new Vector2(1.808f, 3.561f),
                PositionZ = new Vector2(1.663f, -0.899f),
                RotationY = new Vector2(0, 359)
            };

        private readonly PropsRange _secondFloorRange2 =
            new PropsRange
            {
                PositionY = new Vector2(0.815f, 0.815f),
                PositionX = new Vector2(1.808f, 2.92f),
                PositionZ = new Vector2(1.663f, -1.532f),
                RotationY = new Vector2(0, 359)
            };

        [SerializeField] private GameObject bottlePrefab;
        [SerializeField] private GameObject canPrefab;
        [SerializeField] private GameObject cigarettePrefab;
        [SerializeField] private GameObject crumpledPaperPrefab;

        [SerializeField] private GameObject garbageBagPrefab;

        protected override void InitSchemes()
        {
            Schemes = new Dictionary<byte, PropsScheme>
            {
                [(byte) GarbageId.GARBAGE_BAG] = new PropsScheme
                {
                    Prefab = garbageBagPrefab,
                    AmountRange = new Vector2Int(0, 2),
                    Ranges = new[]
                    {
                        new PropsRange
                        {
                            PositionX = new Vector2(-3.162f, -2.595f),
                            PositionZ = new Vector2(1.657f, 1.657f),
                            RotationY = new Vector2(0, 359)
                        }
                    }
                },
                [(byte) GarbageId.BOTTLE] = new PropsScheme
                {
                    Prefab = bottlePrefab,
                    AmountRange = new Vector2Int(0, 3),
                    Ranges = new[]
                    {
                        new PropsRange
                        {
                            PositionX = new Vector2(3.64f, 3.9f),
                            PositionZ = new Vector2(0.39f, -0.34f)
                        },
                        new PropsRange
                        {
                            PositionY = new Vector2(0.817f, 0.817f),
                            PositionX = new Vector2(3.559f, 3.155f),
                            PositionZ = new Vector2(1.679f, -1.098f)
                        }
                    }
                },
                [(byte) GarbageId.CIGARETTE] = new PropsScheme
                {
                    Prefab = cigarettePrefab,
                    AmountRange = new Vector2Int(2, 10),
                    Ranges = new[]
                    {
                        _firstFloorRange,
                        _firstFloorRange,
                        _secondFloorRange1,
                        _secondFloorRange2
                    }
                },
                [(byte) GarbageId.CRUMPLED_PAPER] = new PropsScheme
                {
                    Prefab = crumpledPaperPrefab,
                    AmountRange = new Vector2Int(1, 2),
                    Ranges = new[]
                    {
                        _firstFloorRange,
                        _firstFloorRange,
                        _secondFloorRange1,
                        _secondFloorRange2
                    }
                },
                [(byte) GarbageId.CAN] = new PropsScheme
                {
                    Prefab = canPrefab,
                    AmountRange = new Vector2Int(0, 1),
                    Ranges = new[]
                    {
                        _firstFloorRange,
                        _firstFloorRange,
                        _secondFloorRange1,
                        _secondFloorRange2
                    }
                }
            };
        }

        protected override void ApplyAdditionalSettingsToProp(GameObject currentInstance, GameObject prefab,
            PropsRange range)
        {
        }

        private enum GarbageId : byte
        {
            GARBAGE_BAG = 0,
            CAN,
            CRUMPLED_PAPER,
            BOTTLE,
            CIGARETTE
        }
    }
}