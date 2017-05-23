print("### Default Outpost Omega Extension Mod ###")
print("###   Written by MaxKhl (maxkhl.com)    ###")

print("Building 'RapeCube'")
builder = NewBuilder("RapeCube", g_type_item)

builder:AddAttribute(g_type_attr_definition,  {"Rape Cube", "I wonder whats inside that cube"})
builder:AddAttribute(g_type_attr_construction,  { getType("OutpostOmega.Game.GameObjects.Structures.Frame"), getType("OutpostOmega.Game.GameObjects.Items.Tool") }) 

builder:OnNewInstance(function(myObject)

	myObject.ID = myObject:GetUniqueID("RapeCube")


	model = myObject:LoadModel(GetFirstContent("CubeModel").Path)
	model:AssignTexture("Cube", myObject, GetFirstContent("CubeTexture"))
	model:GetMesh("Cube"):GetGOPair(myObject).UseAlpha = true
	
	--Setup physics
    myObject.Shape = PhysicBoxShape(2, 2, 2) --X,Y,Z (Y = UP)
	myObject.Mass = 20
    myObject.Static = true
    myObject:PhysicCreateMaterial() --Create the shapes material (needed to apply mass)
    myObject:PhysicEnable() --Tell engine we are done
    myObject:PhysicEnableDebug() --Needs to be enabled to see shape in debugging mode

	animate(myObject)

	print("New RapeCube created. Prepare your anussss")

end)
 
newType = builder:Compile()
print("Object "..newType:ToString().." compiled")

function animate(myObject)	
	animType = math.random (3)
	
	if(animType == 1) then
		anim = myObject:Animate("Position", Vector3(myObject.Position.X + 5, myObject.Position.Y, myObject.Position.Z), 5000, "Linear")
		anim.AnimationDone:Add(function()	
			anim = myObject:Animate("Position", Vector3(myObject.Position.X, myObject.Position.Y, myObject.Position.Z + 5), 5000, "Linear")
			anim.AnimationDone:Add(function()	
				anim = myObject:Animate("Position", Vector3(myObject.Position.X - 5, myObject.Position.Y, myObject.Position.Z), 5000, "Linear")
				anim.AnimationDone:Add(function()	
					anim = myObject:Animate("Position", Vector3(myObject.Position.X, myObject.Position.Y, myObject.Position.Z - 5), 5000, "Linear")
					anim.AnimationDone:Add(function()
						animate(myObject)
					end)
				end)
			end)
		end)
	end
	
	if(animType == 2) then
		upAnimation = myObject:Animate("Position", Vector3(myObject.Position.X, myObject.Position.Y + 10, myObject.Position.Z), 5000, "Linear")
		upAnimation.AnimationDone:Add(function()
			forwardAnimation = myObject:Animate("Position", Vector3(myObject.Position.X, myObject.Position.Y, myObject.Position.Z + 5), 2500, "Linear")
			forwardAnimation.AnimationDone:Add(function()
				sidewardAnimation = myObject:Animate("Position", Vector3(myObject.Position.X+10, myObject.Position.Y, myObject.Position.Z), 2500, "Linear")
				sidewardAnimation.AnimationDone:Add(function()
					myObject.Static = false
					myObject.RigidBody.AngularVelocity = Vector3(0, 15, 0)
					myObject.RigidBody.LinearVelocity = Vector3(-5, 10, -5)
					print(myObject.ID .. " was released. RUN!")
				end)
			end)
		end)
	end
	
	if(animType == 3) then	 
		anim = myObject:Animate("Position", Vector3(myObject.Position.X + 5, myObject.Position.Y, myObject.Position.Z), 5000, "CircEaseInOut")
		animl = myObject:Animate("Position", Vector3(myObject.Position.X, myObject.Position.Y, myObject.Position.Z + 2.5), 2500, "CircEaseOut") 
		animl.AnimationDone:Add(function()	
			animlk = myObject:Animate("Position", Vector3(myObject.Position.X, myObject.Position.Y, myObject.Position.Z - 2.5), 2500, "CircEaseIn") 
		end)

		anim.AnimationDone:Add(function()
			animas = myObject:Animate("Position", Vector3(myObject.Position.X - 5, myObject.Position.Y, myObject.Position.Z), 5000, "CircEaseInOut")
			animds = myObject:Animate("Position", Vector3(myObject.Position.X, myObject.Position.Y, myObject.Position.Z - 2.5), 2500, "CircEaseOut") 
			animds.AnimationDone:Add(function()	
				animlak = myObject:Animate("Position", Vector3(myObject.Position.X, myObject.Position.Y, myObject.Position.Z + 2.5), 2500, "CircEaseIn") 
			end)
			animas.AnimationDone:Add(function()
				animate(myObject)
			end)	
		end)	
	end
end