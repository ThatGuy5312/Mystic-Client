using MysticClient.Utils;
using System.Collections;
using UnityEngine;

namespace MysticClient.Classes
{
    public class ColorChanger : TimedBehaviour
    {
        public Renderer gameObjectRenderer;
        public Gradient colors = null;
        public Color color;
        public bool timeBased = true;
        public bool useRigMaterial = false;
        public bool rainbow = false;
        private Gradient rainbowGradient = new Gradient
        {
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(Color.red, 0f),
                new GradientColorKey(Color.green, .2f),
                new GradientColorKey(Color.blue, .4f)
            }
        };
        public override void Start()
        {
            base.Start();
            gameObjectRenderer = GetComponent<Renderer>();
        }
        public override void Update()
        {
            base.Update();
            if (colors != null && gameObjectRenderer != null)
            {
                if (timeBased)
                    color = colors.Evaluate(progress);
                gameObjectRenderer.material.color = color;
                gameObjectRenderer.material.SetColor("_EmissionColor", color);
            }
            if (rainbow)
            {
                if (timeBased)
                    color = rainbowGradient.Evaluate(progress);
                gameObjectRenderer.material.color = color;
                gameObjectRenderer.material.SetColor("_EmissionColor", color);
            }
            if (useRigMaterial)
                gameObjectRenderer.material = RigUtils.MyOfflineRig.mainSkin.material;
        }

        public class Clamper : MonoBehaviour
        {
            private Renderer renderer;
            public Renderer target;
            public void Start() => renderer = GetComponent<Renderer>();
            void Update() => renderer.material.color = target.material.color;
        }
    }
}