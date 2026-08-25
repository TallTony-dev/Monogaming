using Microsoft.Xna.Framework.Audio;

namespace Monogaming
{
    //taken from my old game
    public static class AudioPlayer
    {
        private static SoundEffect[] soundEffects;
        static Vector2 listenerPos;
        static Vector2 listenerVelocity;
        public static float volume = 1;
        internal static List<(SoundEffectInstance instance, Vector2 emitterPos)> activeSoundEffectInstances = new List<(SoundEffectInstance, Vector2)>();


        public static void UpdateListener(Vector2 ListenerPos, Vector2 listenerNewPos)
        {
            listenerPos = ListenerPos;
            listenerVelocity = new Vector2(listenerPos.X - listenerNewPos.X, listenerPos.Y - listenerNewPos.Y);
        }
        public static void Update()
        {
            float overallMovement = listenerVelocity.X + listenerVelocity.Y / 1000;

            for (int i = 0; i < activeSoundEffectInstances.Count; i++)
            {
                if (activeSoundEffectInstances[i].instance.State == SoundState.Stopped)
                {
                    activeSoundEffectInstances[i].instance.Dispose();
                    activeSoundEffectInstances.RemoveAt(i);
                    return;
                }

                if (activeSoundEffectInstances[i].emitterPos != Vector2.One)
                {
                    var instance = Apply3DTransform(activeSoundEffectInstances[i].emitterPos, activeSoundEffectInstances[i].instance, null);
                    activeSoundEffectInstances[i] = (instance, activeSoundEffectInstances[i].emitterPos);
                }

            }

        }
        public static void PlaySoundEffect(ushort index)
        {
            SoundEffectInstance instance = soundEffects[index].CreateInstance();
            instance.Volume = volume;
            instance.Play();
            activeSoundEffectInstances.Add((instance, Vector2.One));
        }
        public static void PlaySoundEffect2D(ushort index, Vector2 emitterPos, Vector2 emitterNewPos)
        {
            SoundEffectInstance instance = soundEffects[index].CreateInstance();
            Apply3DTransform(emitterPos, instance, emitterNewPos);
            instance.Play();
            activeSoundEffectInstances.Add((instance, emitterPos));
        }
        public static void PlaySoundEffect2D(ushort index, Vector2 emitterPos)
        {
            SoundEffectInstance instance = soundEffects[index].CreateInstance();
            Apply3DTransform(emitterPos, instance, null);
            instance.Play();
            activeSoundEffectInstances.Add((instance, emitterPos));
        }
        /// <summary>
        /// EmitterVelocity is a vector of the new position of the emitter, not relative movement.
        /// </summary>
        public static SoundEffectInstance Apply3DTransform(Vector2 emitterPos, SoundEffectInstance instance, Vector2? emitterNewPos)
        {
            float distance = Vector2.Distance(emitterPos, listenerPos);
            instance.Volume = volume;
            if (distance < 1000)
                instance.Volume = instance.Volume * (1 - distance / 1000);
            else
                instance.Volume = 0;
            float deltaX = listenerPos.X - emitterPos.X;
            float deltaY = listenerPos.Y - emitterPos.Y;
            Vector2 normalizedDeltaVals = Vector2.Normalize(new Vector2(deltaX, deltaY));
            instance.Pan = -normalizedDeltaVals.X + 0.9f * normalizedDeltaVals.X;

            float overallMovement = 0;
            if (emitterNewPos != null)
            {
                float velocityTotalX = listenerVelocity.X - (emitterNewPos.Value.X - emitterPos.X);
                float velocityTotalY = listenerVelocity.Y - (emitterNewPos.Value.Y - emitterPos.Y);
                overallMovement = ((normalizedDeltaVals.X * velocityTotalX) + (normalizedDeltaVals.Y * velocityTotalY));
            }
            else
            {
                overallMovement = (normalizedDeltaVals.X * listenerVelocity.X + normalizedDeltaVals.Y * listenerVelocity.Y);
            }

            float pitchChange = overallMovement / 20;
            if (pitchChange > -1 && pitchChange < 1)
                instance.Pitch = pitchChange;
            else instance.Pitch = Math.Sign(pitchChange);

            return instance;
        }
    }
}
