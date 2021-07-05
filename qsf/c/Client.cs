using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;

namespace Client
{
    class Client
    {
        public static int respek = 5;
        public static int MissionIndex = -1;
        public static Blip MissionBlip;
        public static Vector3 MissionCoords1 = new Vector3(-12.07F, -1430.44F, 30.1F);
        public static Vector3 MissionCoords2 = new Vector3(104.96F, -1940.66F, 19.8F);
        public static Vector3 EmptyCoords = new Vector3(0F, 0F, 0F);
        public static Vector3 MissionBlipScale = new Vector3(1.5f, 1.5f, 0.5f);
        private static Vehicle cVanEntity;
        private static int cVanBlip;
        private static Ped cVanPed1;
        private static int cVanPed1Blip;
        private static Ped cVanPed2;
        private static int cVanPed2Blip;
        public static bool eventSpawned;
        private static bool eventOnScene;
        private static bool eventGreeting;
        private static bool eventFollow;
        public static bool eventEnd;
        private static Vector3 targetLocation = new Vector3();
        private static readonly Random random = new Random();

        public static async void Loop()
        {   
            Ped player = Game.Player.Character;
            if (eventSpawned)
            {
                if(!eventOnScene && API.GetDistanceBetweenCoords(cVanEntity.Position.X, cVanEntity.Position.Y, cVanEntity.Position.Z, targetLocation.X, targetLocation.Y, targetLocation.Z, true) < 30F)
                {
                    eventOnScene = true;
                    API.SetVehicleForwardSpeed(cVanEntity.Handle, 0F);
                    API.TaskLeaveVehicle(cVanPed1.Handle, cVanEntity.Handle, 64);
                    API.TaskLeaveVehicle(cVanPed2.Handle, cVanEntity.Handle, 64);
                    await BaseScript.Delay(2000);
                    
                }
                if (eventOnScene && !eventGreeting && !eventFollow)
                {
                    if (API.GetDistanceBetweenCoords(cVanPed1.Position.X, cVanPed1.Position.Y, cVanPed1.Position.Z, player.Position.X, player.Position.Y, player.Position.Z, true) < 3F)
                    {
                        eventGreeting = true;                        
                        cVanPed1.Task.ClearAllImmediately();
                        cVanPed1.PlayAmbientSpeech("GREET_GANG_FAMILIES_M");
                        cVanPed2.Task.ClearAllImmediately();
                        cVanPed2.PlayAmbientSpeech("GENERIC_HI");
                        await BaseScript.Delay(0);
                    }

                    API.TaskGoToEntity(cVanPed1.Handle, player.Handle, -1, 3F, 100, 1073741824, 0);
                    API.TaskGoToEntity(cVanPed2.Handle, player.Handle, -1, 3F, 100, 1073741824, 0);
                    await BaseScript.Delay(0);
                }
                if (eventOnScene && eventGreeting && !eventFollow)
                {
                    eventFollow = true;
                    //ped1
                    cVanPed1.Task.ClearAllImmediately();
                    cVanPed1.PlayAmbientSpeech("GREET_GANG_FAMILIES_M");
                    cVanPed1.Task.LookAt(player);
                    API.SetPedAsGroupMember(cVanPed1.Handle, API.GetPedGroupIndex(player.Handle));
                    API.SetPedRelationshipGroupHash(cVanPed1.Handle, (uint)API.GetPedRelationshipGroupHash(player.Handle));
                    API.SetPedCombatAbility(cVanPed1.Handle, 2);
                    API.GiveWeaponToPed(cVanPed1.Handle, (uint)WeaponHash.Pistol, 200, false, true);
                    API.SetPedArmour(cVanPed1.Handle, 100);
                    //ped2
                    cVanPed2.Task.ClearAllImmediately();
                    cVanPed2.PlayAmbientSpeech("GENERIC_HI");
                    cVanPed2.Task.LookAt(player);
                    API.SetPedAsGroupMember(cVanPed2.Handle, API.GetPedGroupIndex(player.Handle));
                    API.SetPedRelationshipGroupHash(cVanPed2.Handle, (uint)API.GetPedRelationshipGroupHash(player.Handle));
                    API.SetPedCombatAbility(cVanPed2.Handle, 2);
                    API.GiveWeaponToPed(cVanPed2.Handle, (uint)WeaponHash.Pistol, 200, false, true);
                    API.SetPedArmour(cVanPed2.Handle, 100);
                    await BaseScript.Delay(0);
                    eventEnd = true;
                }                
            }   
        }
        public static async void NdLoop()
        {
            API.SetRelationshipBetweenGroups(respek, (uint)API.GetHashKey("AMBIENT_GANG_FAMILY"), (uint)API.GetHashKey("PLAYER"));
            API.SetRelationshipBetweenGroups(5, (uint)API.GetHashKey("AMBIENT_GANG_BALLAS"), (uint)API.GetHashKey("PLAYER"));
            API.SetRelationshipBetweenGroups(5, (uint)API.GetHashKey("AMBIENT_GANG_MEXICAN"), (uint)API.GetHashKey("PLAYER"));
            API.SetRelationshipBetweenGroups(5, (uint)API.GetHashKey("AMBIENT_GANG_LOST"), (uint)API.GetHashKey("PLAYER"));
            if (MissionIndex == -1)
            {
                MissionBlip = World.CreateBlip(MissionCoords1);
                API.SetBlipAsShortRange(MissionBlip.Handle, true);
                if (MissionBlip.Exists())
                {
                    MissionBlip.Sprite = BlipSprite.Franklin;
                    MissionBlip.Color = BlipColor.FranklinGreen;
                }
                MissionIndex = 0;
            }
            if (MissionIndex == 0)
            {
                if (Game.Player.Character.Position.DistanceToSquared(MissionCoords1) < 10f)
                {
                    World.DrawMarker(MarkerType.VerticalCylinder, MissionCoords1, EmptyCoords, EmptyCoords, MissionBlipScale, System.Drawing.Color.FromArgb(0, 255, 0));
                    if (Game.Player.Character.Position.DistanceToSquared(MissionCoords1) < 1.5f)
                    {
                        CommonFunctions.DisplayMessage("Press E to call", 1);
                        if (Game.IsControlJustPressed(2, Control.Context))
                        {
                            MissionIndex = 10;
                        }
                    }
                }
            }
            if (MissionIndex == 10)
            {
                MissionBlip.Alpha = 0;
                MissionBlip.Delete();
                SummonBodyguard1();
                SummonBodyguards();
                MissionIndex = 11;
            }
            if (MissionIndex == 11)
            {
                MissionBlip = World.CreateBlip(MissionCoords2);
                if (MissionBlip.Exists())
                {
                    MissionBlip.Sprite = BlipSprite.Franklin;
                    MissionBlip.Color = BlipColor.FranklinGreen;
                    MissionBlip.ShowRoute = true;
                    MissionIndex = 12;
                }
            }
            if (MissionIndex == 12)
            {
                if (Game.Player.Character.Position.DistanceToSquared(MissionCoords2) < 55f)
                {
                    World.DrawMarker(MarkerType.VerticalCylinder, MissionCoords2, EmptyCoords, EmptyCoords, MissionBlipScale, System.Drawing.Color.FromArgb(0, 255, 0));
                    CommonFunctions.DisplayMessage("Shoot the Ballas!", 1);
                    if (Game.Player.Character.IsShooting)
                    {                        
                        Game.Player.WantedLevel = 2;
                        MissionBlip.Alpha = 0;
                        MissionBlip.Delete();
                        MissionIndex = 13;
                    }
                }
            }
            if (MissionIndex == 13)
            {
                Player player1 = Game.Player;
                if (API.IsPlayerWantedLevelGreater(player1.Handle, 0))
                {
                    CommonFunctions.DisplayMessage("LOSE WANTED", 1);
                }
                else
                {
                    respek = 0;
                    MissionIndex = 14;
                }
            }
            await BaseScript.Delay(0);
        }
        public async static void SummonBodyguards() 
        {
            Ped player = Game.Player.Character;
            //van
            Vector3 spawnlocation = new Vector3();
            float spawnHeading = 0F;
            int unusedVar = 0;
            API.GetNthClosestVehicleNodeWithHeading(player.Position.X, player.Position.Y, player.Position.Z, 30, ref spawnlocation, ref spawnHeading, ref unusedVar, 9, 3.0F, 2.5F);
            await CommonFunctions.LoadModel((uint)VehicleHash.Burrito3);
            cVanEntity = await World.CreateVehicle(VehicleHash.Burrito3, spawnlocation, spawnHeading);
            cVanEntity.Mods.PrimaryColor = VehicleColor.Green;
            cVanEntity.Mods.LicensePlate = $"FAM G {random.Next(10)}";
            cVanEntity.Mods.LicensePlateStyle = LicensePlateStyle.YellowOnBlack;
            //van blip
            cVanBlip = API.AddBlipForEntity(cVanEntity.Handle);
            API.SetBlipColour(cVanBlip, 2);
            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentString("CGF VAN");
            API.EndTextCommandSetBlipName(cVanBlip);
            //driver
            await CommonFunctions.LoadModel((uint)PedHash.Famca01GMY);
            cVanPed1 = await World.CreatePed(PedHash.Famca01GMY, spawnlocation);
            cVanPed1.SetIntoVehicle(cVanEntity, VehicleSeat.Driver);
            //driver blip
            cVanPed1Blip = API.AddBlipForEntity(cVanPed1.Handle);
            API.SetBlipColour(cVanPed1Blip, 2);
            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentString("CGF DRIVER");
            API.EndTextCommandSetBlipName(cVanPed1Blip);
            //passenger
            await CommonFunctions.LoadModel((uint)PedHash.Famca01GMY);
            cVanPed2 = await World.CreatePed(PedHash.Famca01GMY, spawnlocation);
            cVanPed2.SetIntoVehicle(cVanEntity, VehicleSeat.Passenger);
            //passenger blip
            cVanPed2Blip = API.AddBlipForEntity(cVanPed2.Handle);
            API.SetBlipColour(cVanPed2Blip, 2);
            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentString("CGF DRIVER");
            API.EndTextCommandSetBlipName(cVanPed2Blip);
            //config
            float targetHeading = 0F;
            API.GetClosestVehicleNodeWithHeading(player.Position.X, player.Position.Y, player.Position.Z, ref targetLocation, ref targetHeading, 1, 3.0F, 0);
            //driving style21758524,262972
            cVanPed1.Task.DriveTo(cVanEntity, targetLocation, 30F, 20F, 262972);
            eventSpawned = true;
        }
        private static bool eventOne;
        private static int bodyguardBlip;
        public async static void SummonBodyguard1()
        {
            if (!eventOne)
            {
                Ped player = Game.Player.Character;
                API.RequestModel((uint)PedHash.Famca01GMY);
                while (!API.HasModelLoaded((uint)PedHash.Famca01GMY))
                {
                    //Debug.WriteLine("WAITING MODELS");
                    await BaseScript.Delay(100);
                }
                Ped bodyguard = await World.CreatePed(PedHash.Famca01GMY, player.Position + (player.ForwardVector * 2));
                //blip
                bodyguardBlip = API.AddBlipForEntity(bodyguard.Handle);
                API.SetBlipColour(bodyguardBlip, 2);
                API.BeginTextCommandSetBlipName("STRING");
                API.AddTextComponentString("CGF OG");
                API.EndTextCommandSetBlipName(bodyguardBlip);
                //tasks
                bodyguard.Task.LookAt(player);
                API.SetPedAsGroupMember(bodyguard.Handle, API.GetPedGroupIndex(player.Handle));
                API.SetPedRelationshipGroupHash(bodyguard.Handle, (uint)API.GetPedRelationshipGroupHash(player.Handle));
                API.SetPedCombatAbility(bodyguard.Handle, 2);
                API.GiveWeaponToPed(bodyguard.Handle, (uint)WeaponHash.Pistol, 200, false, true);
                API.SetPedArmour(bodyguard.Handle, 100);
                bodyguard.PlayAmbientSpeech("GENERIC_HI");
                eventOne = true;
            }
        }
    }
}
