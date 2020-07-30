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

        protected override void InitSchemes()
        {
            Schemes = new Dictionary<byte, PropsScheme>
            {
                [(byte) GarbageId.GARBAGE_BAG] = new PropsScheme()
                {
                    Prefab = garbageBagPrefab,
                    AmountRange = new Vector2Int(3, 8),
                    Ranges = new[]
                    {
                        new PropsRange()
                        {
                            PositionX = new Vector2(-3.162f, -2.595f),
                            PositionZ = new Vector2(1.657f, 1.657f)
                        }
                    }
                },
                [(byte) GarbageId.CIGARETTE] = new PropsScheme()
                {
                    Prefab = cigarettePrefab,
                    AmountRange = new Vector2Int(10, 50),
                    Ranges = new []
                    {
                        new PropsRange()
                        {
                            PositionX = new Vector2(-2.6f,-1.685f),
                            PositionZ = new Vector2(0.74f,-0.217f),
                            RotationY = new Vector2(0, 359)
                        }
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