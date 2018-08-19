Popup Emotes

This library is intended to allow you to add a whole bunch of awesome emotes to your game's characters. This package is
designed to make it as easy as possible to get started so that you can spend your valuable time focusing on all the other
cool elements of your game. 

In order to allow your characters to display these emotes, you only need to follow the following steps:

    1) Import this package. It should create a "PopupEmote" folder in your project view. It's recommended that you leave the
       internal folder structure of the "PopupEmote" folder alone in case you want to pull an update from the asset store at 
       a later time.
            - However, you're free to move all the pieces around in any way that you want. There are no hard-coded references
              to these assets, so you won't break anything.
    2) Click on your character in the project hierarchy
    3) In the inspector, click the "Add Component" button
    4) In the search bar, type "PopupEmote" and choose the "PopupEmote" script.
    5) YOU'RE DONE!... well, mostly. 
    6) The script will initialize with a bunch of default settings that should be good enough to get you going.
    7) The last step is to ask the PopupEmote script to show an emote. How you do this depends on your game structure and
       character controller.

This package contains an example project that might help to see the usage of the PopupEmote scripts in action. This project
is located in the PopupEmote/Examples folder. 

    1) Open the "emoteExample" scene.
    2) Press "Play" in the editor.
    3) Select the emote you want to show above the beautiful character capsule and click "Show".
    4) Select another emote type from the dropdown and click "Show" again.
    5) That's it! You're done. 


Customizing

The PopupEmote comes with a number of customization options for your particular game. It's likely that not every single use
case is covered, but feedback is always welcome!

   PopupEmote Component

   Emote Prefab: This reference is to the actual prefab instance that will be spawned when you invoke PopupEmote.ShowEmote().
        - You can double-click on the emote prefab from the inspector and it will highlight the prefab in the project window.
          You're welcome to ajust the prefab as you see fit. Maybe you want to add a new shader instead of the default Unity
          sprite shader. Maybe you want to adjust some sizes. Maybe you want to add your own script(s). The world is your
          oyster.
        - Do be careful with this prefab. If you delete it or break it, you won't be able to show any more emotes. *Sad emote*

    Popup Animation: There are two stock popup animations provided in this package. This animation plays on the emote when it's
                     spawned to create a more dynamic look. The default behavior is a "spring up" animation. There's also a
                     "Fade Up" animation provided in the PopupEmote/Animation folder. Feel free to add your own. 

    Custom Emotes: If you want to add new emotes to your project, you can just create one or more sprites. The example project
                   included has an example of a "custom" sprite. The original .psd (photoshop) file is also included in the
                   project for your convenience. 
        - NOTE: It's recommended that you set the "Pivot" of the sprites to be "Bottom" using the Unity SpriteEditor. All of the
                default sprites are shipped with this setting. It helps with the animation. If you include non-"Bottom" sprites,
                the built-in animations might not work for you. 

    Emote Origin: This transform indicates the origin of the spawned emote. By default, it's set to the origin of the same object
                  that the script is attached to. However, you may want to change this behavior for your own project.
         For example: 
            If you have a character whose pivot (origin) is set to the be at the character's feet. That means that the
            emote would try to spawn at their feet. This probably isn't what you want. Instead, you can drag the character's head
            transform into the "Emote Origin" field. Now, the emote will spawn from the character's head. This is much closer to what
            you probably want (but who am I to judge?). 

    Offset: This value indicates any additional offset that should be applied to the emote relative to the "Emote Origin" specified
            above.
         For example:
            Using the character's head scenario from above, the emote will try to spawn directly at (0,0,0) relative to the character's
            head bone. However, the head pivot might be located at the center of the head, rather than the top. In that case, you can
            increase the "Y" value of the offset to cause the emote to spawn a bit above the head pivot. 

    Scale: Not all character's are created equal - some are bigger or smaller than others. This setting allows you to scale the emotes
           to look more appropriate with the size of the character. The emote sprites are quite large by default so that they look good
           at many different screen sizes, so you might need to shrink them for your game. The example project included in this asset
           demonstrates scaling the emotes by 1/2. 


Other

This asset also ships with a "Billboard" script for the emotes. It's super simple, but you might want to use it elsewhere in your project.
Feel free!



Good luck making games!

Sam Luo


