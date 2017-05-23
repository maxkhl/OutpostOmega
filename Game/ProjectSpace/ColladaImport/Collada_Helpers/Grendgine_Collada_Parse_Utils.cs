using System;

namespace grendgine_collada
{
	public class Grendgine_Collada_Parse_Utils
	{
		public static int[] String_To_Int(string int_array)
		{
			string[] str = int_array.Split(' ');

            int mod = 0;
            if (str[str.Length-1] == "")
                mod = -1;

            int[] array = new int[str.GetLongLength(0) + mod];
			try
			{
				for (long i = 0; i < str.GetLongLength(0) + mod; i++)
				{
					array[i] = Convert.ToInt32(str[i]);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.WriteLine();
				Console.WriteLine(int_array);
			}
			return array;
		}
        public static uint[] String_To_uInt(string int_array)
        {
            string[] str = int_array.Split(' ');
            uint[] array = new uint[str.GetLongLength(0)];
            try
            {
                for (long i = 0; i < str.GetLongLength(0); i++)
                {
                    array[i] = uint.Parse(str[i]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine();
                Console.WriteLine(int_array);
            }
            return array;
        }
		
		public static float[] String_To_Float(string float_array)
		{
			string[] str = float_array.Split(' ');
			float[] array = new float[str.GetLongLength(0)];
			try
			{
				for (long i = 0; i < str.GetLongLength(0); i++)
				{
                    //str[i] = str[i].Replace('.', ',');
                    //array[i] = (float)Convert.ToDecimal(str[i], System.Globalization.CultureInfo.InstalledUICulture);
                    
                    array[i] = float.Parse(str[i], new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.WriteLine();
				Console.WriteLine(float_array);
			}
			return array;
		}
	
		public static bool[] String_To_Bool(string bool_array)
		{
			string[] str = bool_array.Split(' ');
			bool[] array = new bool[str.GetLongLength(0)];
			try
			{
				for (long i = 0; i < str.GetLongLength(0); i++)
				{
					array[i] = Convert.ToBoolean(str[i]);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.WriteLine();
				Console.WriteLine(bool_array);
			}
			return array;
		}
		

		
	}
}