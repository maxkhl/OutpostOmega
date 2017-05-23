using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OutpostOmega.Game.Lua;
using OutpostOmega.Data;

namespace OutpostOmega.ModMaker
{
    static class Functions
    {
        public static ModPack.ModScriptFile NewScript(ModPack mod, DirectoryInfo TargetFolder)
        {
            ModPack.ModScriptFile newFile = new ModPack.ModScriptFile();
            var nameDialog = new Dialog.NewString("Scriptname");
            if(nameDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var name = nameDialog.ReturnString;

                var hookDialog = new Dialog.NewEnum("Hook to event", typeof(ModPack.ScriptHook));
                if(hookDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var hook = (ModPack.ScriptHook)hookDialog.ReturnEnum;

                    string tFilePath = Path.Combine(TargetFolder.FullName, name + ".lua");
                    using(var stream = File.Create(tFilePath))
                    {
                        var header = DataHandler.Encoder.GetBytes(
                            String.Format("--OutpostOmega Script "+Environment.NewLine+"-- {0}: {1} ({2}-hook)",
                                mod.Name, name, hook.ToString()));

                        stream.Write(header, 0, header.Length);
                        stream.Close();
                    }

                    newFile = new ModPack.ModScriptFile()
                        {
                            File = new FileInfo(tFilePath),
                            Hook = hook,
                        };

                    mod.Scripts.Add(newFile);

                }
            }
            return newFile;
        }
    }
}
