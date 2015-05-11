// 
//     Take Command!
// 
//     Copyright (C) 2015 Sean McDougall
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TakeCommand
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class AddSeatModule : MonoBehaviour
    {
        private static bool initialized = false;

        public void Update()
        {
            if (!initialized)
            {
                initialized = true;
                addSeatModule();
            }
        }

        private void addSeatModule()
        {
            try
            {
                ConfigNode node = new ConfigNode("MODULE");
                node.AddValue("name", "TakeCommand");

                var partInfo = PartLoader.getPartInfoByName("seatExternalCmd");
                print("[TakeCommand] addSeatModule [" + Time.time + "]: Part info = " + partInfo);

                var prefab = partInfo.partPrefab;
                print("[TakeCommand] addSeatModule [" + Time.time + "]: Prefab = " + prefab);

                prefab.CrewCapacity = 1;

                var module = prefab.AddModule(node);
                print("[TakeCommand] Did not detect expected exception in addSeatModule [" + Time.time + "]: Module = " + module);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Object reference not set"))
                {
                    print("[TakeCommand] addSeatModule succeeded.");
                }
                else
                {
                    print("[TakeCommand] addSeatModule [" + Time.time + "]: Failed to add the part module to seatExternalCmd: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
    }

    public class TakeCommand : PartModule
    {
        public static List<Part> allCommandSeats = new List<Part>();

        GameObject escapeHatch = null;
        BoxCollider escapeHatchCollider = null;

        private bool boardKerbal = false;
        private string myKerbal;

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (this.part.protoModuleCrew.Count > 0)
                {
                    if (allCommandSeats.Count == 0)
                    {
                        print("[TakeCommand] populating seat list");
                        foreach (Part p in vessel.parts)
                        {
                            if (p.Modules.OfType<TakeCommand>().Any())
                            {
                                if (p.protoModuleCrew.Count > 0)
                                {
                                    allCommandSeats.Add(p);
                                }
                            }
                        }
                        print("[TakeCommand] found " + allCommandSeats.Count + " occupied seats");
                    }
                    if (escapeHatch == null)
                    {
                        escapeHatch = new GameObject("EscapeHatch");
                        escapeHatch.tag = "Airlock";
                        escapeHatch.layer = 21;
                        escapeHatch.transform.parent = this.part.transform;
                        escapeHatch.transform.localEulerAngles = new Vector3(0, 0, 0);
                        escapeHatch.transform.localPosition = new Vector3(0, 0, 0);

                        escapeHatch.AddComponent<BoxCollider>();
                        escapeHatchCollider = escapeHatch.GetComponent<BoxCollider>();
                        escapeHatchCollider.size = new Vector3(0.25f, 0.25f, 0.25f);
                        escapeHatchCollider.isTrigger = true;
                        this.part.airlock = escapeHatch.transform;
                        print("[TakeCommand] added escape hatch to " + this.part.name + " (" + this.part.GetInstanceID() + ")");
                    }
                }
            }
            base.OnStart(state);
        }

        public override void OnUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight && vessel.Landed && vessel.HoldPhysics == false)
            {
                if (boardKerbal == false)
                {
                    if (this.part.protoModuleCrew.Count > 0 && allCommandSeats.First().GetInstanceID() == this.part.GetInstanceID())
                    {
                        ProtoCrewMember kerbal = this.part.protoModuleCrew.Single();
                        print("[TakeCommand] ejecting " + kerbal.name + " from " + this.part.GetInstanceID());
                        if (FlightEVA.fetch.spawnEVA(kerbal, this.part, escapeHatch.transform))
                        {
                            myKerbal = "kerbalEVA (" + kerbal.name + ")";
                            boardKerbal = true;
                            escapeHatch.collider.enabled = false;
                            this.part.airlock = null;
                            DestroyObject(escapeHatchCollider);
                            DestroyObject(escapeHatch);
                            print("[TakeCommand] removed escape hatch from " + this.part.name + " (" + this.part.GetInstanceID() + ")");
                        }
                        else
                        {
                            print("[TakeCommand] error ejecting " + kerbal.name);
                        }
                    }
                }
                else
                {
                    bool grabASeat = false;

                    foreach (Vessel v in FlightGlobals.Vessels)
                    {
                        if (v.name == myKerbal && v.isActiveVessel)
                        {
                            grabASeat = true;
                        }
                    }

                    if (grabASeat)
                    {
                        KerbalEVA kerbal = FlightGlobals.ActiveVessel.GetComponent<KerbalEVA>();

                        if (kerbal.fsm.Started == true)
                        {

                            boardKerbal = false;
                            allCommandSeats.Remove(allCommandSeats.First());
                            print("[TakeCommand]  seating " + kerbal.name + " in " + this.part.GetInstanceID());
                            this.part.Modules.OfType<KerbalSeat>().Single().BoardSeat();
                        }
                    }

                }
            }
            base.OnUpdate();
        }

    }

}