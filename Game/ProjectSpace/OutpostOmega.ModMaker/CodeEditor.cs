using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using OutpostOmega.Game;
using OutpostOmega.Game.Lua;
using OutpostOmega.Data;
using System.IO;
using System.Reflection;
using ScintillaNET;


namespace OutpostOmega.ModMaker
{
    class CodeEditor : ScintillaNET.Scintilla
    {
        public string KeyWordList { get; set; }
        public List<MethodInfo> API_Methods { get; set; }
        public ModPack.ModScriptFile ScriptFile;

        public CodeEditor(ModPack.ModScriptFile ScriptFile)
        {
            this.ScriptFile = ScriptFile;

            var lines = File.ReadAllLines(ScriptFile.File.FullName);
            foreach (var line in lines)
                Text += line + Environment.NewLine;

            API_Methods = new List<MethodInfo>();
            

            this.Styles[2].ForeColor = Color.Green; // Comments
            this.Styles[4].ForeColor = Color.Red; // Numbers
            this.Styles[6].ForeColor = Color.Brown; // Strings ("")
            this.Styles[11].ForeColor = Color.DarkBlue; // Rest
            //scEditor.Styles[ScintillaNET.Style.Lua.Keywords].ForeColor = Color.DarkCyan;
            //for (int i = 4; i < 5; i++ )
            //    scEditor.Styles[i].ForeColor = Color.Red;

            this.Lexer = ScintillaNET.Lexer.Lua;
            this.MouseDwellTime = 500;

            this.CharAdded += CodeEditor_CharAdded;
            this.DwellStart += CodeEditor_DwellStart;
            this.DwellEnd += CodeEditor_DwellEnd;
            //var blah = scEditor.DescribeKeywordSets();

            var methods = typeof(OutpostOmega.Game.Lua.Assembly).GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(LuaDocumentationAttr), false);
                if (attributes.Length == 1)
                {
                    KeyWordList += method.Name + this.AutoCSeparator;
                    API_Methods.Add(method);
                }
            }

            SetStyle();
        }

        private void SetStyle()
        {
            scTextChanged(null, null);
            TextChanged += scTextChanged;

            // Instruct the lexer to calculate folding
            SetProperty("fold", "1");
            SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            Margins[2].Type = MarginType.Symbol;
            Margins[2].Mask = Marker.MaskFolders;
            Margins[2].Sensitive = true;
            Margins[2].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                Markers[i].SetForeColor(SystemColors.ControlLightLight);
                Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Configure folding markers with respective symbols
            Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
        }

        void CodeEditor_DwellStart(object sender, ScintillaNET.DwellEventArgs e)
        {
            var currentPos = e.Position;
            var wordStartPos = this.WordStartPosition(currentPos, true);
            var wordEndPos = this.WordEndPosition(currentPos, true);

            var word = this.GetTextRange(wordStartPos, wordEndPos - wordStartPos);

            foreach(var method in API_Methods)
            {
                if(method.Name.ToLower() == word.ToLower())
                {
                    var attributes = method.GetCustomAttributes(typeof(LuaDocumentationAttr), false);
                    if (attributes.Length == 1)
                    {
                        var attribute = (LuaDocumentationAttr)attributes[0];
                        this.CallTipShow(wordStartPos, 
                            String.Format(
                                "{0}" + Environment.NewLine + "{1}" + Environment.NewLine + "{2}" + Environment.NewLine + "{3}",
                                method.ToString(),
                                attribute.Description,
                                attribute.Parameters,
                                "Returns: " + attribute.Return)
                            );
                    }
                    else
                    {
                        this.CallTipShow(wordStartPos, String.Format("{0}: No documentation found", method.Name));
                    }
                    return;
                }
            }
        }

        void CodeEditor_DwellEnd(object sender, ScintillaNET.DwellEventArgs e)
        {
            this.CallTipCancel();
        }

        void CodeEditor_CharAdded(object sender, ScintillaNET.CharAddedEventArgs e)
        {
            var scSender = (ScintillaNET.Scintilla)sender;

            var currentPos = scSender.CurrentPosition;
            var wordStartPos = scSender.WordStartPosition(currentPos, true);
            var currentLine = scSender.Lines[scSender.LineFromPosition(currentPos)];

            var lenEntered = currentPos - wordStartPos;
            if (lenEntered > 0)
            {
                scSender.AutoCShow(lenEntered, KeyWordList);
            }
        }

        /// <summary>
        /// Saves the text to the file
        /// </summary>
        /// <returns>false if aborted</returns>
        public bool Save()
        {
            using (var fileStream = ScriptFile.File.Open(FileMode.Open, FileAccess.ReadWrite))
            {
                byte[] Data = new byte[fileStream.Length];

                fileStream.Seek(0, SeekOrigin.Begin);
                fileStream.Read(Data, 0, (int)fileStream.Length);

                var algorithm = System.Security.Cryptography.SHA1.Create();
                var sourceHash = algorithm.ComputeHash(Data);

                var editorData = DataHandler.Encoder.GetBytes(Text.Substring(0, TextLength - 2));
                var newHash = algorithm.ComputeHash(editorData);

                if (!sourceHash.SequenceEqual(newHash))
                {
                    var dialogResult = System.Windows.Forms.MessageBox.Show("Do you want to save your changes?", ScriptFile.File.Name, System.Windows.Forms.MessageBoxButtons.YesNoCancel);
                    switch (dialogResult)
                    {

                        case System.Windows.Forms.DialogResult.Yes:
                            Save(fileStream);
                            return true;

                        case System.Windows.Forms.DialogResult.No:
                            return true;

                        case System.Windows.Forms.DialogResult.Cancel:
                            return false;
                    }
                }
                else
                    return true;
            }
            return false;
        }

        public void Save(FileStream stream)
        {
            var editorData = DataHandler.Encoder.GetBytes(Text.Substring(0, TextLength - 2));
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(editorData.Length);
            stream.Write(editorData, 0, editorData.Length);
        }

        private int maxLineNumberCharLength;
        private void scTextChanged(object sender, EventArgs e)
        {
            // Did the number of characters in the line number display change?
            // i.e. nnn VS nn, or nnnn VS nn, etc...
            var maxLineNumberCharLength = Lines.Count.ToString().Length;
            if (maxLineNumberCharLength == this.maxLineNumberCharLength)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 2;
            Margins[0].Width = TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
            this.maxLineNumberCharLength = maxLineNumberCharLength;
        }
    }
}
