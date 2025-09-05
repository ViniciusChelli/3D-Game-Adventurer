using System;

public class WAV
{
    public float[] LeftChannel { get; private set; }
    public int ChannelCount { get; private set; }
    public int SampleCount { get; private set; }
    public int Frequency { get; private set; }

    public WAV(byte[] wav)
    {
        ChannelCount = BitConverter.ToInt16(wav, 22);
        Frequency = BitConverter.ToInt32(wav, 24);
        int pos = 44;

        SampleCount = (wav.Length - pos) / 2;
        LeftChannel = new float[SampleCount];

        for (int i = 0; i < SampleCount; i++)
        {
            short sample = BitConverter.ToInt16(wav, pos);
            LeftChannel[i] = sample / 32768.0f;
            pos += 2;
        }
    }
}
