using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Tilemap))]
    public sealed class InstantiateVisualWalls : MonoBehaviour
    {
        [SerializeField, Required] private Tile m_wallTile = null;
        [SerializeField, Required] private GameObject m_wallVisualPrefab = null;

        private GameObject m_wallVisualsParent = null;
        private Tilemap m_tilemap = null;


        private void Awake()
        {
            m_tilemap = GetComponent<Tilemap>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_tilemap, this);

            //CustomDebug.AssertSerializeFieldIsNotNull(m_wallVisualPrefab, nameof(m_wallVisualPrefab), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_wallVisualsParent = new GameObject("Wall Visuals Parent");

            BoundsInt t_bounds = m_tilemap.cellBounds;
            for (int x = t_bounds.xMin; x <= t_bounds.xMax; ++x)
            {
                for (int y = t_bounds.yMin; y <= t_bounds.yMax; ++y)
                {
                    for (int z = t_bounds.zMin; z <= t_bounds.zMax; ++z)
                    {
                        Vector3Int t_cellPos = new Vector3Int(x, y, z);
                        Tile t_tile = m_tilemap.GetTile<Tile>(t_cellPos);
                        if (t_tile != null && t_tile.sprite == m_wallTile.sprite)
                        {
                            Bounds t_tileBounds = m_tilemap.GetBoundsLocal(t_cellPos);
                            Vector3 t_tileWorldPos = t_tileBounds.center + m_tilemap.tileAnchor + m_tilemap.transform.position;
                            Instantiate(m_wallVisualPrefab, t_tileWorldPos, Quaternion.identity, m_wallVisualsParent.transform);
                        }
                    }
                }
            }
        }
    }
}