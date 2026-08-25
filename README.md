Hey there, this is a hopefully longish term game I am working on with Monogame! 

Genre isn't yet known (totally not another survival craft though) and I just want it to have a bunch of fun things in it that make it enjoyable to play. There will be crafting, inventories, menus, items, and a controllable player lol. Also will be top down and pixel styled (tiles are 16x16 and ui should be vaguely folowing dims in camera eg 640/360)

Current Decisions/Assertions:
Textures in classes are references to ImTextures (Either animated texs or atlas texs) which are loaded from Jsons and contain references to a texture2D loaded via monogame content pipeline. This is to facilitate texture atlassing kinda :P.
No significant AI/LLM support will be used apart from after >30 mins working on a bug as it makes me depressed to use on personal projects, and if it is used it will be well noted in comments.
All data is stored in jsons for now as it seems to be much better than hardcoding like I did in my other old monogame project.
Ui elements are recursive? polymorphic trees based off UiElements that contain elements which can be bound to values passed in to them and can have certain on click actions as defined in UiAction.
There is a map which contains retangular "chunks" that describe an area on the map and are stored as JSONs. Chunks store an array of tiles and a list of JsonEntities (defined by an ID) which are then serialized into drawable, updatable, and collidable lists. This is because I'm lazy and didn't wanna make a custom entity json converter yet lol.
Entities and Items and Tiles are stored in dictionaries of sorts where copies are made from those dictionary/factory(?) classes 

Jsons are stored in ./GameData/**/*.json for now, should find a better way to manage it in the future probably for actual builds.

Hopefully I'll implement visual editors for json to make texture creation and atlassing/modification easier, will definitly make a level design one at some point. (Currently there is a ui editor and map editor! ui editor lacks some features but helps to get a skeleton of layout). The editors are going to be in game for now, I'll probably make a flag that prevents their attachment to the program on publish tho. 

to edit mgcb on mac: dotnet tool run mgcb-editor-mac ./Content/Content.mgcb

.jsons contain data for things (something new I'm trying that should be helpful)

Aseprite export: fileName is {title}.{tag}.{tagframe}

Current external libs: monogame, apos.input, nativefiledialognet

Potential future opimizations:
- Use ZLinq for faster linq
- Optimize tile rendering to be better
- Have a 'lazy' ui that draws to a rendertarget only every so often
- Have tilemap be drawn lazily to a rendertarget too cause currently its pretty rough for no reason
- Optimize how/when chunks, ui menus, and textures are loaded

Name ideas:
NASC (NASC Ain't a Survival Craft OR Not Another Survival Craft OR NASC is A Survival Craft)
or MASC with same defs
