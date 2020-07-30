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
        [SerializeField] private GameObject bottleBagPrefab;
        [SerializeField] private GameObject cigaretteBagPrefab;

        protected override void InitSchemes()
        {
            Schemes = new Dictionary<byte, PropsScheme>
            {
                [(byte) GarbageId.GARBAGE_BAG] = new PropsScheme()
                {
                    Prefab = garbageBagPrefab,
                    Ranges = new[]
                    {
                        new PropsRange()
                        {
                            PositionX = new Vector2(-3.162f, -2.595f),
                            PositionZ = new Vector2(1.657f, 1.657f)
                        }
                    },
                    AmountRange = new Vector2Int(3, 8)
                }
            };
        }

        protected override void ApplyAdditionalSettingsToProp(GameObject currentInstance, GameObject prefab,
            PropsRange range)
        {
        }
    }
}