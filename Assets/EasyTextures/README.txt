Created by Cody Anderson.
2019.


INSTRUCTIONS:

*These tools are based on multi-selection where you need to select multiple textures and/or materials at the same time.
To use multi-selection, hold down Ctrl, then left click all the items you want to select.*


HOW TO ASSIGN TEXTURES TO A MATERIAL: 
------------------------------------------------------
You can assign a whole texture set or only specific textures depending on your selection. Designed to be used with the Standard Shader, but may work on shaders with similar texture slots.
(NOTE: Textures CANNOT be assigned to the Default-Material. Assign a different material to the object or use the CREATE A NEW MATERIAL option mentioned below.)

1) In the Project panel, select all the textures and the material you want to assign them to.

OR, if all the textures are in their own folder, you can:
1) In the Project panel, just select the material and the folder the textures are in, instead of all the individual textures.

OR, if you don’t want to find the material in the project files:
1) You can select a GameObject with a MeshRenderer in the Game view, then select the textures.

2) Then right-click on them in the Project panel and go to “Easy Textures > Assign Textures to Material” or use the hotkey (default is Alt+T). 


RESULT: This will assign the textures to the material based on the name of each texture. This tool searchs for "keys" contained in the texture name unique to each texture type.
To learn more about how to change the keys and what they do, see the Settings window section below.




HOW TO CREATE A NEW MATERIAL FROM TEXTURES:
------------------------------------------------------
1) Select all the textures 

OR, if the textures are in their own folder, you can:
1) Just select the folder the textures are in.

2) Then right-click in the Project panel and go to “Easy Textures > Create New Material from Selection” or use the hotkey (default is Ctrl+Alt+C).


RESULT: This will create a new material and assign all the textures to it for you. The material will be created in the same folder as the textures, 
and it will try to match the name of your textures. Now you can simply drag that material onto any model you want.




CLEAR ALL TEXTURES FROM MATERIALS:
------------------------------------------------------
1) Select the material(s) you want to clear textures from in the Project panel.

OR, you can:
1) Select the GameObjects in the Game view with the materials you want to clear.

2) Then just right-click and go to “Easy Textures > Clear Textures” or use the hotkey (default is Shift+Alt+C).


RESULT: This will quickly reset a material back to default!




THE SETTINGS WINDOW:
------------------------------------------------------
To open the Settings window:
	Right-click in the Project panel and go to "Easy Textures > Settings".
	OR, on the menu bar, go to “Windows > Easy Textures > Settings”.

Auto-Fix Normal Maps Toggle:
	When toggled on this tool will automatically re-import any normal maps it detects with the Normal map TextureType. 
	This means you no longer have to click the “Fix Normal Map” button every time you assign a normal map! 

Customizable Texture Keys:
	As explained in the ASSIGN TEXTURES TO A MATERIAL section, this tool assigns textures based on the texture names. Therefore each texture type should have a unique "key" in its name.

	For example, the default key for normal maps is "normal." This means that the first texture found with "normal" in its name is the one assgined to the material.
	Therefore, it would be best to all textures of one type have a similar prefix or suffix, such as having all normal maps ending in "_normal" or "_NormalMap".
	Default keys are vague enough to work with most naming conventions, such as Substance Painter's default export for Unity 5.

	Keys can be changed for the follow texture channels: Albedo, Metallic/Smoothness, Emission, NormalMap, Height, and Occlusion.
	The "Reset Keys to Default" button will reset the keys back to default values that should work for most naming conventions.

	(NOTE: KEYS ARE NOT CASE SENSITIVE)

	(ANOTHER NOTE: Be careful that keys are truly unique! For example, if the NormalMap key is set to "normal" and the Albedo key is set to "albedo",
	a texture named "MyTextureNormalVersion_albedo" will result in weird behavior since it contains both keys. Therefore you would have to avoid using the word "normal" in your textures names, OR
	change the NormalMap key to something longer like "_normalmap" or even "_normal". The longer and more specfic the keys, the less chance of running into a name conflict.)


Hotkeys:
	Speed up your workflow even more with the hotkeys. Current hotkeys are shown in the Settings window.
	Currently there is no easy way adjust hotkeys in Unity.	However you still can change these hotkeys by editting the Hotkeys_ET.cs file. (Find the file in the path "Assets/EasyTextureAssign/Scripts/Hotkeys_ET.cs") 
	
	How to edit hotkeys: (From Unity Documentation)
    To create a hotkey you can use the following special characters: 
        % (ctrl on Windows, cmd on macOS), 
        # (shift)
        & (alt). 
        _ (No special modifier key combinations are required)

    For example to create a menu with hotkey shift-alt-g use "#&g". 
    To create a menu with hotkey g and no key modifiers pressed use "_g".
	(These instructions on how to edit the hotkeys are also included in that file.)
