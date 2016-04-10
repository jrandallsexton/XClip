using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace XClip.Utils
{
    public class NAudioPlayground
    {
        public void Foo()
        {
            var file = @"H:\XClip\Audio\Stone Temple Pilots - Sex Type Thing.mp3";
            int xPos = 2;
            var yScale = 50;

            using (var reader = new AudioFileReader(file))
            {
                var samplePerSecond = reader.WaveFormat.SampleRate*reader.WaveFormat.Channels;
                var readBuffer = new float[samplePerSecond];

                int samplesRead;
                var idx = 0;

                do
                {
                    samplesRead = reader.Read(readBuffer, 0, samplePerSecond);

                    if (samplesRead > 0)
                    {
                        var max = readBuffer.Take(samplesRead).Max();
                        var top = yScale + max * yScale;
                        var bottom = yScale - max * yScale;
                        xPos += 2;
                        Console.WriteLine("{0}: {1}\t{2}", reader.CurrentTime, top, bottom);
                        idx++;
                    }
                } while (samplesRead > 0);
            }
        }
    }
}
