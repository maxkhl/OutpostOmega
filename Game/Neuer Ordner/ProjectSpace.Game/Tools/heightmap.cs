using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutpostOmega.Game.Tools
{
    /// <summary>
    /// http://ideone.com/RGVf8J
    /// </summary>
    static class heightmap
    {
        static private float persistence = 0.5f;
        static private int NumberOfOctaves = 10;

        static public float Noise(int x, int y)
        {
            long n = x + (y * 57);
            n = (long)((n << 13) ^ n);
            return (float)(1.0F - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
        }

        static public float SmoothNoise(float x, float y)
        {
            float corners = (Noise((int)(x - 1), (int)(y - 1)) + Noise((int)(x + 1), (int)(y - 1)) + Noise((int)(x - 1), (int)(y + 1)) + Noise((int)(x + 1), (int)(y + 1))) / 16;
            float sides = (Noise((int)(x - 1), (int)y) + Noise((int)(x + 1), (int)y) + Noise((int)x, (int)(y - 1)) + Noise((int)x, (int)(y + 1))) / 8;
            float center = Noise((int)x, (int)y) / 4;
            return corners + sides + center;
        }

        static public float CosineInterpolate(float a, float b, float x)
        {
            double ft = x * 3.1415927;
            double f = (1 - Math.Cos(ft)) * 0.5;

            return (float)((a * (1 - f)) + (b * f));
        }

        static public float InterpolatedNoise(float x, float y)
        {
            // MessageBox.Show( x.ToString() );
            int intX = (int)x;
            float fractX = x - intX;

            int intY = (int)y;
            float fractY = y - intY;

            float v1 = SmoothNoise(intX, intY);
            float v2 = SmoothNoise(intX + 1, intY);
            float v3 = SmoothNoise(intX, intY + 1);
            float v4 = SmoothNoise(intX + 1, intY + 1);

            float i1 = CosineInterpolate(v1, v2, fractX);
            float i2 = CosineInterpolate(v3, v4, fractX);

            // MessageBox.Show( intX + "\n" + intY + "\n" + fractX + "\n" + fractY + "\n" + v1 + "\n" + v2 + "\n" + v3 + "\n" + v4 + "\n" + i1 + "\n" + i2 + "\n" + CosineInterpolate( i1, i2, fractY ) );

            return CosineInterpolate(i1, i2, fractY);
        }

        static public float PerlinNoise2D(float x, float y)
        {
            float total = 0;
            float p = persistence;
            int n = NumberOfOctaves;

            for (int i = 0; i < n; i++)
            {
                int frequency = (int)Math.Pow(2, i);
                // MessageBox.Show( Math.Pow( 2, i ).ToString() );
                float amplitude = (int)Math.Pow(p, i);
                total = total + InterpolatedNoise(x * frequency, y * frequency) * amplitude;
            }
            return total;
        }

        static public byte[,] generate(int sizeX, int sizeY, float zoom = 1)
        {
            int zoomX = (int)(sizeX * zoom);
            int zoomY = (int)(sizeY * zoom);

            float max = byte.MinValue;
            float min = byte.MaxValue;

            float[,] nmap = new float[zoomX, zoomY];

            var heightMap = new byte[zoomX, zoomY];

            for (int x = 0; x < zoomX; x++)
            {
                for (int y = 0; y < zoomY; y++)
                {
                    // MessageBox.Show( PerlinNoise2D( x / zoom, y / zoom ).ToString() );

                    nmap[x, y] = PerlinNoise2D(x / zoom, y / zoom);
                    max = (max < nmap[x, y]) ? nmap[x, y] : max;
                    min = (min > nmap[x, y]) ? nmap[x, y] : min;
                }
            }

            max = max - min;
            for (int x = 0; x < zoomX; x++)
            {
                for (int y = 0; y < zoomY; y++)
                {
                    byte calc = (byte)(((nmap[x, y] - min) / max) * 255.0);
                    heightMap[x, y] = calc;
                }
            }

            return heightMap;
        }
    }
}
