
namespace WebmOpus
{
     public class OpusFormat
    {
        public OpusFormat(float sz,int ch)
        {
            sampleFrequency = sz;
            channels = ch;
        }
        public float sampleFrequency { get; }
        public int channels { get; }
    }
}
