using ProtoBuf;
using SMLHelper.V2.Utility;
using System;
using System.IO;
using UnityEngine;
namespace techpistol
{
    [RequireComponent(typeof(EnergyMixin))]
    class Gun : PlayerTool, IProtoEventListener
    {
        public override string animToolName => "flashlight";
        public FMOD_CustomLoopingEmitter Drillon;
        public FMODAsset shoot1;
        public FMODAsset shoot2;
        public FMOD_StudioEventEmitter xulikai;
        public FMODASRPlayer laseroopS;
        public FMODAsset modechang;
        public GameObject grabbedEffect;
        public ParticleSystem[] par = new ParticleSystem[10];
        public LineRenderer[] Line = new LineRenderer[10];
        public GameObject LineRender;
        public GameObject scale;
        public bool CannonStart = false;
        public bool LaserStart = false;
        public bool Scalebig = false;
        public bool Scalesamm = false;
        public bool Drill = false;
        public float time;
        public float time2;
        public int mode;
        public TextMesh textname;
        public TextMesh textblood;
        public TextMesh textmode;
        public config cof;
        void tagetlaser(float range, Transform LineRen, LineRenderer lineder)
        {
            if (Targeting.GetTarget(Player.main.gameObject, range, out GameObject Target, out float dist))
            {
                lineder.SetPosition(0, gameObject.FindChild("Point").transform.position);
                lineder.SetPosition(1, Player.main.camRoot.mainCamera.transform.forward * dist + Player.main.camRoot.mainCamera.transform.position);
                LineRen.position = Player.main.camRoot.mainCamera.transform.forward * dist + Player.main.camRoot.mainCamera.transform.position;
            }
            else
            {
                lineder.SetPosition(0, gameObject.FindChild("Point").transform.position);
                lineder.SetPosition(1, Player.main.camRoot.mainCamera.transform.forward * range + Player.main.camRoot.mainCamera.transform.position);
                LineRen.position = Player.main.camRoot.mainCamera.transform.forward * range + Player.main.camRoot.mainCamera.transform.position;
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
                GameObject rend = (techpistol.darktest.LoadAsset<GameObject>("Laserend.prefab"));
                LineRender = Instantiate(rend, transform.position, transform.rotation);
                GameObject scaleae = (techpistol.darktest.LoadAsset<GameObject>("scale.prefab"));
                scale = Instantiate(scaleae, transform.position, transform.rotation);

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

                GameObject DrillMode = gameObject.FindChild("DrillMode");
                par[7] = DrillMode.FindChild("Driill").GetComponent<ParticleSystem>();

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
                    textmode.text = "Drill";
                }
                if (mode == 5)
                {
                    textmode.text = "Standby";
                    mode = 0;
                }
            }
            else
            {
                textmode.text = "No Power";
            }
            return true;
        }
        public void Update()
        {
            if (energyMixin.charge > 0f)
            {
                if (LaserStart == true)
                {
                    tagetlaser(cof.LaserRange,LineRender.transform, Line[1]);
                }
                if (mode == 3)
                {
                    if (Scalebig == true)
                    {
                        tagetlaser(cof.Scalerange, scale.transform, Line[2]);
                    }
                    if (Scalesamm == true)
                    {
                        tagetlaser(cof.Scalerange, scale.transform, Line[3]);
                    }
                    if (Input.GetKeyDown(KeyCode.Q) && energyMixin.charge > 0f)
                    {
                        if (Scalebig == false)
                        {
                            scale.GetComponent<ParticleSystem>().Play();
                            par[6].Play();
                            FMODUWE.PlayOneShot(shoot1, transform.position, 1f);
                            Scalesamm = true;
                        }
                    }
                    if (Input.GetKeyUp(KeyCode.Q))
                    {
                        par[6].Stop();
                        scale.GetComponent<ParticleSystem>().Stop();
                        Line[3].SetPosition(0, new Vector3(0, 0, 0));
                        Line[3].SetPosition(1, new Vector3(0, 0, 0));
                        Scalesamm = false;
                    }
                }
            }
        }
        public void LateUpdate()
        {
            if (energyMixin.charge > 0f)
            {
                if (mode == 1)
                {
                    if (CannonStart == true)
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
                                energyMixin.ConsumeEnergy(30f);
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
                }
                if (mode == 2)
                {
                    if (LaserStart == true)
                    {
                        energyMixin.ConsumeEnergy(0.2f);
                        par[4].gameObject.transform.Rotate(Vector3.forward * 5);
                        if (Targeting.GetTarget(Player.main.gameObject, cof.LaserRange, out GameObject Targetb, out float distb) && Targetb.GetComponentInChildren<LiveMixin>())
                        {
                            Targetb.GetComponent<LiveMixin>().TakeDamage(cof.LaserDamage, Targetb.transform.position, DamageType.Explosive, null);
                        }
                        else if (Targetb)
                        {
                            DamageSystem.RadiusDamage(cof.LaserDamage, Targetb.transform.position, 1f, DamageType.Explosive, Targetb);
                        }
                    }
                }
                if (mode == 3)
                {
                    if (Scalebig == true)
                    {
                        energyMixin.ConsumeEnergy(0.1f);
                        par[5].gameObject.transform.Rotate(Vector3.forward * 5);
                        if (Targeting.GetTarget(Player.main.gameObject, cof.Scalerange, out GameObject TargetB, out float distB) && TargetB.GetComponentInChildren<Creature>())
                        {
                            float size = UWE.Utils.GetEntityRoot(TargetB).transform.localScale.x;
                            UWE.Utils.GetEntityRoot(TargetB).GetComponentInChildren<Creature>().SetScale(size + cof.ScaleUpspeed);
                        }
                    }
                    if (Scalesamm == true)
                    {
                        energyMixin.ConsumeEnergy(0.1f);
                        par[6].gameObject.transform.Rotate(-Vector3.forward * 5);
                        if (Targeting.GetTarget(Player.main.gameObject, cof.Scalerange, out GameObject TargetB, out float distB) && TargetB.GetComponentInChildren<Creature>())
                        {
                            float size = UWE.Utils.GetEntityRoot(TargetB).transform.localScale.x;
                            UWE.Utils.GetEntityRoot(TargetB).GetComponentInChildren<Creature>().SetScale(size - cof.ScaleDownspeed);
                        }
                    }
                }
                if (mode == 4)
                {
                    if (Targeting.GetTarget(Player.main.gameObject, 5, out GameObject Targetb, out float distb) && Targetb.FindAncestor<Drillable>() && Drill)
                    {
                        energyMixin.ConsumeEnergy(0.5f);
                        grabbedEffect.transform.localScale = Vector3.one * 1f;
                        grabbedEffect.transform.position = Player.main.camRoot.mainCamera.transform.forward * distb + Player.main.camRoot.mainCamera.transform.position;
                        Targetb.transform.position = Vector3.MoveTowards(Targetb.transform.position, Player.main.camRoot.mainCamera.transform.forward * 3 + Player.main.camRoot.mainCamera.transform.position, 10 * Time.deltaTime);
                        Targetb.FindAncestor<Drillable>().OnDrill(transform.position, null, out GameObject arth);
                    }
                }
            }
            else
            {
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
                    LineRender.GetComponent<ParticleSystem>().Play();
                }
                if (mode == 3 && Scalesamm == false)
                {
                    scale.GetComponent<ParticleSystem>().Play();
                    FMODUWE.PlayOneShot(shoot1, transform.position, 1f);
                    par[5].Play();
                    Scalebig = true;
                }
                if (mode == 4 && Targeting.GetTarget(Player.main.gameObject, 5, out GameObject Targetb, out float distb) && Targetb.FindAncestor<Drillable>())
                {
                    grabbedEffect.SetActive(true);
                    Drillon.Play();
                    par[7].Play();
                    Drill = true;
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
                LineRender.GetComponent<ParticleSystem>().Stop();
                laseroopS.Stop();
                LaserStart = false;
            }
            if (mode == 3)
            {
                scale.GetComponent<ParticleSystem>().Stop();
                Line[2].SetPosition(0, new Vector3(0, 0, 0));
                Line[2].SetPosition(1, new Vector3(0, 0, 0));
                par[5].Stop();
                Scalebig = false;
            }
            if (mode == 4)
            {
                Drillon.Stop();
                par[7].Stop();
                grabbedEffect.SetActive(false);
                Drill = false;
            }
            return true;
        }
        public void reset()
        {
            par[1].Stop();
            par[2].Stop();
            par[3].Stop();
            par[4].Stop();
            par[5].Stop();
            par[7].Stop();
            LineRender.GetComponent<ParticleSystem>().Stop();
            scale.GetComponent<ParticleSystem>().Stop();
            LineRender.transform.position = new Vector3(0, 0, 0);
            par[4].gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
            laseroopS.Stop();
            Drillon.Stop();
            grabbedEffect.SetActive(false);
            xulikai.Stop(true);
            LaserStart = false;
            CannonStart = false;
            Scalebig = false;
            Scalesamm = false;
            Drill = false;
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
                    File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + GetComponent<PrefabIdentifier>().Id + ".type", type);
                    File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + GetComponent<PrefabIdentifier>().Id + ".charge", energyMixin.charge.ToString());
                }
            }
            else
            {
                type = "None";
                File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + GetComponent<PrefabIdentifier>().Id + ".type", type);
                File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/" + GetComponent<PrefabIdentifier>().Id + ".charge", "0");
            }
        }
        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            if (energyMixin == null)
            {
                energyMixin = GetComponent<EnergyMixin>();
            }
            if (File.Exists((SaveUtils.GetCurrentSaveDataDir() + "/" + GetComponent<PrefabIdentifier>().Id + ".type")))
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