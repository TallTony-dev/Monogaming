using System;
using Monogaming.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogaming.Interfaces;
using Monogaming.TextureThings;
using System.Runtime.Intrinsics.X86;
using Microsoft.Xna.Framework.Content;

namespace Monogaming.Ui;

public class Wave : UiElement, ImUpdatable
{
    private static Effect waveEffect;
    private static Texture2D BlankTex;
    public static void Initialize(ContentManager content, GraphicsDevice g)
    {
        waveEffect = content.Load<Effect>("waveshader");
        BlankTex = new Texture2D(g, 1, 1);
        BlankTex.SetData(new Color[] { Color.White });
    }
    
    public float Frequency { get; set; }
    public static readonly float Wavelength = 100;
    public float Amplitude { get; set; }
    public float PhaseDiff { get; set; }
    private float currentTime;

    public Wave(Vector2 startPos, int width, int height, float freq, float amplitude, float phaseDiff = 0) 
    : base(startPos, width, height, "")
    {
        Frequency = freq;
        Amplitude = amplitude;
        PhaseDiff = phaseDiff;
    }
    /// <summary>
    /// Returns the resultant sum wave of this wave and another passed in by param
    /// The hitbox is cloned from this wave with new pos
    /// </summary>
    public Wave Sum(Wave wave2, Vector2 pos)
    {
        float newFreq = Frequency;
        float newAmp = MathF.Sqrt(Amplitude * Amplitude + wave2.Amplitude * wave2.Amplitude
        + 2 * Amplitude * wave2.Amplitude * MathF.Cos(PhaseDiff - wave2.PhaseDiff));
        
        float newPhase;
        if (MathF.Abs(PhaseDiff - (wave2.PhaseDiff % (MathF.PI * 2))) == 0)
        {
            newPhase = MathF.Atan((Amplitude * MathF.Sin(PhaseDiff) - wave2.Amplitude * MathF.Sin(wave2.PhaseDiff)) /
            (Amplitude * MathF.Cos(PhaseDiff) - wave2.Amplitude * MathF.Cos(wave2.PhaseDiff)));
        }
        else { newPhase = PhaseDiff; }

        Wave wave = new(pos, hitbox.Width, hitbox.Height, newFreq, newAmp, newPhase);
        return wave;
    }
    public Wave Sum(Wave wave2)
    {
        return Sum(wave2, hitbox.Pos);
    }

    
    //TEMPTEMPTEMP
    public virtual void SetPos(Vector2 pos)
    {
        //should be private
        hitbox.SetPos(pos);
    }

    public override void Update(GameTime gameTime)
    {
        currentTime = (float)gameTime.TotalGameTime.TotalSeconds;
    }

    public void Draw(SpriteBatch spriteBatch, Matrix viewProj)
    {
        waveEffect.Parameters["view_projection"].SetValue(viewProj);
        waveEffect.Parameters["freq"].SetValue(Frequency);
        waveEffect.Parameters["amp"].SetValue(Amplitude);
        waveEffect.Parameters["k"].SetValue(2 * MathF.PI / Wavelength * hitbox.Width);
        waveEffect.Parameters["time"].SetValue(currentTime);
        waveEffect.Parameters["phase"].SetValue(PhaseDiff);
        
        spriteBatch.Begin(effect : waveEffect);
        spriteBatch.Draw(BlankTex, hitbox.AsRect(), Color.White);
        spriteBatch.End();
    }
}