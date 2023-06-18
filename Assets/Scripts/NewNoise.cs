using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class NewNoise
{
    public struct noise2DMapData
    {
        public int mapWidth;
        public int mapHeight;
        public int seed;
        public float scale;
        public int octaves;
        public float persistance;
        public float lacunarity;
        public Vector2 offset;
    }
    internal class Noise2D
    {
        private int mapWidth;
        private int mapHeight;
        private int seed = 0;
        private float scale;
        private int octaves;
        private float persistance;
        private float lacunarity;
        private Vector2 offset;
        private Noise2D parentMap = null;
        private Noise2D displacementMapX = null;
        private Noise2D displacementMapY = null;
        private Noise2D head = null;
        private Perlin perlin;

        public int MapWidth
        {
            set
            {
                if (this == head)
                {
                    if (value >= 1)
                    {
                        this.mapWidth = value;
                    }
                    else
                    {
                        this.mapWidth = 1;
                    }
                }
            }
            get
            {
                return this.mapWidth;
            }
        }
        public int MapHeight
        {
            set
            {
                if (this == head)
                {
                    if (value >= 1)
                    {
                        this.mapHeight = value;
                    }
                    else
                    {
                        this.mapHeight = 1;
                    }
                }
            }
            get
            {
                return this.mapHeight;
            }
        }
        public int Seed
        {
            set
            {
                this.seed = value;
                perlin = new Perlin(value);
            }
            get
            {
                return this.seed;
            }
        }
        public float Scale
        {
            set
            {
                if (value <= 1)
                {
                    this.scale = 1.0001f;
                }
                else
                {
                    this.scale = value;
                }
            }
            get { return this.scale; }
        }
        public int Octaves
        {
            set
            {
                if (value <= 0)
                {
                    this.octaves = 1;
                }
                else
                {
                    this.octaves = value;
                }
            }
            get { return this.octaves; }
        }
        public float Persistance
        {
            set
            {
                if (value < 0)
                {
                    this.persistance = 0;
                }
                else if (value > 1)
                {
                    this.persistance = 1;
                }
                else
                {
                    this.persistance = value;
                }
            }
            get { return this.persistance; }
        }
        public float Lacunarity
        {
            set
            {
                if (value < 1)
                {
                    this.lacunarity = 1;
                }
                else
                {
                    this.lacunarity = value;
                }
            }
            get { return this.lacunarity; }
        }
        public Vector2 Offset
        {
            set
            {
                this.offset = value;
            }
            get { return this.offset; }
        }
        public Noise2D ParentMap
        {
            get { return this.parentMap; }
        }
        public Noise2D DisplacementMapX
        {
            get { return this.displacementMapX; }
        }
        public Noise2D DisplacementMapY
        {
            get { return this.displacementMapY; }
        }
        public Noise2D Head
        {
            get { return this.head; }
        }

        public static float FindMinMaxValue(float[,] noiseMap, string howValueYouNeed = "-")
        {
            float MAXVALL = float.MinValue;
            float MINVALL = float.MaxValue;
            if (howValueYouNeed == "-")
            {
                for (int i = 0; i < noiseMap.GetUpperBound(0) + 1; i++)
                {
                    for (int j = 0; j < noiseMap.GetUpperBound(1) + 1; j++)
                    {
                        if (MINVALL > noiseMap[i, j])
                            MINVALL = noiseMap[i, j];
                    }
                }
            }
            else if (howValueYouNeed == "+")
            {
                for (int i = 0; i < noiseMap.GetUpperBound(0) + 1; i++)
                {
                    for (int j = 0; j < noiseMap.GetUpperBound(1) + 1; j++)
                    {
                        if (MAXVALL < noiseMap[i, j])
                            MAXVALL = noiseMap[i, j];
                    }
                }
                return MAXVALL;
            }
            return MINVALL;
        }

        // Нормализует значения между 0 и 1
        public static float InverseLerp(float minNoiseHeight, float maxNoiseHeight, float inputValue)
        {
            return (inputValue - minNoiseHeight) / (maxNoiseHeight - minNoiseHeight);
        }

        public Noise2D(noise2DMapData val)
        {
            if (val.mapWidth <= 0)
            {
                mapWidth = 1;
            }
            else
            {
                mapWidth = val.mapWidth;
            }
            if (val.mapHeight <= 0)
            {
                mapHeight = 1;
            }
            else
            {
                mapHeight = val.mapHeight;
            }
            if (val.scale <= 1)
            {
                scale = 1.0001f;
            }
            else
            {
                scale = val.scale;
            }
            if (val.octaves <= 0)
            {
                octaves = 1;
            }
            else
            {
                octaves = val.octaves;
            }
            if (val.persistance < 0)
            {
                persistance = 0;
            }
            else if (val.persistance > 1)
            {
                persistance = 1;
            }
            else
            {
                persistance = val.persistance;
            }
            if (val.lacunarity < 1)
            {
                lacunarity = 1;
            }
            else
            {
                lacunarity = val.lacunarity;
            }
            if (val.scale <= 0)
            {
                scale = 0.0001f;
            }
            else
            {
                scale = val.scale;
            }
            offset = val.offset;
            this.head = this;
            this.seed = val.seed;
            perlin = new Perlin(this.seed);
        }

        public static void addDisplacementMap(ref Noise2D noiseMap2D, noise2DMapData val, string axis)
        {
            // axis == a(x & y) axis == x(x) axis == y(y)
            if (axis != "a" && axis != "x" && axis != "y")
            {
                axis = "a";
            }
            val.mapWidth = noiseMap2D.head.mapWidth;
            val.mapHeight = noiseMap2D.head.mapHeight;
            if (axis == "a")
            {
                noiseMap2D.displacementMapX = new Noise2D(val);
                noiseMap2D.displacementMapY = noiseMap2D.displacementMapX;
                noiseMap2D.displacementMapX.parentMap = noiseMap2D.displacementMapY.parentMap = noiseMap2D;
                noiseMap2D.displacementMapX.head = noiseMap2D.head;
                noiseMap2D.displacementMapY.head = noiseMap2D.head;
            }
            else if (axis == "x")
            {
                noiseMap2D.displacementMapX = new Noise2D(val);
                noiseMap2D.displacementMapX.parentMap = noiseMap2D;
                noiseMap2D.displacementMapX.head = noiseMap2D.head;
            }
            else
            {
                noiseMap2D.displacementMapY = new Noise2D(val);
                noiseMap2D.displacementMapY.parentMap = noiseMap2D;
                noiseMap2D.displacementMapY.head = noiseMap2D.head;
            }
        }

        public static noise2DMapData get2DMapData(Noise2D noiseMap2D)
        {
            noise2DMapData data = new noise2DMapData();
            data.mapWidth = noiseMap2D.mapWidth;
            data.mapHeight = noiseMap2D.mapHeight;
            data.seed = noiseMap2D.seed;
            data.scale = noiseMap2D.scale;
            data.octaves = noiseMap2D.octaves;
            data.persistance = noiseMap2D.persistance;
            data.lacunarity = noiseMap2D.lacunarity;
            data.offset = noiseMap2D.offset;
            return data;
        }

        public static void set2DMapData(ref Noise2D noiseMap2D, noise2DMapData mapData)
        {
            noiseMap2D.MapWidth = mapData.mapWidth;
            noiseMap2D.MapHeight = mapData.mapHeight;
            noiseMap2D.Seed = mapData.seed;
            noiseMap2D.Scale = mapData.scale;
            noiseMap2D.Octaves = mapData.octaves;
            noiseMap2D.Persistance = mapData.persistance;
            noiseMap2D.Lacunarity = mapData.lacunarity;
            noiseMap2D.Offset = mapData.offset;
        }

        public float[,] generateNoiseMap()
        {
            //создаем двухмерный массив
            float[,] noiseMap = new float[this.mapWidth, this.mapHeight];

            //генератор псевдо случайных чисел
            //System.Random prng = new System.Random(this.seed);
            //смещения октавы
            //PointF[] octaveOffsets = new PointF[this.octaves];
            //for (int i = 0; i < this.octaves; i++)
            //{
            //    //нельзя давать слижком высокие координаты, поэтому ограничиваем
            //    //offset нужен для ручного смещения по шуму перлина
            //    float offsetX = prng.Next(-100000, 100000) + this.offset.X;
            //    float offsetY = prng.Next(-100000, 100000) + this.offset.Y;
            //    //смещение всех октав, но для всех в одни координаты, как я понял (можно улучшить)
            //    octaveOffsets[i] = new PointF(offsetX, offsetY);
            //}

            //проверка на то что не делится на масштаб, равный нулю
            if (this.scale <= 0)
            {
                this.scale = 0.0001f;
            }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            //параметры для скейла от центра, а не верхнего правого края
            float halfWidth = this.mapWidth / 2f;
            float halfHeight = this.mapHeight / 2f;

            float[,] xDisplacementMap = null;
            float[,] yDisplacementMap = null;

            //карты смещения
            if (this.displacementMapX != null)
            {
                xDisplacementMap = this.displacementMapX.generateNoiseMap();
            }

            if (this.displacementMapY != null)
            {
                if (this.displacementMapX == this.displacementMapY)
                {
                    yDisplacementMap = xDisplacementMap;
                }
                else
                {
                    yDisplacementMap = this.displacementMapY.generateNoiseMap();
                }
            }

            //прогонка по карте шума
            for (int y = 0; y < this.mapHeight; y++)
                for (int x = 0; x < this.mapWidth; x++)
                {
                    float perlinValue = 0;

                    float sampleX = (x + this.offset.x) / this.scale;
                    float sampleY = (y - this.offset.y) / this.scale;


                    if (xDisplacementMap != null && yDisplacementMap != null)
                    {
                        perlinValue = perlin.OctavePerlin(sampleX + xDisplacementMap[x, y], sampleY + yDisplacementMap[x, y], 0, this.octaves, this.persistance, this.lacunarity);
                    }
                    else if (yDisplacementMap != null && xDisplacementMap == null)
                    {
                        perlinValue = perlin.OctavePerlin(sampleX, sampleY + yDisplacementMap[x, y], 0, this.octaves, this.persistance, this.lacunarity);
                    }
                    else if (xDisplacementMap != null && yDisplacementMap == null)
                    {
                        perlinValue = perlin.OctavePerlin(sampleX + xDisplacementMap[x, y], sampleY, 0, this.octaves, this.persistance, this.lacunarity);
                    }
                    else
                    {
                        perlinValue = perlin.OctavePerlin(sampleX, sampleY, 0, this.octaves, this.persistance, this.lacunarity);
                    }

                    //float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    //r-turbulence
                    //float perlinValue = ( 1 - Math.Abs(PerlinNoise2D.Noise(sampleX, sampleY, data.scale, data.seed)) );
                    //fBm
                    //perlinValue = perlin.OctavePerlin((sampleX / this.scale), (sampleY / this.scale), 0, this.octaves, this.persistance);

                    //оперделение максимальных значений
                    if (perlinValue > maxNoiseHeight)
                    {
                        maxNoiseHeight = perlinValue;
                    }
                    else if (perlinValue < minNoiseHeight)
                    {
                        minNoiseHeight = perlinValue;
                    }
                    //перменение высоты шума к карте шума
                    noiseMap[x, y] = perlinValue;
                }

            for (int y = 0; y < this.mapHeight; y++)
                for (int x = 0; x < this.mapWidth; x++)
                {
                    noiseMap[x, y] = InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }

            return noiseMap;
        }

        public static void switchToABranch(ref Noise2D noiseMap2D, string branch)
        {
            switch (branch)
            {
                case "x":
                    if (noiseMap2D.displacementMapX != null)
                    {
                        noiseMap2D = noiseMap2D.displacementMapX;
                    }
                    break;
                case "y":
                    if (noiseMap2D.displacementMapY != null)
                    {
                        noiseMap2D = noiseMap2D.displacementMapY;
                    }
                    break;
                case "p":
                    if (noiseMap2D.parentMap != null)
                    {
                        noiseMap2D = noiseMap2D.parentMap;
                    }
                    break;
            }
        }

        public static void swapMaps(ref Noise2D noiseMap2DA, ref Noise2D noiseMap2DB)
        {
            if (noiseMap2DA != null && noiseMap2DB != null)
            {
                if (noiseMap2DA.Head == noiseMap2DB.Head)
                {
                    noise2DMapData entermediateValue = get2DMapData(noiseMap2DA);
                    set2DMapData(ref noiseMap2DA, get2DMapData(noiseMap2DB));
                    set2DMapData(ref noiseMap2DB, entermediateValue);
                }
            }
        }

        //public static void deleteMap(ref Noise2D noiseMap2D)
        //{
        //    if( noiseMap2D != null && noiseMap2D != noiseMap2D.Head)
        //    {
        //        noiseMap2D.ParentMap.
        //    }
        //}
    }

    public class Perlin
    {

        public int repeat;

        public float OctavePerlin(float x, float y, float z, int octaves, float persistence, float lacunarity)
        {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;            // Используется для нормализации результата между 0.0 - 1.0
            for (int i = 0; i < octaves; i++)
            {
                total += perlin(x * frequency, y * frequency, z * frequency) * amplitude;

                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return total / maxValue;
        }

        private static int[] p;

        private int[] givenSeedPermutation;

        public Perlin(int givenSeed)
        {
            givenSeedPermutation = new int[256];
            System.Random rand = new System.Random(givenSeed);
            for (int x = 0; x < 256; x++)
            {
                givenSeedPermutation[x] = rand.Next(256);
            }
            p = new int[512];
            for (int x = 0; x < 512; x++)
            {
                p[x] = givenSeedPermutation[x % 256];
            }
        }

        public float perlin(float x, float y, float z)
        {
            if (repeat > 0)
            {                                   // If we have any repeat on, change the coordinates to their "local" repetitions
                x = x % repeat;
                y = y % repeat;
                z = z % repeat;
            }

            int xi = (int)x & 255;                              // Calculate the "unit cube" that the point asked will be located in
            int yi = (int)y & 255;                              // The left bound is ( |_x_|,|_y_|,|_z_| ) and the right bound is that
            int zi = (int)z & 255;                              // plus 1.  Next we calculate the location (from 0.0 to 1.0) in that cube.
            float xf = x - (int)x;                             // We also fade the location to smooth the result.
            float yf = y - (int)y;

            float zf = z - (int)z;
            float u = fade(xf);
            float v = fade(yf);
            float w = fade(zf);

            int aaa, aba, aab, abb, baa, bba, bab, bbb;
            aaa = p[p[p[xi] + yi] + zi];
            aba = p[p[p[xi] + inc(yi)] + zi];
            aab = p[p[p[xi] + yi] + inc(zi)];
            abb = p[p[p[xi] + inc(yi)] + inc(zi)];
            baa = p[p[p[inc(xi)] + yi] + zi];
            bba = p[p[p[inc(xi)] + inc(yi)] + zi];
            bab = p[p[p[inc(xi)] + yi] + inc(zi)];
            bbb = p[p[p[inc(xi)] + inc(yi)] + inc(zi)];

            float x1, x2, y1, y2;
            x1 = lerp(grad(aaa, xf, yf, zf),
                        grad(baa, xf - 1, yf, zf),
                        u);
            x2 = lerp(grad(aba, xf, yf - 1, zf),
                        grad(bba, xf - 1, yf - 1, zf),
                          u);
            y1 = lerp(x1, x2, v);

            x1 = lerp(grad(aab, xf, yf, zf - 1),
                        grad(bab, xf - 1, yf, zf - 1),
                        u);
            x2 = lerp(grad(abb, xf, yf - 1, zf - 1),
                          grad(bbb, xf - 1, yf - 1, zf - 1),
                          u);
            y2 = lerp(x1, x2, v);

            return (lerp(y1, y2, w) + 1) / 2;
        }

        public int inc(int num)
        {
            num++;
            if (repeat > 0) num %= repeat;

            return num;
        }

        public static float grad(int hash, float x, float y, float z)
        {
            int h = hash & 15;
            float u = h < 8 /* 0b1000 */ ? x : y;

            float v;

            if (h < 4 /* 0b0100 */)
                v = y;
            else if (h == 12 /* 0b1100 */ || h == 14 /* 0b1110*/)
                v = x;
            else
                v = z;

            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        public static float fade(float t)
        {
            // Функция затухания, определенная Кеном Перлином, также имеет название smothstep.
            // Это позволяет смягчить координатные значения,чтобы они сглаживались к целочисленным значениям.
            // Это приводит к плавному конечному результату.
            return t * t * t * (t * (t * 6 - 15) + 10);         // 6t^5 - 15t^4 + 10t^3
        }

        public static float lerp(float a, float b, float x)
        {
            // Линейная интерполяция
            // (по сути нахождение точки на прямой, используя две точки этой прямой)
            return a + x * (b - a);
        }
    }
}
