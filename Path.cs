using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace techpistol
{
    public static class techpistol
    {
        public static AssetBundle darktest;
        public static void Patch()
        {


            darktest = AssetBundle.LoadFromFile(Environment.CurrentDirectory + "/QMods/techpistol/Assets");
            Atlas.Sprite Icon = new Atlas.Sprite(darktest.LoadAsset<Sprite>("Icon"));
            var gun = TechTypeHandler.AddTechType("techpistol", "tech pistol", "tech pistol", true);
            SpriteHandler.RegisterSprite(gun, Icon);
            GunPrefab gunper = new GunPrefab("techpistol", "WorldEntities/Tools/techpistol", gun);
            PrefabHandler.RegisterPrefab(gunper);
            CraftDataHandler.SetEquipmentType(gun, EquipmentType.Hand);
            var techData = new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
               {
                   new Ingredient(TechType.SeaTreaderPoop, 1),
                   new Ingredient(TechType.TitaniumIngot, 2),
                   new Ingredient(TechType.Lubricant, 1),
                   new Ingredient(TechType.EnameledGlass, 3),
               }
            };
            CraftDataHandler.SetTechData(gun, techData);
            CraftDataHandler.SetCraftingTime(gun, 5f);
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, gun, "Personal", "Tools", "techpistol");
            CraftDataHandler.SetItemSize(gun, 2, 2);
        }
        public static T AgComponent<T>(this GameObject objec) where T : Component
        {
            if (objec.GetComponent<T>() != null)
            {
                return objec.GetComponent<T>();
            }
            else
            {
                return objec.AddComponent<T>();
            }
        }
    }
}
