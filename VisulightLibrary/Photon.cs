namespace VisulightLibrary
{
    public class Photon
    {
        public enum TargetObject { OBJECT_A, OBJECT_B }

        private TargetObject target = TargetObject.OBJECT_B;
        public TargetObject Target
        {
            get
            {
                return target;
            }
            set
            {
                // TODO: Raise OnTargetChange event.
                target = value;
            }
        }

        public double Speed { get; set; } = 299_792.458;

        private int width;
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                if (value < 0)
                {
                    width = 0;
                }
                else
                {
                    width = value;
                }
            }
        }
    }
}
