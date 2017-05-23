using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace OutpostOmega.Tools
{
    static class Screen
    {        
        public struct ScreenSetting
        {
            public uint Width;
            public uint Height;
            public int BitsPerPixel;
        }

        public static List<ScreenSetting> GetAvailableResolutions()
        {
            var resolutions = new List<ScreenSetting>();
            var scope = new ManagementScope();

            var query = new ObjectQuery("SELECT * FROM CIM_VideoControllerResolution");

            using (var searcher = new ManagementObjectSearcher(scope, query))
            {
                var results = searcher.Get();

                //previous resolution for checking when new resolution is found
                string prevReso = string.Empty;
                //current resolution
                string currReso = string.Empty;

                foreach (var result in results)
                {
                    currReso = result["HorizontalResolution"] + "x" + result["VerticalResolution"];

                    if (currReso != prevReso)
                    {
                        resolutions.Add(new ScreenSetting()
                            {
                                Width = (uint)result["HorizontalResolution"],
                                Height = (uint)result["VerticalResolution"]

                            });
                        /*Console.WriteLine(
                            "resolution={0}x{1}",
                            result["HorizontalResolution"],
                            result["VerticalResolution"]);
                        Console.WriteLine();*/

                        prevReso = currReso;
                    }
                }
            }



            return resolutions;
        }

    }
}
