using JetBrains.Annotations;
using UnityEngine;

namespace CHAMCHI.BehaviourEditor
{
    public class ContentStyle
    {
        public ContentStyle(Font font)
        {
            this.font = font;
        }
        
        [CanBeNull] public Font font;
    }
}