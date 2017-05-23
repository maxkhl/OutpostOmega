--[=====[ 
Outpost Omega - Lua API Examples
This is a collection of useful actions if you want to modify Outpost Omega.
The whole script can be executed ingame.
--]=====]

-- 1: Hello World!
print("Example 1: Hello World")
print("Hello World!") -- yep... thats it. This is only visible in the console

-- 2: Get a GameObject and do stuff with it
print("Example 2: Get a GameObject and do stuff")
firstgameobject = GetFirstGO("")    -- we could enter a string to find a specific gameobject but in this case we just pick the first we find
print(firstgameobject.ID) 			-- thats the GOs ID
print(firstgameobject.Position) 	-- Position
print(firstgameobject.Static)       -- Static or dynamic object?
print(firstgameobject.IsPhysical)   -- Has a physical body?

firstgameobject:SetPosition(Vector3(10, 10, 10)) -- We move the object to [10,10,10]

if(firstgameobject.IsPhysical) then -- If object is physical
	firstgameobject:PhysicDisable() -- Disable physic
end									-- This means everything can pass through it now

-- 3: Get all GameObjects and print details about them
print("Example 3: All loaded GameObjects")
gameobjects = GetGO("")
for k, v in pairs(gameobjects) do
  print(k .. " - " .. v.Position:ToString()) -- we need to  make a string out of v.Position to be able to concatenate it
end

-- 4: Create a new GameObject
print("Example 4: Create a new GameObject")
builder = NewBuilder()				-- first we need a GameObject-Builder
builder.ClassName = "NiceObject"	-- the classname will be the internal name of our new gameobject
builder:SetParent("OutpostOmega.Game.gObject.structure.machinery.machinery")		-- now we need to tell him what kind of object we want to create

-- Now we add attributes to our gameobject
-- The first parameter is the argument type and the second one are the parameters
builder:AddAttribute("OutpostOmega.Game.gObject.attributes.Definition",  {"Nice Object", "What a nice object. Holy sheeet"})
builder:AddAttribute("OutpostOmega.Game.gObject.attributes.Construction",  { getType("OutpostOmega.Game.gObject.structure.Frame"), getType("OutpostOmega.Game.gObject.item.tool"), "Content\\\Model\\\Structure\\Machinery\\\Doors\\\Airlock.dae"})

-- Now we need to allocate functions to our newly designed gameobject
-- Its basically the way to add action to our gameobject. We hook Lua functions to specific actions, gameobjects are working with.

-- First we hook a function to the constructor point. This function will be executed every time a instance of our new gameobject is created.
-- We can use this to setup our gameobject
builder:Hook("Constructor", 
function(myObject) -- the constructor point will pass the new created instance
	myObject.Model = "Content\\\Model\\\Structure\\Machinery\\\Dooars\\\Airlock.dae" -- now we set the model path for the new instance
end)

-- Alright thats the first hook. Our object would be setup now but we want to give it some logic.
-- To do that in this case we will hook our function to the KeyPress point.
-- This point gets passed every time a player is standing close and directly looking at our gameobject while pressing a key
builder:Hook("KeyPress",
function(myObject, Key, IsRepeat)
	if(IsKey(Key, "E") and IsRepeat) then  -- We check if the Key is E and we only want the first frame the user startet to press E
		myObject:Dispose()					-- This does delete this instance of our gameobject completely
	end
end)

-- Now we are ready to compile our builder and release it into the gameworld
newType = builder:Compile()
-- The gameobject can now be spawned from the spawnmenu or a script

print(newType)