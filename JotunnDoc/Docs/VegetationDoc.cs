﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using Jotunn.Managers;
using UnityEngine;

namespace JotunnDoc.Docs
{
    class VegetationDoc : Doc
    {

        public VegetationDoc() : base("zones/vegetation-list.md")
        {
            ZoneManager.OnVanillaLocationsAvailable += DocVegetations;
        }

        private void DocVegetations()
        {
            if (Generated)
            {
                return;
            }

            Jotunn.Logger.LogInfo("Documenting vegetation");

            AddHeader(1, "Vegetation");
            AddText("All of the vegetation currently in the game.");
            AddText("This file is automatically generated from Valheim using the JotunnDoc mod found on our GitHub.");

            var imageDirectory = Path.Combine(DocumentationDirConfig.Value, "images/prefabs");
            Directory.CreateDirectory(imageDirectory);
             
            AddTableHeader("Vegetation", "Biome", "BiomeArea", "Quantity per zone", "Properties", "Filters");
            foreach (var vegetation in ZoneSystem.instance.m_vegetation.Where(zl => zl.m_enable && zl.m_prefab))
            { 
                bool hasSprite = RequestSprite(Path.Combine(imageDirectory, $"{vegetation.m_prefab.name}.png"), vegetation.m_prefab, RenderManager.IsometricRotation); 
                
                AddTableRow(
                    GetNameBox(vegetation, hasSprite),
                    GetBiome(vegetation.m_biome),
                    GetBiomeArea(vegetation.m_biomeArea),
                    GetQuantity(vegetation),
                    GetProperties(vegetation),
                    GetFilters(vegetation)
                );
            }
            Save();
        }

        private static string GetNameBox(ZoneSystem.ZoneVegetation zoneVegetation, bool hasSprite)
        {
            StringBuilder sb = new StringBuilder(zoneVegetation.m_prefab.name);
            if (hasSprite)
            { 
                sb.Append($"<br><img src=\"../../images/prefabs/{zoneVegetation.m_prefab.name}.png\">");
            }
            return sb.ToString();
        }

        private static string GetQuantity(ZoneSystem.ZoneVegetation vegetation)
        {
            if(vegetation.m_groupSizeMax > 1)
            {
                return $"<ul><li>{RangeString(vegetation.m_min, vegetation.m_max)} groups of {RangeString(vegetation.m_groupSizeMin, vegetation.m_groupSizeMax)}</li>{(vegetation.m_groupSizeMax > 1 ? $"<li>Group Radius: {vegetation.m_groupRadius}</li>": "")}</ul>";
            }
            return $"{RangeString(vegetation.m_min, vegetation.m_max)}";
        }

        private string GetFilters(ZoneSystem.ZoneVegetation vegetation)
        {
            
            var inForest = vegetation.m_inForest && (vegetation.m_forestTresholdMin > 0 || vegetation.m_forestTresholdMax < 1);
            return "<ul>" +
                $"<li>Altitude: {RangeString(vegetation.m_minAltitude, vegetation.m_maxAltitude)}</li>" +
                $"<li>Terrain Delta: {RangeString(vegetation.m_minTerrainDelta, vegetation.m_maxTerrainDelta)}</li>" +
                $"<li>Terrain Delta Radius: {vegetation.m_terrainDeltaRadius}</li>" +
                $"<li>Ocean Depth: {RangeString(vegetation.m_minOceanDepth, vegetation.m_maxOceanDepth)}</li>" +
                $"<li>Tilt: {RangeString(vegetation.m_minTilt, vegetation.m_maxTilt)}</li>" +
                $"{(inForest ? $"<li>Forest Threshold: {RangeString(vegetation.m_forestTresholdMin, vegetation.m_forestTresholdMax)}" : "")}</li>" +
                 "</ul>";
        } 

        private string GetProperties(ZoneSystem.ZoneVegetation vegetation)
        {
            return $"<ul>" +
                $"{(vegetation.m_snapToWater ? "<li>Snap to water</li>" : "")}" +
                $"{(vegetation.m_scaleMax > vegetation.m_scaleMin ? $"<li>Random scale: {RangeString(vegetation.m_scaleMin, vegetation.m_scaleMax)}</li>" : "")}" +
                $"{(vegetation.m_groundOffset != 0 ? $"<li>Ground offset: {vegetation.m_groundOffset}" : "")}</li>" +
                $"</ul>";
        }

        private string GetBiomeArea(Heightmap.BiomeArea biomeArea)
        {
            StringBuilder biomeAreas = new StringBuilder("<ul>");
            foreach (Heightmap.BiomeArea area in Enum.GetValues(typeof(Heightmap.BiomeArea)))
            {
                if (area == Heightmap.BiomeArea.Everything || (biomeArea & area) == 0)
                {
                    continue;
                }
                biomeAreas.Append($"<li>{area}</li>");
            }

            biomeAreas.Append("</ul>");

            return biomeAreas.ToString();
        }

        private string GetBiome(Heightmap.Biome biome)
        {
            StringBuilder biomeAreas = new StringBuilder("<ul>");

            foreach (Heightmap.Biome area in ZoneManager.GetMatchingBiomes(biome))
            {
                biomeAreas.Append($"<li>{area}</li>");
            }

            biomeAreas.Append("</ul>");

            return biomeAreas.ToString();
        }
    }
}
