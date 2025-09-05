using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Lê WAV PCM 16-bit (mono/stereo) de byte[] e cria AudioClip.
/// Suporta sample rates comuns (ex.: 22050, 24000, 44100, 48000).
/// </summary>
public static class WAVReader
{
    public static AudioClip ToAudioClip(byte[] wavBytes, string clipName = "TTS_Audio")
    {
        using var stream = new MemoryStream(wavBytes);
        using var reader = new BinaryReader(stream);

        // "RIFF"
        var riff = new string(reader.ReadChars(4));
        if (riff != "RIFF") throw new Exception("Formato inválido: RIFF não encontrado.");

        reader.ReadInt32(); // chunk size (ignorar)

        // "WAVE"
        var wave = new string(reader.ReadChars(4));
        if (wave != "WAVE") throw new Exception("Formato inválido: WAVE não encontrado.");

        // Procura "fmt "
        var fmt = new string(reader.ReadChars(4));
        while (fmt != "fmt ")
        {
            int chunkSizeSkip = reader.ReadInt32();
            reader.ReadBytes(chunkSizeSkip);
            fmt = new string(reader.ReadChars(4));
        }

        int subchunk1Size = reader.ReadInt32();      // 16 para PCM
        ushort audioFormat = reader.ReadUInt16();    // 1 = PCM
        ushort numChannels = reader.ReadUInt16();    // 1 ou 2
        int sampleRate = reader.ReadInt32();         // 22050, 24000, 44100...
        reader.ReadInt32();                          // byteRate (ignorar)
        reader.ReadUInt16();                         // blockAlign (ignorar)
        ushort bitsPerSample = reader.ReadUInt16();  // 16 esperado

        if (audioFormat != 1) throw new Exception("Somente PCM é suportado.");
        if (bitsPerSample != 16) throw new Exception("Somente 16-bit PCM é suportado.");

        // Extra do fmt se houver
        int fmtExtra = subchunk1Size - 16;
        if (fmtExtra > 0) reader.ReadBytes(fmtExtra);

        // Procura "data"
        string dataID = new string(reader.ReadChars(4));
        while (dataID != "data")
        {
            int size = reader.ReadInt32();
            reader.ReadBytes(size);
            dataID = new string(reader.ReadChars(4));
        }

        int dataSize = reader.ReadInt32();
        byte[] pcm = reader.ReadBytes(dataSize);

        int sampleCount = dataSize / 2;
        int samplesPerChannel = sampleCount / numChannels;

        float[] samples = new float[sampleCount];
        int idx = 0;
        for (int i = 0; i < dataSize; i += 2)
        {
            short s = BitConverter.ToInt16(pcm, i);
            samples[idx++] = s / 32768f;
        }

        var clip = AudioClip.Create(clipName, samplesPerChannel, numChannels, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
