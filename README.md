to edit mgcb: dotnet tool run mgcb-editor-mac ./Content/Content.mgcb

using Apos.Input and Monogame.


drawable and updatable template: 
    public bool IsVisible { get; private set; }
    public bool IsActive { get; private set; } // determines if should be updated
    
    public void Update(GameTime gameTime)
    {
        
    }

    public void Draw()
    {
        
    }