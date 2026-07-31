Hey there, this is a hopefully longish term game I plan to work on using Monogame with Apos.Input for input management as thats something I've implemented before and wouldn't learn much from doing again. 

Current Design Decisions:
Textures in classes are references to ImTextures (Either animated texs or atlas texs) which are loaded from Jsons and contain references to a texture2D loaded via monogame content pipeline. This is to facilitate texture atlassing kinda hopefully.
No significant AI/LLM support will be used apart from after >30 mins working on a bug as it makes me depressed to use on personal projects, and if it is used it will be noted in comments.
All data is stored in jsons for now as it seems to be much better than hardcoding.
Ui elements are recursive polymorphic structures based off UiElements that contain elements which can be bound to values passed in to them.
There is a map which contains retangular "chunks" that describe an area on the map and are stored as JSONs. Chunks store a list of Updateables, Drawables, and JsonEntities (defined by an ID). Entities are noted in the jsonentitieslist to serialize them for now, though theres probably a better way to approach it in the future.
Entities and Items and Tiles are stored in dictionaries of sorts where copies are made from those dictionary/factory? classes 
Jsons are stored in ./GameData/ for now, should find a better way to manage it in the future.

Hopefully I'll implement visual editors for json to make texture creation and atlassing/modification easier, will definitly make a level design one at some point.

to edit mgcb on mac: dotnet tool run mgcb-editor-mac ./Content/Content.mgcb

.jsons contain data for things (something new I'm trying that should be helpful)

Aseprite export: fileName is {title}.{tag}.{tagframe}


Current external libs: monogame, apos.input, nativefiledialognet

Potential future opimizations:
- Use ZLinq for faster linq
- Optimize tile rendering to be better
- Have a 'lazy' ui that draws to a rendertarget only every so often
- Have tilemap be drawn lazily to a rendertarget too