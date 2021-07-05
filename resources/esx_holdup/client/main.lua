local holdingUp = false
local store = ""
local blipRobbery = nil

ESX = nil
Citizen.CreateThread(function()
	while ESX == nil do
		TriggerEvent('esx:getSharedObject', function(obj) ESX = obj end)
		Citizen.Wait(0)
	end
end)


function drawTxt(x,y, width, height, scale, text, r,g,b,a, outline)
	SetTextFont(0)
	SetTextScale(scale, scale)
	SetTextColour(r, g, b, a)
	SetTextDropshadow(0, 0, 0, 0,255)
	SetTextDropShadow()
	if outline then SetTextOutline() end

	BeginTextCommandDisplayText('STRING')
	AddTextComponentSubstringPlayerName(text)
	EndTextCommandDisplayText(x - width/2, y - height/2 + 0.005)
end

RegisterNetEvent('esx_holdup:currentlyRobbing')
AddEventHandler('esx_holdup:currentlyRobbing', function(currentStore)
	holdingUp, store = true, currentStore
end)

RegisterNetEvent('esx_holdup:killBlip')
AddEventHandler('esx_holdup:killBlip', function()
	RemoveBlip(blipRobbery)
end)

RegisterNetEvent('esx_holdup:setBlip')
AddEventHandler('esx_holdup:setBlip', function(position)
	blipRobbery = AddBlipForCoord(position.x, position.y, position.z)
	SetBlipSprite(blipRobbery, 161)
	SetBlipScale(blipRobbery, 2.0)
	SetBlipColour(blipRobbery, 3)
	PulseBlip(blipRobbery)
end)

RegisterNetEvent('esx_holdup:tooFar')
AddEventHandler('esx_holdup:tooFar', function()
	holdingUp, store = false, ''
	ESX.ShowNotification(_U('robbery_cancelled'))
end)

RegisterNetEvent('esx_holdup:robberyComplete')
AddEventHandler('esx_holdup:robberyComplete', function(award)
	holdingUp, store = false, ''
	ESX.ShowNotification(_U('robbery_complete', award))
end)

RegisterNetEvent('esx_holdup:startTimer')
AddEventHandler('esx_holdup:startTimer', function()
	local timer = Stores[store].secondsRemaining

	Citizen.CreateThread(function()
		while timer > 0 and holdingUp do
			Citizen.Wait(1000)
			if timer > 0 then
				timer = timer - 1
				
			end
		end
	end)

	Citizen.CreateThread(function()
		while holdingUp do
			Citizen.Wait(0)
			drawTxt(0.66, 1.44, 1.0, 1.0, 0.4, _U('robbery_timer', timer), 255, 255, 255, 255)
		end
	end)
end)

Citizen.CreateThread(function()
	for k,v in pairs(Stores) do
		local blip = AddBlipForCoord(v.position.x, v.position.y, v.position.z)
		SetBlipSprite(blip, 156)
		SetBlipScale(blip, 0.8)
		SetBlipAsShortRange(blip, true)

		BeginTextCommandSetBlipName("STRING")
		AddTextComponentString(_U('shop_robbery'))
		EndTextCommandSetBlipName(blip)
	end
end)

Citizen.CreateThread(function()
	while true do
		Citizen.Wait(1)
		local playerPos = GetEntityCoords(PlayerPedId(), true)

		for k,v in pairs(Stores) do
			local storePos = v.position
			local distance = Vdist(playerPos.x, playerPos.y, playerPos.z, storePos.x, storePos.y, storePos.z)

			if distance < Config.Marker.DrawDistance then
				if not holdingUp then
					DrawMarker(Config.Marker.Type, storePos.x, storePos.y, storePos.z - 1, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, Config.Marker.x, Config.Marker.y, Config.Marker.z, Config.Marker.r, Config.Marker.g, Config.Marker.b, Config.Marker.a, false, false, 2, false, false, false, false)

					if distance < 0.5 then
						ESX.ShowHelpNotification(_U('press_to_rob', v.nameOfStore))
						if IsControlJustReleased(0, 38) then
							if IsPedArmed(PlayerPedId(), 4) then
								TriggerServerEvent('esx_holdup:robberyStarted', k)
								
								SetPlayerWantedLevel(xplayer, 5, false)
								SetPlayerWantedLevelNow(xplayer, false)
								TriggerEvent("war")	
							else
								ESX.ShowNotification(_U('no_threat'))
							end
						end
					end
				end
			end
		end

		if holdingUp then
			local storePos = Stores[store].position
			if Vdist(playerPos.x, playerPos.y, playerPos.z, storePos.x, storePos.y, storePos.z) > Config.MaxDistance then
				TriggerServerEvent('esx_holdup:tooFar', store)
			end
		end
	end
end)
--
--pedmoneydrop
AddEventHandler('gameEventTriggered', function(eventName, args)
    if eventName == 'CEventNetworkEntityDamage' then
        local victim = args[1]
        local culprit = args[2]
        local isDead = args[4] == 1

        if isDead then
            local origCoords = GetEntityCoords(victim)
            local pickup = CreatePickupRotate(`PICKUP_MONEY_VARIABLE`, origCoords.x, origCoords.y, origCoords.z+0.001, 0, 0, 0, 512, 1, 0, false, 0)
            local netId = PedToNet(victim)

            local undoStuff = { false }

            CreateThread(function()
                local self = PlayerPedId()

                while not undoStuff[1] do
                    Wait(50)

                    if #(GetEntityCoords(self) - origCoords) < 2.5 and HasPickupBeenCollected(pickup) then
                        RemovePickup(pickup)

                        ESX.TriggerServerCallback('esx_holdup:giveMoney', function(amount)
                            ESX.ShowNotification(_U('stolen_money', amount))
                        end)

                        break
                    end
                end

                undoStuff[1] = true
            end)

            SetTimeout(5000, function()
                if not undoStuff[1] then
                    RemovePickup(pickup)
                    undoStuff[1] = true
                end
            end)
        end
    end
end)
--
--jevawar
local j = nil

local teams = {
    {name = "AMBIENT_GANG_BALLAS", model = "g_m_y_ballaorig_01", weapon = "WEAPON_PISTOL"},    
}
for i=1, #teams, 1 do 
    AddRelationshipGroup(teams[i].name)
end

AddEventHandler("war", function()
    local totalPeople = 4
    for i=1,totalPeople, 1 do 
        j = math.random(1,#teams) --[[ this is just a lazy way to alternate between spawning an enemy or ally ]]
        local ped = GetHashKey(teams[j].model)
        RequestModel(ped)
        while not HasModelLoaded(ped) do 
            Citizen.Wait(1)
        end
		local x,y,z = table.unpack(GetEntityCoords(GetPlayerPed(-1)))
        newPed = CreatePed(4, ped, x+math.random(-totalPeople,totalPeople), y+math.random(-totalPeople,totalPeople), z , 0.0, false, true)
        SetPedCombatAttributes(newPed, 0, true) --[[ BF_CanUseCover ]]
        SetPedCombatAttributes(newPed, 5, true) --[[ BF_CanFightArmedPedsWhenNotArmed ]]
        SetPedCombatAttributes(newPed, 46, true) --[[ BF_AlwaysFight ]]
        SetPedRelationshipGroupHash(newPed, GetHashKey(teams[j].name))
        SetPedAccuracy(newPed, 50)
        SetRelationshipBetweenGroups(5, GetHashKey(teams[j].name), GetHashKey("PLAYER")) --[[ this is really janky sorry  ]]
        TaskStartScenarioInPlace(newPed, "WORLD_HUMAN_SMOKING_POT", 0, true)
        GiveWeaponToPed(newPed, GetHashKey(teams[j].weapon), 200, true--[[ weapon is hidden or not (bool)]], false)
        SetPedArmour(newPed, 100)
        SetPedMaxHealth(newPed, 100)
    end
end)
--
