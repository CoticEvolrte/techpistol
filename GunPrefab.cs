using SMLHelper.V2.Assets;
using SMLHelper.V2.MonoBehaviours;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace techpistol
{
    class GunPrefab : ModPrefab
    {
        public GunPrefab(string classId, string prefabFileName, TechType techType = TechType.None) : base(classId, prefabFileName, techType)
        {
            ClassID = classId;
            PrefabFileName = prefabFileName;
            TechType = techType;
        }
        public override GameObject GetGameObject()
        {
            try
            {
                GameObject gun = techpistol.darktest.LoadAsset<GameObject>("techpistol.prefab");
                MeshRenderer[] allshader = gun.FindChild("HandGun").GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer setshader in allshader)
                {
                    setshader.material.shader = Shader.Find("MarmosetUBER");
                    setshader.material.SetColor("_Emission", new Color(1f, 1f, 1f));
                }
                gun.transform.Find("Cannonmode/shoot/shoo").gameObject.AgComponent<cool>();

                gun.AgComponent<PrefabIdentifier>().ClassId = ClassID;
                gun.AgComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
                gun.AgComponent<Pickupable>().isPickupable = true; ;
                gun.AgComponent<TechTag>().type = TechType;
                gun.AgComponent<Fixer>().techType = TechType;
                gun.AgComponent<Fixer>().ClassId = ClassID;
                WorldForces World = gun.AgComponent<WorldForces>();
                Rigidbody body = gun.AgComponent<Rigidbody>();
                World.underwaterGravity = 0;
                World.useRigidbody = body;

                EnergyMixin battery = gun.AgComponent<EnergyMixin>();
                battery.storageRoot = gun.FindChild("BatteryRoot").AgComponent<ChildObjectIdentifier>();
                battery.allowBatteryReplacement = true;
                battery.compatibleBatteries = new List<TechType>
                {
                    TechType.PrecursorIonBattery,
                    TechType.Battery,
                    TechType.PrecursorIonPowerCell,
                    TechType.PowerCell
                };
                battery.batteryModels = new EnergyMixin.BatteryModels[]
                {
                    new EnergyMixin.BatteryModels
                    {
                        techType = TechType.PrecursorIonPowerCell,
                        model = gun.transform.Find("BatteryRoot/PrecursorIonPowerCell").gameObject
                    },
                    new EnergyMixin.BatteryModels
                    {
                        techType = TechType.Battery,
                        model = gun.transform.Find("BatteryRoot/Battery").gameObject
                    },
                    new EnergyMixin.BatteryModels
                    {
                        techType = TechType.PrecursorIonBattery,
                        model = gun.transform.Find("BatteryRoot/PrecursorIonBattery").gameObject
                    },
                    new EnergyMixin.BatteryModels
                    {
                        techType = TechType.PowerCell,
                        model = gun.transform.Find("BatteryRoot/PowerCell").gameObject
                    }

                };
                Gun biggun = gun.AgComponent<Gun>();
                RepulsionCannon Boo = CraftData.InstantiateFromPrefab(TechType.RepulsionCannon).GetComponent<RepulsionCannon>();
                StasisRifle Boo2 = CraftData.InstantiateFromPrefab(TechType.StasisRifle).GetComponent<StasisRifle>();
                PropulsionCannon build = CraftData.InstantiateFromPrefab(TechType.PropulsionCannon).GetComponent<PropulsionCannon>();
                Welder laserloop = CraftData.InstantiateFromPrefab(TechType.Welder).GetComponent<Welder>();
                VFXFabricating vfxfabricating = gun.FindChild("HandGun").AgComponent<VFXFabricating>();
                vfxfabricating.localMinY = -3f;
                vfxfabricating.localMaxY = 3f;
                vfxfabricating.posOffset = new Vector3(0f, 0, 0f);
                vfxfabricating.eulerOffset = new Vector3(0f, 90f, -90f);
                vfxfabricating.scaleFactor = 1f;
                biggun.shoot1 = Boo.shootSound;
                biggun.shoot2 = Boo2.fireSound;
                biggun.xulikai = Boo2.chargeBegin;
                biggun.modechang = build.shootSound;
                biggun.laseroopS = laserloop.weldSound;
                biggun.mainCollider = gun.GetComponent<BoxCollider>();
                biggun.ikAimRightArm = true;
                biggun.useLeftAimTargetOnPlayer = true;
                UnityEngine.Object.Destroy(Boo2);
                UnityEngine.Object.Destroy(build);
                UnityEngine.Object.Destroy(Boo);
                UnityEngine.Object.Destroy(laserloop);
                return gun;
            }
            catch
            {
                Console.WriteLine("初始化错误");
                return new GameObject();
            }
        }
    }
}