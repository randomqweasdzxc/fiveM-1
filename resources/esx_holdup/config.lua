Config = {}
Config.Locale = 'en'

Config.Marker = {
	r = 250, g = 0, b = 0, a = 100,  -- red color
	x = 1.0, y = 1.0, z = 1.5,       -- tiny, cylinder formed circle
	DrawDistance = 5.0, Type = 1    -- default circle type, low draw distance due to indoors area
}

Config.PoliceNumberRequired = 0
Config.TimerBeforeNewRob    = 60 -- The cooldown timer on a store after robbery was completed / canceled, in seconds

Config.MaxDistance    = 15   -- max distance from the robbary, going any longer away from it will to cancel the robbary
Config.GiveBlackMoney = false -- give black money? If disabled it will give cash instead

Config.MinMoney = 1
Config.MaxMoney = 46

Stores = {
	--[[
	["vanilla"] = {
		position = { x = 95.11, y = -1294.15, z = 29.27 },
		reward = math.random(777, 1666),
		nameOfStore = "Vanilla Unicorn",
		secondsRemaining = 60, -- seconds
		lastRobbed = 0
	};
	]]--
	["grove_ltd"] = {
		position = { x = -43.40, y = -1749.20, z = 29.42 },
		reward = math.random(133, 1666),
		nameOfStore = "LTD Gasoline. (Grove Street)",
		secondsRemaining = 60, -- seconds
		lastRobbed = 0
	}
}
