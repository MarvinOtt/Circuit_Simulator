using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI.TextBox
{
    public class UI_TextBox : UI_Element
    {
        Generic_Conf conf;
        System.Windows.Forms.Form form;
        Scintilla t;

        public UI_TextBox(Pos pos, Point size, Generic_Conf conf) : base(new Pos(-10), new Point(-1))
        {
            form = new System.Windows.Forms.Form();
            t = new Scintilla();
            t.CharAdded += scintilla_CharAdded;
            t.InsertCheck += scintilla_InsertCheck;
            t.Location = new System.Drawing.Point(0, 0);
            t.Name = "Code Editor";
            t.Font = new System.Drawing.Font("Times New Roman", 16);
            t.BackColor = System.Drawing.Color.Black;
            t.Size = new System.Drawing.Size(size.X, size.Y);
            t.VScrollBar = true;
            t.HScrollBar = false;
            t.Lexer = Lexer.Cpp;
            t.Styles[Style.Cpp.CommentLine].Font = "Consolas";
            t.Styles[Style.Cpp.CommentLine].Size = 10;
            t.Styles[Style.Cpp.CommentLine].ForeColor = System.Drawing.Color.FromArgb(0, 128, 0); // Green

            t.Styles[Style.Cpp.UserLiteral].Font = "Consolas";
            t.Styles[Style.Cpp.UserLiteral].Size = 10;
            t.Styles[Style.Cpp.UserLiteral].ForeColor = System.Drawing.Color.FromArgb(0, 128, 0); // Green

            t.Styles[Style.Default].Font = "Consolas";
            t.Styles[Style.Default].Size = 10;
            t.Styles[Style.Default].BackColor = System.Drawing.Color.Black;
            t.Styles[Style.Default].ForeColor = System.Drawing.Color.White;
            t.BorderStyle = System.Windows.Forms.BorderStyle.None;
            t.StyleClearAll();
            // Configure the CPP (C#) lexer styles
            t.SetKeywords(0, "break case continue default do else extern false for goto if new operator override return switch this true using virtual while public protected private");
            t.SetKeywords(1, "char double float int long static struct void unsigned");
            //t.SetKeywords(0, "abstract as base break case catch checked continue default delegate do else event explicit extern false finally fixed for foreach goto if implicit in interface internal is lock namespace new null object operator out override params private protected public readonly ref return sealed sizeof stackalloc switch this throw true try typeof unchecked unsafe using virtual while");
            //t.SetKeywords(1, "bool byte char class const decimal double enum float int long sbyte short static string struct uint ulong ushort void");
            t.Styles[Style.Cpp.Default].ForeColor = System.Drawing.Color.Silver;
            t.Styles[Style.Cpp.Comment].ForeColor = System.Drawing.Color.FromArgb(0, 128, 0); // Green
            t.Styles[Style.Cpp.CommentLine].ForeColor = System.Drawing.Color.FromArgb(0, 128, 0); // Green
            t.Styles[Style.Cpp.CommentLineDoc].ForeColor = System.Drawing.Color.FromArgb(128, 128, 128); // Gray
            t.Styles[Style.Cpp.Number].ForeColor = System.Drawing.Color.FromArgb(215, 255, 150);
            t.Styles[Style.Cpp.Word].ForeColor = System.Drawing.Color.FromArgb(87, 156, 215); // Blue
            t.Styles[Style.Cpp.Word2].ForeColor = System.Drawing.Color.FromArgb(87, 156, 215); // Blue
            t.Styles[Style.Cpp.String].ForeColor = System.Drawing.Color.FromArgb(240, 80, 80); // Red
            t.Styles[Style.Cpp.Character].ForeColor = System.Drawing.Color.FromArgb(240, 80, 80); // Red
            t.Styles[Style.Cpp.Verbatim].ForeColor = System.Drawing.Color.FromArgb(240, 80, 80); // Red
            //t.Styles[Style.Cpp.StringEol].BackColor = System.Drawing.Color.Pink;
            t.Styles[Style.Cpp.Operator].ForeColor = System.Drawing.Color.White;
            t.Styles[Style.Cpp.Preprocessor].ForeColor = System.Drawing.Color.Maroon;
            t.Styles[Style.LineNumber].BackColor = System.Drawing.Color.Black;
            t.Styles[Style.LineNumber].ForeColor = System.Drawing.Color.White;
            t.Margins[0].Width = 32;
            //t.Margins[0].Width = 16;
            t.Margins[0].Type = MarginType.Number;
            t.Margins[2].Width = 1;
            t.Margins[2].Type = MarginType.Color;
            t.Margins[2].BackColor = System.Drawing.Color.Gray;
            t.Margins[1].Width = 8;
            t.Margins[3].Width = 8;
            t.CaretForeColor = System.Drawing.Color.White;
            t.SetSelectionBackColor(true, System.Drawing.Color.FromArgb(100, 100, 100));
            form.Controls.Add(t);
            form.MinimizeBox = false;
            form.Resize += Form_Resize;
            //form.LostFocus += Form_LostFocus;
            form.Deactivate += Form_LostFocus;
            form.FormClosing += Form_Closed;
            //form. += Form_LostFocus;
            Form_Resize(null, null);
            
        }

        public void scintilla_CharAdded(object sender, CharAddedEventArgs e)
        {
            // Find the word start
            var currentPos2 = t.CurrentPosition;
            var wordStartPos = t.WordStartPosition(currentPos2, true);

            // Display the autocompletion list
            var lenEntered = currentPos2 - wordStartPos;
            if (lenEntered > 0)
            {
                if (true || !t.AutoCActive)
                {
                    //"char double float int long static struct void unsigned"
                    string autokeywords = "abstract as base break case catch char checked continue default delegate do double else event explicit extern false finally fixed float for foreach goto if implicit in int interface internal is lock long namespace new null object operator out override params private protected public readonly ref return sealed sizeof stackalloc static struct switch this throw true try typeof unchecked unsafe unsigned using virtual void while";
                    //string[] autokeywords_split = autokeywords.Split(' ');
                    //string curword = t.Text.Substring(wordStartPos, lenEntered);
                    //string[] filtered = autokeywords_split.Where(x => x.StartsWith(curword)).ToArray();
                    //string final = "";
                    //for(int i = 0; i < filtered.Length; ++i)
                    //{
                    //    final += filtered[i] + " ";
                    //}
                    t.AutoCShow(lenEntered, autokeywords);
                }
            }
            if (e.Char == '}')
            {
                var currentPos = t.CurrentPosition;
                t.SearchFlags = SearchFlags.None;

                // Search back from the current position
                t.TargetStart = currentPos;
                t.TargetEnd = 0;

                // Is the bracket following 4 spaces or a tab?
                if (t.SearchInTarget("    }") == (currentPos - 5))
                {
                    // Delete the leading 4 spaces
                    t.DeleteRange((currentPos - 5), 4);
                }
                else if (t.SearchInTarget("\t}") == (currentPos - 2))
                {
                    // Delete the leading tab
                    t.DeleteRange((currentPos - 2), 1);
                }
            }
        }

        private void scintilla_InsertCheck(object sender, InsertCheckEventArgs e)
        {
            if ((e.Text.EndsWith("\r") || e.Text.EndsWith("\n")) && e.Text.Length > 1)
            {
                e.Text.Remove(e.Text.Length - 2);
                var curLine = t.LineFromPosition(e.Position);
                var curLineText = t.Lines[curLine].Text;

                var indent = Regex.Match(curLineText, @"^[ \t]*");
                e.Text += indent.Value; // Add indent following "\r\n"

                // Current line end with bracket?
                if (Regex.IsMatch(curLineText, @"{\s*$"))
                    e.Text += '\t'; // Add tab
            }
        }

        private void Form_Resize(object sender, System.EventArgs e)
        {
            t.Size = new System.Drawing.Size(form.Width - (System.Windows.Forms.SystemInformation.VerticalScrollBarWidth - 1), form.Height - (40 - 1));
        }
        private void Form_Closed(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            e.Cancel = true;
            form.Hide();
        }

        private void Form_LostFocus(object sender, EventArgs e)
        {
            form.BringToFront();
            form.Activate();
            form.Hide();
        }

        public void Show()
        {
            form.Show();
        }

        protected override void UpdateSpecific()
        {
            //t.Size = new System.Drawing.Size(form.Width - (System.Windows.Forms.SystemInformation.VerticalScrollBarWidth - 1), form.Height - (40 - 1));
            //base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);
        }
    }
}
