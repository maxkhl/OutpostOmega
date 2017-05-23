print("Building PDA object")

builder = NewBuilder("PDA", g_type_item)

builder:AddAttribute(g_type_attr_definition,  {"PDA", "They dumped smartphones and went back to PDAs."})
builder:AddAttribute(g_type_attr_construction,  { getType("OutpostOmega.Game.GameObjects.Structures.Frame"), getType("OutpostOmega.Game.GameObjects.Items.Tool") })  

builder:OnNewInstance(function(myObject)
	model = myObject:LoadModel(GetFirstContent("PDAModel").Path)
	model:AssignTexture("Cube.001", myObject, GetFirstContent("PDATexture"))
	model:AssignUserInterface("Cube.002", GetFirstContent("MainScreen"))

	
	--Setup physics
    myObject.Shape = myObject:MeshToShape(model, model:GetMesh("Colission")) --PhysicBoxShape(0.1, 0.26, 0.26) --X,Y,Z (Y = UP)
	myObject.Mass = 1
    --myObject.Static = true
    myObject:PhysicCreateMaterial() --Create the shapes material (needed to apply mass)
    myObject:PhysicEnable() --Tell engine we are done
    myObject:PhysicEnableDebug() --Needs to be enabled to see shape in debugging mode
end)

newType = builder:Compile()
print("Object "..newType:ToString().." compiled")