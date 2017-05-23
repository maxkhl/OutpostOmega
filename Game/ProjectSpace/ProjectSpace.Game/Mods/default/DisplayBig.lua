print("Building Big Display object")

builder = NewBuilder("DisplayBig", g_type_machine)

builder:AddAttribute(g_type_attr_definition,  {"Big Display", "Thats a nice flatscreen"})
builder:AddAttribute(g_type_attr_construction,  { getType("OutpostOmega.Game.GameObjects.Structures.Frame"), getType("OutpostOmega.Game.GameObjects.Items.Tool") })  


builder:OnNewInstance(function(myObject)
	model = myObject:LoadModel(GetFirstContent("DisplayBigModel").Path)
	
	model:AssignTexture("Cube.001", myObject, GetFirstContent("DisplayBigTexture"))
	interface = GetFirstContent("MainScreen")
	model:AssignUserInterface("Cube.002", myObject, interface)

	voidlogo = GetFirstContent("VoidLogo")
	interface.UIBase:SetAttribute("ImageName", voidlogo.Path)
	
	--model:AssignTexture("Cube.002", myObject, )

	--myObject.World.NewGameObject:Add(function(newObject)
	--	interface.UIBase:GetChild("groupbox"):GetChild("textbox"):SetAttribute("text", newObject.ID .. " spawned")
	--end)
	
	--Setup physics
    myObject.Shape = PhysicBoxShape(0.1, 0.26, 0.26) --X,Y,Z (Y = UP)
	myObject.Mass = 1
    myObject.Static = true
    myObject:PhysicCreateMaterial() --Create the shapes material (needed to apply mass)
    myObject:PhysicEnable() --Tell engine we are done
    myObject:PhysicEnableDebug() --Needs to be enabled to see shape in debugging mode
end)
 
newType = builder:Compile()
print("Object "..newType:ToString().." compiled")