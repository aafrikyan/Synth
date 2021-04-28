using System;
using System.IO;
using System.Text;

namespace WS.Application
{
    class Program
    {
        public static string monoWaveFileLocation = "";

        static void Main(string[] args)
        {
            Console.WriteLine("Enter mono wave file location: ");
            monoWaveFileLocation = Console.ReadLine();
            WaveFileStructure(monoWaveFileLocation);
            Console.ReadKey();
        }

        public static void WaveFileStructure(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    int fileLength = (int)fileStream.Length;
                    byte[] waveBytes = new byte[fileLength];
                    fileStream.Read(waveBytes, 0, fileLength);

                    #region RIFF chunk descriptor

                    Console.WriteLine("=============================");
                    Console.WriteLine("*** RIFF chunk descriptor ***");
                    Console.WriteLine("=============================");
                    Console.WriteLine();

                    // Chunk ID
                    string chunkId = Encoding.UTF8.GetString(new[] { waveBytes[0], waveBytes[1], waveBytes[2], waveBytes[3] });
                    Console.WriteLine("Chunk ID: {0}", chunkId);

                    // Chunk Size
                    int chunkSize = BitConverter.ToInt32(new byte[] { waveBytes[4], 0, 0, 0 }, 0);
                    Console.WriteLine("Chunk Size: {0}", chunkSize);

                    //Format
                    string format = Encoding.UTF8.GetString(new[] { waveBytes[8], waveBytes[9], waveBytes[10], waveBytes[11] });
                    Console.WriteLine("Format: {0}", format);

                    #endregion

                    #region FTM Sub-chunk

                    Console.WriteLine("=====================");
                    Console.WriteLine("*** FTM Sub-chunk ***");
                    Console.WriteLine("=====================");
                    Console.WriteLine();

                    // Subchunk1 ID
                    string subchunk1Id = Encoding.UTF8.GetString(new[] { waveBytes[12], waveBytes[13], waveBytes[14], waveBytes[15] });
                    Console.WriteLine("Subchunk1 ID: {0}", subchunk1Id);

                    // Subchunk1 Size in bytes
                    int subchunk1Size = BitConverter.ToInt32(new byte[] { waveBytes[16], waveBytes[17], waveBytes[18], waveBytes[19] });
                    Console.WriteLine("Subchunk1 Size: {0}", subchunk1Size);

                    // Audio Compression (pulse code modulation)
                    int pcm = BitConverter.ToInt32(new byte[] { waveBytes[20], waveBytes[21], 0, 0}, 0); // Values other than 1 indicate some form of compression.
                    Console.WriteLine("Audio Compression: {0}", pcm);

                    // Audio Channels: how many channels wave file use
                    int audioChannels = BitConverter.ToInt32(new byte[] { waveBytes[22], waveBytes[23], 0, 0 }, 0);
                    Console.WriteLine("Audio Channels: {0}", audioChannels);

                    // Smaple Rate: indicate how many samples are in a second
                    int sampleRate = BitConverter.ToInt32(new byte[] { waveBytes[24], waveBytes[25], waveBytes[26], waveBytes[27] }, 0);
                    Console.WriteLine("Sample Rate: {0}", sampleRate);

                    // Block align
                    int blockAlign = BitConverter.ToInt32(new byte[] { waveBytes[32], waveBytes[33], 0, 0 }, 0);
                    Console.WriteLine("Block Align: {0}", blockAlign);

                    // Bits Per Sample (bit depth)
                    int bitsPerSample = BitConverter.ToInt32(new byte[] { waveBytes[34], waveBytes[35], 0, 0 }, 0);
                    Console.WriteLine("Bits Per Sample: {0}", bitsPerSample);

                    // Bit Rate: if we divide result to 1024 instead of 8, we will get kbps/s (kilobytes per second). This show how many space it need for 1 second
                    float bitRate = (sampleRate * audioChannels * bitsPerSample) / 8;
                    Console.WriteLine("Bit Rate: {0}", bitRate);

                    #endregion

                    #region Data sub-chunk

                    Console.WriteLine("=====================");
                    Console.WriteLine("*** Data sub-chunk ***");
                    Console.WriteLine("=====================");
                    Console.WriteLine();

                    // Subchunk2 ID
                    string subchunk2Id = Encoding.UTF8.GetString(new[] { waveBytes[36], waveBytes[37], waveBytes[38], waveBytes[39] });
                    Console.WriteLine("Subchunk2 ID: {0}", subchunk2Id);

                    // Subchunk2 Size in bytes
                    int subchunk2Size = BitConverter.ToInt32(new byte[] { waveBytes[40], waveBytes[41], waveBytes[42], waveBytes[43] });
                    Console.WriteLine("Subchunk2 Size: {0}", subchunk2Size);

                    #region Wave Data

                    byte[] waveDataBytes = new byte[waveBytes.Length - 44];
                    Array.Copy(waveBytes, waveDataBytes, 44);

                    int bytesForSample = bitsPerSample / 8;
                    int numberOfValues = subchunk2Size / bytesForSample;

                    // Wave file duration
                    int duration = waveDataBytes.Length / (sampleRate * audioChannels * bitsPerSample / 8);
                    TimeSpan durationTimeSpan = new TimeSpan(0, 0, duration);
                    Console.WriteLine("Wave file duration");
                    Console.WriteLine("\tHours: {0}\n\tMinutes: {1}\n\tSeconds: {2}\n\tMilliseconds: {3}", durationTimeSpan.Hours, durationTimeSpan.Minutes,
                        durationTimeSpan.Seconds, durationTimeSpan.Milliseconds);


                    #endregion

                    #endregion

                    Console.WriteLine();
                    Console.WriteLine("End.");
                }
            }
            else
            {
                Console.WriteLine("File not exists.");
            }
            Console.ReadKey();
        }
    }
}
