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
                {
                    color = colors.Evaluate(progress);
                }
                gameObjectRenderer.material.color = color;
                gameObjectRenderer.material.SetColor("_EmissionColor", color);
            }
        }
    }
}