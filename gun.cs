using ProtoBuf;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace techpistol
{
    [RequireComponent(typeof(EnergyMixin))]
    class Gun : PlayerTool, IProtoEventListener
    {
        public override string animToolName => "flashlight";
        public FMODAsset shoot1;
        public FMODAsset shoot2;
        public FMOD_StudioEventEmitter xulikai;
        public FMODASRPlayer laseroopS;
        public FMODAsset modechang;
        public ParticleSystem[] par = new ParticleSystem[10];
        public LineRenderer[] Line = new LineRenderer[10];
        public GameObject dis;
        public bool CannonStart = false;
        public bool LaserStart = false;
        public bool Scalebig = false;
        public bool Scalesamm = false;
        public float time;
        public float time2;
        public int mode;
        public TextMesh textname;
        public TextMesh textblood;
        public TextMesh textmode;
        public config cof;
        void tagetlaser(float range, LineRenderer lineder)
        {
            if (Targeting.GetTarget(Player.main.gameObject, range, out GameObject Target, out float dist))
            {
                lineder.SetPosition(0, gameObject.FindChild("Point").transform.position);
                lineder.SetPosition(1, Player.main.camRoot.mainCamera.transform.forward * dist + Player.main.camRoot.mainCamera.transform.position);
                dis.transform.position = Player.main.camRoot.mainCamera.transform.forward * dist + Player.main.camRoot.mainCamera.transform.position;
            }
            else
            {
                lineder.SetPosition(0, gameObject.FindChild("Point").transform.position);
                lineder.SetPosition(1, Player.main.camRoot.mainCamera.transform.forward * range + Player.main.camRoot.mainCamera.transform.position);
                dis.transform.position = Player.main.camRoot.mainCamera.transform.forward * range + Player.main.camRoot.mainCamera.transform.position;
            }
        }
        public override void OnHolster()
        {
            reset();
        }
        private void Start()
        {
            try
            {
                cof = JsonUtility.FromJson<config>(File.ReadAllText(Environment.CurrentDirectory + "/QMods/techpistol/config.json"));
                GameObject help = (techpistol.darktest.LoadAsset<GameObject>("Dis.prefab"));
                dis = Instantiate(help, transform.position, transform.rotation);

                par[0] = gameObject.FindChild("modech").GetComponent<ParticleSystem>();

                GameObject Cannonmode = gameObject.FindChild("Cannonmode");
                par[1] = Cannonmode.FindChild("Ball").GetComponent<ParticleSystem>();
                par[2] = Cannonmode.FindChild("Charge").GetComponent<ParticleSystem>();
                par[3] = Cannonmode.FindChild("shoot").GetComponent<ParticleSystem>();

                GameObject Lasermode = gameObject.FindChild("Lasermode");
                par[4] = Lasermode.FindChild("Laser").GetComponent<ParticleSystem>();
                Line[1] = Lasermode.FindChild("line").GetComponent<LineRenderer>();

                GameObject Scalemode = gameObject.FindChild("Scalemode");
                par[5] = Scalemode.FindChild("Laser").GetComponent<ParticleSystem>();
                par[6] = Scalemode.FindChild("Lasersamm").GetComponent<ParticleSystem>();
                Line[2] = Scalemode.FindChild("linebig").GetComponent<LineRenderer>();
                Line[3] = Scalemode.FindChild("linesamm").GetComponent<LineRenderer>();
                textname = gameObject.transform.Find("miazhun/name").gameObject.GetComponent<TextMesh>();
                textblood = gameObject.transform.Find("miazhun/blood").gameObject.GetComponent<TextMesh>();
                textmode = gameObject.transform.Find("modech/modehud").gameObject.GetComponent<TextMesh>();
            }
            catch
            {
                Console.WriteLine("初始化错误");
            }
        }
        public override bool OnAltDown()
        {
            if (energyMixin.charge > 0f)
            {
                par[0].Play();
                reset();
                mode++;
                FMODUWE.PlayOneShot(modechang, transform.position, 1f);
                if (mode == 1)
                {
                    textmode.text = "Cannon";
                    time = 10f;
                    time2 = 10f;
                }
                if (mode == 2)
                {
                    textmode.text = "Laser";
                }
                if (mode == 3)
                {
                    textmode.text = "Scale";
                }
                if (mode == 4)
                {
                    textmode.text = "Standby";
                    mode = 0;
                }
            }
            return true;
        }
        public void Update()
        {
            if (energyMixin.charge > 0f && isDrawn)
            {
                if (LaserStart == true)
                {
                    tagetlaser(cof.LaserRange,Line[1]);
                }
                if (mode == 3)
                {
                    if (Scalebig == true)
                    {
                        tagetlaser(cof.ScaleRange, Line[2]);
                    }
                    if (Scalesamm == true)
                    {
                        tagetlaser(cof.ScaleRange, Line[3]);
                    }
                    if (Input.GetKeyDown(KeyCode.Q) && energyMixin.charge > 0f && Scalebig == false)
                    {
                        dis.FindChild("scale").GetComponent<ParticleSystem>().Play();
                        par[6].Play();
                        FMODUWE.PlayOneShot(shoot1, transform.position, 1f);
                        Scalesamm = true;
                        laseroopS.Play();
                    }
                    if (Input.GetKeyUp(KeyCode.Q))
                    {
                        laseroopS.Stop();
                        par[6].Stop();
                        dis.FindChild("scale").GetComponent<ParticleSystem>().Stop();
                        Line[3].SetPosition(0, new Vector3(0, 0, 0));
                        Line[3].SetPosition(1, new Vector3(0, 0, 0));
                        Scalesamm = false;
                    }
                }
            }
        }
        public void LateUpdate()
        {
            if (isDrawn)
            {
                if (energyMixin.charge > 0f)
                {
                    if (mode == 1 && CannonStart == true)
                    {
                        energyMixin.ConsumeEnergy(0.05f);
                        if (time > 0f)
                        {
                            time -= 5f * Time.deltaTime;
                        }
                        else
                        {
                            par[2].Stop();
                            if (time2 > 0f)
                            {
                                time2 -= 5f * Time.deltaTime;
                            }
                            else
                            {
                                energyMixin.ConsumeEnergy(15f);
                                FMODUWE.PlayOneShot(shoot1, transform.position, 1f);
                                FMODUWE.PlayOneShot(shoot2, transform.position, 1f);
                                par[1].Stop();
                                par[1].Clear();
                                par[3].transform.rotation = Player.main.camRoot.mainCamera.transform.rotation;
                                par[3].Play();
                                time = 10f;
                                time2 = 10f;
                                CannonStart = false;
                            }
                        }
                    }
                    if (mode == 2 && LaserStart == true)
                    {
                        energyMixin.ConsumeEnergy(0.2f);
                        par[4].gameObject.transform.Rotate(Vector3.forward * 5);
                        if (Targeting.GetTarget(Player.main.gameObject, cof.LaserRange, out GameObject Targetb, out float distb) && Targetb.GetComponentInChildren<LiveMixin>())
                        {
                            UWE.Utils.GetEntityRoot(Targetb).GetComponentInChildren<LiveMixin>().TakeDamage(cof.LaserDamage, Targetb.transform.position, DamageType.Explosive, null);
                        }
                        else if (Targetb)
                        {
                            DamageSystem.RadiusDamage(cof.LaserDamage, Targetb.transform.position, 1f, DamageType.Explosive, UWE.Utils.GetEntityRoot(Targetb));
                        }
                    }
                    if (mode == 3)
                    {
                        if (Scalebig == true)
                        {
                            energyMixin.ConsumeEnergy(0.1f);
                            par[5].gameObject.transform.Rotate(Vector3.forward * 5);
                            if (Targeting.GetTarget(Player.main.gameObject, cof.ScaleRange, out GameObject TargetB, out float distB))
                            {
                                float size = UWE.Utils.GetEntityRoot(TargetB).transform.localScale.x;
                                if (UWE.Utils.GetEntityRoot(TargetB).GetComponent<Creature>() != null)
                                {
                                    UWE.Utils.GetEntityRoot(TargetB).GetComponent<Creature>().SetScale(size + cof.ScaleUpspeed);
                                }
                                else
                                {
                                    UWE.Utils.GetEntityRoot(TargetB).transform.localScale += Vector3.one * cof.ScaleUpspeed;
                                }
                            }
                        }
                        if (Scalesamm == true)
                        {
                            energyMixin.ConsumeEnergy(0.1f);
                            par[6].gameObject.transform.Rotate(-Vector3.forward * 5);
                            if (Targeting.GetTarget(Player.main.gameObject, cof.ScaleRange, out GameObject TargetB, out float distB))
                            {
                                float size = UWE.Utils.GetEntityRoot(TargetB).transform.localScale.x;
                                if (UWE.Utils.GetEntityRoot(TargetB).GetComponent<Creature>() != null)
                                {
                                    UWE.Utils.GetEntityRoot(TargetB).GetComponent<Creature>().SetScale(size - cof.ScaleDownspeed);
                                }
                                else
                                {
                                    UWE.Utils.GetEntityRoot(TargetB).transform.localScale -= Vector3.one * cof.ScaleDownspeed;
                                }
                            }
                        }
                    }
                }
                else
                {
                    textmode.text = "No Power";
                    mode = 0;
                    reset();
                }
                if (Targeting.GetTarget(Player.main.gameObject, cof.HealthDetectionRange, out GameObject TargetA, out float dist) && TargetA.GetComponentInChildren<LiveMixin>())
                {
                    string blood = TargetA.GetComponentInChildren<LiveMixin>().health.ToString();
                    string name = TargetA.GetComponentInChildren<LiveMixin>().name;
                    name = name.Replace("(Clone)", "");
                    name = name.Replace("Leviathan", "");
                    if (blood == "0")
                    {
                        blood = "0-death";
                    }
                    textname.text = name;
                    textblood.text = blood;
                }
                else
                {
                    textname.text = "No target";
                    textblood.text = "";
                }
            }
        }
        public override bool OnRightHandDown()
        {
            if (energyMixin.charge > 0f)
            {
                if (mode == 1)
                {
                    CannonStart = true;
                    par[1].Play();
                    par[2].Play();
                    time = 10f;
                    time2 = 10f;
                    xulikai.StartEvent();
                }
                if (mode == 2)
                {
                    FMODUWE.PlayOneShot(shoot1, transform.position, 1f);
                    laseroopS.Play();
                    LaserStart = true;
                    par[4].Play();
                    dis.FindChild("Laserend").GetComponent<ParticleSystem>().Play();
                }
                if (mode == 3 && Scalesamm == false)
                {
                    FMODUWE.PlayOneShot(shoot1, transform.position, 1f);
                    par[5].Play();
                    laseroopS.Play();
                    Scalebig = true;
                    dis.FindChild("scale").GetComponent<ParticleSystem>().Play();
                }
            }
            return true;
        }
        public override bool OnRightHandUp()
        {
            if (mode == 1)
            {
                par[1].Stop();
                par[2].Stop();
                par[3].Stop();
                xulikai.Stop(false);
                CannonStart = false;
            }
            if (mode == 2)
            {
                Line[1].SetPosition(0, new Vector3(0, 0, 0));
                Line[1].SetPosition(1, new Vector3(0, 0, 0));
                par[4].Stop();
                laseroopS.Stop();
                LaserStart = false;
                dis.FindChild("Laserend").GetComponent<ParticleSystem>().Stop();
            }
            if (mode == 3)
            {
                dis.FindChild("scale").GetComponent<ParticleSystem>().Stop();
                Line[2].SetPosition(0, new Vector3(0, 0, 0));
                Line[2].SetPosition(1, new Vector3(0, 0, 0));
                par[5].Stop();
                laseroopS.Stop();
                Scalebig = false;
            }
            return true;
        }
        public void reset()
        {
            dis.FindChild("scale").GetComponent<ParticleSystem>().Stop();
            dis.FindChild("Laserend").GetComponent<ParticleSystem>().Stop();
            par[1].Stop();
            par[2].Stop();
            par[3].Stop();
            par[4].Stop();
            par[5].Stop();
            par[6].Stop();
            par[4].gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
            laseroopS.Stop();
            xulikai.Stop(true);
            LaserStart = false;
            CannonStart = false;
            Scalebig = false;
            Scalesamm = false;
            Line[1].SetPosition(0, new Vector3(0, 0, 0));
            Line[1].SetPosition(1, new Vector3(0, 0, 0));
            Line[2].SetPosition(0, new Vector3(0, 0, 0));
            Line[2].SetPosition(1, new Vector3(0, 0, 0));
            Line[3].SetPosition(0, new Vector3(0, 0, 0));
            Line[3].SetPosition(1, new Vector3(0, 0, 0));
        }
        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            Console.WriteLine("保存电池存档");
            string type = null;

            GameObject battery = energyMixin.GetBattery();
            if (battery)
            {
                CraftData.GetTechType(battery);
                if (CraftData.GetTechType(battery) == TechType.PrecursorIonBattery)
                {
                    type = "PrecursorIonBattery";
                }
                if (CraftData.GetTechType(battery) == TechType.Battery)
                {
                    type = "Battery";
                }
                if (CraftData.GetTechType(battery) == TechType.PowerCell)
                {
                    type = "PowerCell";
                }
                if (CraftData.GetTechType(battery) == TechType.PrecursorIonPowerCell)
                {
                    type = "PrecursorIonPowerCell";
                }
                if (CraftData.GetTechType(battery) != TechType.None && energyMixin.HasItem())
                {
                    File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/Pistol/" + GetComponent<PrefabIdentifier>().Id + ".type", type);
                    File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/Pistol/" + GetComponent<PrefabIdentifier>().Id + ".charge", energyMixin.charge.ToString());
                }
            }
            else
            {
                type = "None";
                File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/Pistol/" + GetComponent<PrefabIdentifier>().Id + ".type", type);
                File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/Pistol/" + GetComponent<PrefabIdentifier>().Id + ".charge", "0");
            }
        }
        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            if (energyMixin == null)
            {
                energyMixin = GetComponent<EnergyMixin>();
            }
            if (File.Exists((SaveUtils.GetCurrentSaveDataDir() + "/Pistol/" + GetComponent<PrefabIdentifier>().Id + ".type")))
            {
                string type = File.ReadAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + GetComponent<PrefabIdentifier>().Id + ".type");
                float energy = float.Parse(File.ReadAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + GetComponent<PrefabIdentifier>().Id + ".charge"));
                if(type != "None")
                {
                    if (type == "PrecursorIonBattery")
                    {
                        energyMixin.SetBattery(TechType.PrecursorIonBattery, energy / 500);
                    }
                    if (type == "Battery")
                    {
                        energyMixin.SetBattery(TechType.Battery, energy / 100);
                    }
                    if (type == "PowerCell")
                    {
                        energyMixin.SetBattery(TechType.PowerCell, energy / 200);
                    }
                    if (type == "PrecursorIonPowerCell")
                    {
                        energyMixin.SetBattery(TechType.PrecursorIonPowerCell, energy / 1000);
                    }
                }
            }
        }
    }
}